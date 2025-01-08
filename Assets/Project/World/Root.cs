using System;
using Project.World.Generation.Blocks;
using Project.World.Generation.Chunks;
using Project.World.Generation.Terrain;
using UnityEngine;

namespace Project.World
{
    public class Root : MonoBehaviour
    {
        [SerializeField] private GameObject _prefab;
        [SerializeField] private int _chunksToGenerate = 16;

        private void Start()
        {
            IChunkMeshGenerator chunkMeshGenerator = new LODChunkMeshGenerator(new DummyMeshProvider());
            IBlocksIteratorProvider iteratorProvider = new BlocksIteratorProvider(new DummyBlockGenerator());
            ChunkLODLevelProvider lodLevelProvider = new(new(Vector3Int.zero));

            for (int x = 0; x < _chunksToGenerate; x++)
            for (int z = 0; z < _chunksToGenerate; z++)
            {
                GameObject chunkObject = Instantiate(
                    _prefab,
                    new Vector3(x, 0, z) * Chunk.STANDARD_SIZE,
                    Quaternion.identity
                );

                IChunkMeshSetter meshSetter = chunkObject.GetComponent<IChunkMeshSetter>();
                ChunkPosition position = ChunkPosition.FromWorld(chunkObject.transform.position);
                Chunk chunk = new(position, chunkMeshGenerator, iteratorProvider, meshSetter);

                chunk.GenerateMesh(lodLevelProvider.GetLevel(new(x, 0, z)));
            }
        }

        [Serializable]
        private struct LODMeshFilterPair
        {
            [field: SerializeField] public MeshFilter MeshFilter { get; private set; }
            [field: SerializeField] public ChunkLOD ChunkLOD { get; private set; }
        }
    }

    public class World
    {
        private readonly IChunkMeshGenerator _meshGenerator;
        private readonly IBlocksIteratorProvider _iteratorProvider;
        private readonly IChunkLODLevelProvider _lodLevelProvider;
        
        public World(IChunkMeshGenerator meshGenerator, IBlocksIteratorProvider iteratorProvider)
        {
            _meshGenerator = meshGenerator;
            _iteratorProvider = iteratorProvider;
        }
    }
}
