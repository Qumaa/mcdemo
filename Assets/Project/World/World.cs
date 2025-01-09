using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Project.World.Generation.Chunks;

namespace Project.World
{
    public class World : IChunksSupervisor, IChunksIterator
    {
        private readonly IChunkMeshGenerator _meshGenerator;
        private readonly IBlocksIteratorProvider _blocksProvider;
        private readonly IChunkLODProvider _lodProvider;
        private readonly ChunkViewFactory _factory;
        private readonly Dictionary<ChunkPosition, LODChunk> _chunks;
        
        private ChunkPosition _center;
        private int _loadDistance;

        public World(ChunkPosition center, int loadDistance, IChunkMeshGenerator meshGenerator,
            IBlocksIteratorProvider blocksProvider, IChunkLODProvider lodProvider, ChunkViewFactory factory)
        {
            _center = center;
            _loadDistance = loadDistance;
            
            _meshGenerator = meshGenerator;
            _blocksProvider = blocksProvider;
            _lodProvider = lodProvider;
            _factory = factory;

            _chunks = new();
        }

        public QueryResult<IChunk> QueryNextChunk(ChunkPosition position, FaceDirection direction) =>
            _chunks.TryGetValue(position + direction.ToVector(), out LODChunk chunk) ?
                QueryResult<IChunk>.Successful(chunk.Chunk) :
                QueryResult<IChunk>.Failed;

        public void UpdateChunks(ChunkPosition newCenter)
        {
            int distance = _center.Distance(newCenter);
        }

        public void SetLoadDistance(int distance) =>
            throw new NotImplementedException();

        public async void ForceGenerateChunks()
        {
            int width = _loadDistance * 2 + 1;
            ValueTask[] chunkGens = new ValueTask[width * width];

            ChunkPosition start = _center.OffsetCopy(-_loadDistance, 0, -_loadDistance);
            
            for (int x = 0; x < width; x++)
            for (int z = 0; z < width; z++)
            {
                ChunkPosition position = start.OffsetCopy(x, 0, z);
                
                chunkGens[x + z * width] = GenerateChunkAsync(position);
            }

            await AwaitAll(chunkGens);

            foreach (LODChunk lodChunk in _chunks.Values)
                lodChunk.GenerateMesh();
        }

        private static async ValueTask AwaitAll(ValueTask[] chunkGens)
        {
            for (int i = 0; i < chunkGens.Length; i++)
                await chunkGens[i].ConfigureAwait(false);
        }

        private async ValueTask GenerateChunkAsync(ChunkPosition position)
        {
            IChunkView meshSetter = await _factory.Create(position);
            
            Chunk chunk = new(position, _meshGenerator, _blocksProvider, meshSetter);
            ChunkLOD lod = _lodProvider.GetLevel(_center, position);

            LODChunk lodChunk = new(chunk, lod);
            
            _chunks.Add(position, lodChunk);

            lodChunk.GenerateBlocks();
        }

        private void HideChunkFaces()
        {
            
        }
    }
}
