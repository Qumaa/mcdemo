using UnityEngine;

namespace Project.World.Generation.Chunks
{
    public class ChunkMeshSetter : IChunkMeshSetter
    {
        private readonly MeshFilter _meshFilter;
        
        public ChunkMeshSetter(MeshFilter meshFilter) {
            _meshFilter = meshFilter;
        }

        public void SetMesh(ChunkMesh mesh)
        {
            if (_meshFilter.mesh)
                Object.Destroy(_meshFilter.mesh);
            
            _meshFilter.mesh = mesh.Mesh;
        }
    }
}
