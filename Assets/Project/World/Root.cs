using Project.World.Generation.Blocks;
using Project.World.Generation.Chunks;
using Project.World.Generation.Terrain;
using UnityEngine;

namespace Project.World
{
    public class Root : MonoBehaviour
    {
        [SerializeField] private MeshFilter _meshFilter;
        
        private void Start()
        {
            IMeshGenerator meshGenerator = new MeshGenerator(new DummyMeshProvider());
            IBlockGenerator blockGenerator = new DummyBlockGenerator();
            
            _meshFilter.mesh = new Chunk(meshGenerator, blockGenerator).GenerateMesh();
        }
    }
}
