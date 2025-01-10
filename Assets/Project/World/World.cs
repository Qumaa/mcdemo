using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Project.World.Generation.Chunks;
using UnityEngine.Profiling;

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

        public bool TryGetNextChunk(ChunkPosition position, FaceDirection direction, out IChunk chunk)
        {
            if (_chunks.TryGetValue(position + direction.ToVector(), out LODChunk lodChunk))
            {
                chunk = lodChunk.Chunk;
                return true;
            }

            chunk = null;
            return false;
        }

        public void UpdateChunks(ChunkPosition newCenter)
        {
            int distance = _center.Distance(newCenter);
        }

        public void SetLoadDistance(int distance) =>
            throw new NotImplementedException();

        public void ForceGenerateChunks()
        {
            int width = _loadDistance * 2 + 1;

            ChunkPosition start = _center.OffsetCopy(-_loadDistance, -_loadDistance, -_loadDistance);
            
            for (int x = 0; x < width; x++)
            for (int y = 0; y < width; y++)
            for (int z = 0; z < width; z++)
            {
                ChunkPosition position = start.OffsetCopy(x, y, z);
                
                GenerateChunk(position);
            }

            foreach (LODChunk lodChunk in _chunks.Values.OrderBy(x => x.Chunk.Position.Distance(_center)))
                GenerateChunkMeshAsync(lodChunk);
        }

        private void GenerateChunk(ChunkPosition position)
        {
            Chunk chunk = new(position, _meshGenerator, _blocksProvider);
            ChunkLOD lod = _lodProvider.GetLevel(_center, position);

            LODChunk lodChunk = new(chunk, lod);
            
            _chunks.Add(position, lodChunk);

            lodChunk.GenerateBlocks();
        }

        private async void GenerateChunkMeshAsync(LODChunk lodChunk)
        {
            IChunkView view = await _factory.Create(lodChunk.Chunk.Position);
            
            lodChunk.Chunk.SetView(view);
            lodChunk.GenerateMesh();
        }
        
        

        private void HideChunkFaces()
        {
            
        }

        private struct ChunksEnumerator : IEnumerator<LODChunk>
        {
            private readonly World _world;
            private ChunkPosition _current;
            
            public LODChunk Current => _world._chunks[_current];
            object IEnumerator.Current => Current;
            
            public ChunksEnumerator(World world) 
            {
                _world = world;
                _current = default;
            }

            public bool MoveNext() =>
                throw new NotImplementedException();

            public void Reset() =>
                _current = _world._center;

            public void Dispose() { }
        }
    }
}
