using Project.World.Generation.Block;
using Project.World.Generation.Chunk;
using UnityEngine;

namespace Project.World
{
    public class Root : MonoBehaviour
    {
        [SerializeField] private MeshFilter _meshFilter;
        
        private void Start()
        {
            IMeshGenerator meshGenerator = new MeshGenerator(new DummyMeshProvider());
            
            _meshFilter.mesh = new Chunk(meshGenerator).GenerateMesh();
        }
    }
}
