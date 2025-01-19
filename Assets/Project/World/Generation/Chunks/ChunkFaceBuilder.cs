using System.Collections.Generic;
using Project.World.Generation.Blocks;
using UnityEngine;

namespace Project.World.Generation.Chunks
{
    public class ChunkFaceBuilder
    {
        private readonly List<Vector3> _vertices = new(1024);
        private readonly List<Vector3> _normals = new(1024);
        private readonly List<int> _triangles = new(1536);
        private int _transparentEdgeCounter = 0;

        public void AddBlockFace(Vector3 position, BlockFace face, float verticesScaler, bool transparentOnEdge)
        {
            int verticesOffset = _vertices.Count;
            position *= verticesScaler;
                
            if (transparentOnEdge)
                AddTransparentBlockFace();

            foreach (Vector3 vertex in face.Vertices)
                _vertices.Add(position + (vertex * verticesScaler));

            foreach (int vertexIndex in face.Triangles)
                _triangles.Add(verticesOffset + vertexIndex);

            foreach (Vector3 normal in face.Normals)
                _normals.Add(normal);
        }

        public void AddTransparentBlockFace() =>
            _transparentEdgeCounter++;

        public void Clear()
        {
            _vertices.Clear();
            _normals.Clear();
            _triangles.Clear();
            _transparentEdgeCounter = 0;
        }

        public ChunkFace BuildMesh()
        {
            if (_vertices.Count is 0)
                return new(null, new(_transparentEdgeCounter));
                
            Mesh mesh = new()
            {
                vertices = _vertices.ToArray(), normals = _normals.ToArray(), triangles = _triangles.ToArray()
            };

            mesh.UploadMeshData(true);

            return new(mesh, new(_transparentEdgeCounter));
        }
    }
}
