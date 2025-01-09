using Project.World.Generation.Blocks;
using UnityEngine;

namespace Project.World.Generation.Chunks
{
    public class ChunkView : IChunkView
    {
        private readonly SixFaces<MeshFilter> _filters;

        public ChunkView(SixFaces<MeshFilter> filters)
        {
            _filters = filters;
        }

        public ChunkMesh Mesh { get; private set; }

        public void SetMesh(ChunkMesh mesh)
        {
            Mesh?.Dispose();
            Mesh = mesh;

            foreach (FaceDirection direction in FaceDirections.Array)
                _filters[direction].mesh = mesh.Meshes[direction];
        }
    }
}
