using System;
using UnityEngine;

namespace Project.World
{
    public class Root : MonoBehaviour
    {
        [SerializeField] private MeshFilter _meshFilter;
        
        private void Start()
        {
            _meshFilter.mesh = new Chunk(new DummyMeshGenerator()).GenerateMesh();
        }
    }
}
