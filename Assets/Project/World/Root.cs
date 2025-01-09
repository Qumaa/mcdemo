using System;
using System.Collections.Generic;
using Project.World.Generation.Blocks;
using Project.World.Generation.Chunks;
using Project.World.Generation.Terrain;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Project.World
{
    public class Root : MonoBehaviour
    {
        [SerializeField] private GameObject _prefab;
        [SerializeField] private int _chunksToGenerate = 16;

        private void Start()
        {
            ChunksIteratorKostyl chunksIterator = new();
            IChunkMeshGenerator chunkMeshGenerator = new LODChunkMeshGenerator(new DummyMeshProvider(), chunksIterator);
            IBlocksIteratorProvider iteratorProvider = new BlocksIteratorProvider(new DummyBlockGenerator());
            ChunkLODProvider lodProvider = new();
            ChunkPosition basePosition = new(Vector3Int.zero);
            ChunkViewFactory factory = new(_prefab);

            World world = new(basePosition, _chunksToGenerate, chunkMeshGenerator, iteratorProvider, lodProvider, factory);
            chunksIterator.World = world;
            world.ForceGenerateChunks();
        }
    }

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

        public bool TryGetChunk(ChunkPosition position, out LODChunk chunk) =>
            _chunks.TryGetValue(position, out chunk);

        public void UpdateChunks(ChunkPosition newCenter)
        {
            int distance = _center.Distance(newCenter);
        }

        public void SetLoadDistance(int distance) =>
            throw new NotImplementedException();

        public void ForceGenerateChunks()
        {
            int width = _loadDistance * 2 + 1;

            ChunkPosition start = _center.OffsetCopy(-_loadDistance, 0, -_loadDistance);

            for (int x = 0; x < width; x++)
            for (int z = 0; z < width; z++)
            {
                ChunkPosition position = start.OffsetCopy(x, 0, z);
                
                GenerateChunk(position);
            }

            foreach (LODChunk lodChunk in _chunks.Values)
                lodChunk.GenerateMesh();
        }

        private void GenerateChunk(ChunkPosition position)
        {
            IChunkView meshSetter = _factory.Create(position);
            
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

    public struct LODChunk 
    {
        public readonly IChunk Chunk;
        public ChunkLOD LOD;

        public LODChunk(IChunk chunk, ChunkLOD lod)
        {
            Chunk = chunk;
            LOD = lod;
        }
        
        public void GenerateBlocks() =>
            Chunk.GenerateBlocks(LOD);

        public void GenerateMesh() =>
            Chunk.GenerateMesh(LOD);
    }

    public interface IChunksSupervisor
    {
        void UpdateChunks(ChunkPosition newCenter);
        void SetLoadDistance(int distance);
        void ForceGenerateChunks();
    }

    public interface IChunksIterator
    {
        public bool TryGetChunk(ChunkPosition position, out LODChunk chunk);
    }

    public class ChunksIteratorKostyl : IChunksIterator
    {
        public World World;
        public bool TryGetChunk(ChunkPosition position, out LODChunk chunk) =>
            World.TryGetChunk(position, out chunk);
    }

    public class ChunkViewFactory
    {
        private readonly GameObject _chunkPrefab;
        
        public ChunkViewFactory(GameObject chunkPrefab) 
        {
            _chunkPrefab = chunkPrefab;
        }

        public IChunkView Create(ChunkPosition position) =>
            Object.Instantiate(_chunkPrefab, position.ToWorld(), Quaternion.identity)
                .GetComponent<IChunkView>();
    }
}
