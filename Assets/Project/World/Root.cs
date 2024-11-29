using System;
using Project.World.Generation.Blocks;
using Project.World.Generation.Chunks;
using Project.World.Generation.Terrain;
using UnityEngine;

namespace Project.World
{
    public class Root : MonoBehaviour
    {
        [SerializeField] private LODMeshFilterPair[] _chunks;
        
        private void Start()
        {
            IMeshGenerator meshGenerator = new LODMeshGenerator(new DummyMeshProvider(), 16);
            IBlockIteratorProvider iteratorProvider = new BlockIteratorProvider(new DummyBlockGenerator());

            var chunk = new Chunk(meshGenerator, iteratorProvider);

            foreach (LODMeshFilterPair pair in _chunks)
                pair.meshFilter.mesh = chunk.GenerateMesh(pair.chunkLOD).Mesh;
        }

        [Serializable]
        private struct LODMeshFilterPair
        {
            [field: SerializeField] public MeshFilter meshFilter { get; private set; }
            [field: SerializeField] public ChunkLOD chunkLOD { get; private set; }
        }
    }
}
