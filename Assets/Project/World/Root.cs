using System.Collections;
using System.Diagnostics;
using Project.World.Generation.Blocks;
using Project.World.Generation.Chunks;
using Project.World.Generation.Terrain;
using UnityEngine;
using UnityEngine.Profiling;
using Debug = UnityEngine.Debug;

namespace Project.World
{
    public class Root : MonoBehaviour
    {
        [SerializeField] private GameObject _prefab;
        [SerializeField] private int _chunksToGenerate = 16;

        private Stopwatch _stopwatch = new();

        private void Start()
        {
            IChunkMeshGenerator chunkMeshGenerator = new LODChunkMeshGenerator(new DummyMeshProvider());
            BlocksIteratorProvider iteratorProvider = new(new DummyBlockGenerator());
            IChunkLODProvider lodProvider = new ChunkLODProvider();
            ChunkPosition basePosition = new(Vector3Int.zero);
            ChunkViewFactory factory = new(_prefab);

            _stopwatch.Start();
            World world = new(basePosition, _chunksToGenerate, chunkMeshGenerator, iteratorProvider, lodProvider, factory);
            _stopwatch.Stop();
            
            Debug.Log($"{_stopwatch.ElapsedMilliseconds}ms to generate chunks");
            Debug.Break();
        }

    }
}
