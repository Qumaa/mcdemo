using UnityEngine;

namespace Project.World.Generation.Chunks
{
    public class ChunkFaceView
    {
        private readonly MeshFilter _filter;
        private readonly MeshRenderer _renderer;
        
        public ChunkFaceView(MeshFilter filter, MeshRenderer renderer)
        {
            _filter = filter;
            _renderer = renderer;
        }

        public void SetVisibility(bool visible) =>
            _renderer.enabled = visible;

        public void SetMesh(Mesh mesh) =>
            _filter.mesh = mesh;
    }
}
