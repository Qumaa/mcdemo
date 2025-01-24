using System.Collections.Generic;
using Project.World.Generation.Chunks;

namespace Project.World
{
    public class IncrementalChunksGenerator : IChunksGenerator
    {
        private readonly IChunkMeshGenerator _meshGenerator;
        private readonly IChunkLODProvider _lodProvider;
        private readonly IBlocksIteratorProvider _blocksProvider;
        private readonly ChunkViewFactory _factory;
        
        public IncrementalChunksGenerator(IChunkMeshGenerator meshGenerator, IBlocksIteratorProvider blocksProvider, IChunkLODProvider lodProvider, ChunkViewFactory factory)
        {
            _meshGenerator = meshGenerator;
            _blocksProvider = blocksProvider;
            _lodProvider = lodProvider;
            _factory = factory;
        }

        public Chunks Generate(ChunkPosition center, int loadDistance) =>
            new GenerationScope(this, center, loadDistance).Generate();
        
        private readonly ref struct GenerationScope
        {
            private readonly IncrementalChunksGenerator _context;
            private readonly Chunks _chunks;
            private readonly Queue<ChunkPosition> _generationQueue;
            private readonly HashSet<ChunkPosition> _enqueuedPositions;
            
            public GenerationScope(IncrementalChunksGenerator context, ChunkPosition center, int loadDistance)
            {
                _context = context;
                int optimalCapacity = GetOptimalQueueSize(loadDistance);
                _generationQueue = new(optimalCapacity);
                _enqueuedPositions = new(optimalCapacity);

                _chunks = new(loadDistance, center);
            }

            public Chunks Generate()
            {
                GenerateBlocksAndEnqueueForMeshGeneration(_chunks.Center);

                while (TryDequeue(out ChunkPosition position))
                    GenerateMesh(position);

                return _chunks;
            }

            private void GenerateBlocksAndEnqueueForMeshGeneration(ChunkPosition position)
            {
                _chunks.Set(position, CreateInitializedChunk(position));

                EnqueueForMeshGeneration(position);
            }

            private LODChunk CreateInitializedChunk(ChunkPosition position)
            {
                LODChunk lodChunk = CreateHollowChunk(position);
                GenerateBlocks(lodChunk);
                return lodChunk;
            }

            private void EnqueueForMeshGeneration(ChunkPosition position)
            {
                _enqueuedPositions.Add(position);
                _generationQueue.Enqueue(position);
            }

            private bool TryDequeue(out ChunkPosition position)
            {
                if (!_generationQueue.TryDequeue(out position))
                    return false;

                _enqueuedPositions.Remove(position);
                return true;
            }

            private void GenerateMesh(ChunkPosition position)
            {
                ChunkHandle chunkHandle = EnsureAdjacentChunksState(in position);
                
                ChunkMesh mesh = GenerateCulledMesh(in chunkHandle);

                EnqueueAdjacentChunks(in chunkHandle, mesh);
            }

            private ChunkHandle EnsureAdjacentChunksState(in ChunkPosition position)
            {
                FlatIndexHandle indexHandle = _chunks.GetIndexHandle(in position);
                ChunkHandle.Mutable handleBuilder = ChunkHandle.GetBuilder(_chunks.GetDirect(indexHandle.FlatIndex));
                
                foreach (FaceDirection direction in FaceDirections.Array)
                {
                    if (!indexHandle.TryGetNextIndex(direction, out FlatIndexXYZ nextIndex))
                        continue;

                    LODChunk adjacent = _chunks.Get(in nextIndex);
                    
                    handleBuilder.Flags.SetFlag(direction, true);
                    
                    if (GetChunkState(adjacent.Chunk) is ChunkGenerationState.PendingBlocksGeneration)
                    {
                        adjacent = CreateInitializedChunk(_chunks.IndexToWorld(in nextIndex));
                        _chunks.SetDirect(nextIndex.Flat, in adjacent);
                    }
                    
                    handleBuilder.Set(direction, in adjacent);
                }

                return handleBuilder.Seal();
            }

            private void EnqueueAdjacentChunks(in ChunkHandle chunkHandle, ChunkMesh mesh)
            {
                foreach (Directional<LODChunk> directional in chunkHandle)
                {
                    ChunkFace face = mesh.Faces[directional.Direction];
                    
                    if (face is null or {Opacity: { IsOpaque: true}})
                        continue;

                    Chunk chunk = directional.Value.Chunk;
                    ChunkGenerationState state = GetChunkState(chunk);

                    if (state is ChunkGenerationState.PendingMeshGeneration)
                        EnqueueForMeshGeneration(chunk.Position);
                }
            }

            private ChunkGenerationState GetChunkState(Chunk chunk)
            {
                if (chunk is null)
                    return ChunkGenerationState.PendingBlocksGeneration;

                if (chunk.View.Mesh is null)
                    return _enqueuedPositions.Contains(chunk.Position) ?
                        ChunkGenerationState.EnqueuedForMeshGeneration :
                        ChunkGenerationState.PendingMeshGeneration;

                return ChunkGenerationState.Ready;
            }

            private LODChunk CreateHollowChunk(ChunkPosition position)
            {
                IChunkView view =  _context._factory.Create(position);
                Chunk chunk = new(position, view);
                
                ChunkLOD lod = _context._lodProvider.GetLevel(_chunks.Center, position);
                
                return new(chunk, lod);
            }

            private void GenerateBlocks(LODChunk lodChunk) =>
                lodChunk.Chunk.Blocks = _context._blocksProvider.GetBlockIterator(
                    lodChunk.Chunk.Position,
                    lodChunk.LOD,
                    lodChunk.Chunk.Blocks
                );

            private ChunkMesh GenerateCulledMesh(in ChunkHandle handle)
            {
                Chunk chunk = handle.Base.Chunk;
                ChunkMesh mesh = _context._meshGenerator.Generate(in handle);
                chunk.View.SetMesh(mesh);
                
                DirectionFlags flags = DirectionFlags.FromSignedDifference(_chunks.Center, chunk.Position);
                chunk.View.Cull(flags);

                return mesh;
            }

            private static int GetOptimalQueueSize(int size) =>
                6 * size * (size + 1); // some magic formula
        }

        private enum ChunkGenerationState
        {
            PendingBlocksGeneration,
            PendingMeshGeneration,
            EnqueuedForMeshGeneration,
            Ready
        }
    }
}
