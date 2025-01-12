using Project.World.Generation.Blocks;
using Project.World.Generation.Chunks;
using Project.World.Generation.Terrain;
using UnityEngine;
using UnityEngine.Profiling;

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
            IChunkLODProvider lodProvider = new IncrementalLODProvider();
            ChunkPosition basePosition = new(Vector3Int.zero);
            ChunkViewFactory factory = new(_prefab);

            World world = new(basePosition, _chunksToGenerate, chunkMeshGenerator, iteratorProvider, lodProvider, factory);
        }
    }
}
