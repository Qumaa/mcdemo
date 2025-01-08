using Project.World.Generation.Blocks;
using UnityEngine;

namespace Project.World.Generation.Chunks
{
    public class ChunkMeshSetter : IChunkMeshSetter
    {
        private readonly ChunkMesh _chunkMesh;
        private readonly SixFaces<MeshFilter> _filters;

        public ChunkMeshSetter(SixFaces<MeshFilter> filters)
        {
            _filters = filters;
        }

        public void SetMesh(ChunkMesh mesh)
        {
            _chunkMesh?.Dispose();

            foreach (FaceDirection direction in FaceDirections.Array)
                _filters[direction].mesh = mesh.Meshes[direction];
        }
    }
}
