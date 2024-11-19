using System.Collections.Generic;
using UnityEngine;

namespace Project.World
{
    public class DummyMeshGenerator : IMeshGenerator
    {
        public IMeshBuilder Start() =>
            new Builder();
        
        private class Builder : IMeshBuilder
        {
            private static readonly Vector3[] _cubeVertices =
            {
                // z
                Vector3.zero,                                 // 0
                Vector3.up,                                   // 1
                Vector3.right,                                // 2
                Vector3.right + Vector3.up,                   // 3
                Vector3.zero + Vector3.forward,               // 4
                Vector3.up + Vector3.forward,                 // 5
                Vector3.right + Vector3.forward,              // 6
                Vector3.right + Vector3.up + Vector3.forward, // 7
                // x
                Vector3.zero,                                 // 8
                Vector3.up,                                   // 9
                Vector3.right,                                // 10
                Vector3.right + Vector3.up,                   // 11
                Vector3.zero + Vector3.forward,               // 12
                Vector3.up + Vector3.forward,                 // 13
                Vector3.right + Vector3.forward,              // 14
                Vector3.right + Vector3.up + Vector3.forward, // 15
                // y
                Vector3.zero,                                // 16
                Vector3.up,                                  // 17
                Vector3.right,                               // 18
                Vector3.right + Vector3.up,                  // 19
                Vector3.zero + Vector3.forward,              // 20
                Vector3.up + Vector3.forward,                // 21
                Vector3.right + Vector3.forward,             // 22
                Vector3.right + Vector3.up + Vector3.forward // 23
            };
            
            private static readonly Vector3[] _cubeNormals =
            {
                // z
                Vector3.back, 
                Vector3.back, 
                Vector3.back, 
                Vector3.back, 
                Vector3.forward, 
                Vector3.forward, 
                Vector3.forward, 
                Vector3.forward, 
                // x
                Vector3.left, 
                Vector3.left, 
                Vector3.right,
                Vector3.right,
                Vector3.left, 
                Vector3.left, 
                Vector3.right,
                Vector3.right,
                // y
                Vector3.down, 
                Vector3.up, 
                Vector3.down,
                Vector3.up,
                Vector3.down, 
                Vector3.up, 
                Vector3.down,
                Vector3.up,
            };
            
            private static readonly int[] _cubeTriangles =
            {
                // z
                0, 1, 2,
                2, 1, 3,
                4, 6, 5,
                6, 7, 5,
                // x
                8, 12, 9,
                12, 13, 9,
                10, 11, 14,
                11, 15, 14,
                // y
                16, 18, 20,
                20, 18, 22,
                17, 21, 19,
                21, 23, 19
            };
            
            private Mesh _mesh = new() {name = "Chunk"};
            private List<Vector3> _combinedVertices = new();
            private List<Vector3> _combinedNormals = new();
            private List<int> _combinedTriangles = new();
            
            public void SetBlock(int x, int y, int z, BlockType blockType)
            {
                if (!blockType.IsEmpty)
                    AddCubeToMesh(x, y, z);
            }

            public Mesh Finish()
            {
                _mesh.vertices = _combinedVertices.ToArray();
                _mesh.triangles = _combinedTriangles.ToArray();
                _mesh.normals = _combinedNormals.ToArray();
                
                return _mesh;
            }

            private void AddCubeToMesh(int x, int y, int z)
            {
                int verticesBase = _combinedVertices.Count;
                Vector3 cubeBase = new(x, y, z);
                
                for (var i = 0; i < _cubeVertices.Length; i++)
                    _combinedVertices.Add(cubeBase + _cubeVertices[i]);

                for (var i = 0; i < _cubeTriangles.Length; i++)
                    _combinedTriangles.Add(verticesBase + _cubeTriangles[i]);

                for (int i = 0; i < _cubeNormals.Length; i++)
                    _combinedNormals.Add(_cubeNormals[i]);
            }
        }

        public IMeshBuilder Start(IBlockIterator iterator) =>
            throw new System.NotImplementedException();
    }
}
