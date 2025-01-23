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
                GenerateAndEnqueue(_chunks.Center);

                while (TryDequeue(out ChunkPosition position))
                    GenerateCompleteChunk(position);

                return _chunks;
            }

            private void GenerateAndEnqueue(ChunkPosition position)
            {
                _chunks.Set(position, GenerateBlocks(position));

                EnqueueForMeshGeneration(position);
            }

            private LODChunk GenerateBlocks(ChunkPosition position)
            {
                LODChunk lodChunk = CreateChunk(position);
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

            private void GenerateCompleteChunk(ChunkPosition position)
            {
                FlatIndexHandle handle = _chunks.GetIndexHandle(position);
                
                foreach (FaceDirection direction in FaceDirections.Array)
                {
                    if (!handle.TryGetNextIndex(direction, out FlatIndexXYZ index))
                        continue;
                    
                    if (GetChunkState(index) is not ChunkGenerationState.PendingBlocksGeneration)
                        continue;
                    
                    ChunkPosition generationPosition = _chunks.IndexToWorld(in index);
                    _chunks.SetDirect(index.Flat, GenerateBlocks(generationPosition));
                }

                ChunkCullingFlags flags = ChunkCullingFlags.FromSignedDifference(_chunks.Center, position);
                Chunk chunk = _chunks.Values[handle.FlatIndex].Chunk;
                ChunkMesh mesh = GenerateCulledMesh(chunk, flags);

                foreach (FaceDirection direction in FaceDirections.Array)
                {
                    ChunkFace face = mesh.Faces[direction];
                    
                    if (face is null or {Opacity: { IsOpaque: true}}) // opaque
                        continue;
                    
                    if (!handle.TryGetNextIndex(direction, out FlatIndexXYZ index)) // no next chunk
                        continue;
                    
                    ChunkGenerationState state = GetChunkState(index);
                    ChunkPosition generationPosition = _chunks.IndexToWorld(in index);
                        
                    if (state is ChunkGenerationState.PendingMeshGeneration)
                        EnqueueForMeshGeneration(generationPosition);
                }
            }

            private ChunkGenerationState GetChunkState(FlatIndexXYZ index)
            {
                Chunk chunk = _chunks.Values[index.Flat].Chunk;

                if (chunk is null)
                    return ChunkGenerationState.PendingBlocksGeneration;

                if (chunk.View.Mesh is null)
                    return _enqueuedPositions.Contains(chunk.Position) ?
                        ChunkGenerationState.EnqueuedForMeshGeneration :
                        ChunkGenerationState.PendingMeshGeneration;

                return ChunkGenerationState.Ready;
            }

            private LODChunk CreateChunk(ChunkPosition position)
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

            private ChunkMesh GenerateCulledMesh(Chunk chunk, ChunkCullingFlags flags)
            {
                ChunkMesh mesh = _context._meshGenerator.Generate(chunk, _chunks, flags);
                
                chunk.View.SetMesh(mesh);

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
