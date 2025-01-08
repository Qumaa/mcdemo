using Project.World.Generation.Blocks;
using UnityEngine;

namespace Project.World.Generation.Chunks
{
    public class ChunkMeshRenderer : MonoBehaviour, IChunkMeshSetter
    {
        private ChunkMeshSetter _meshSetter;

        [SerializeField] private MeshFilter _top;
        [SerializeField] private MeshFilter _bottom;
        [SerializeField] private MeshFilter _right;
        [SerializeField] private MeshFilter _left;
        [SerializeField] private MeshFilter _front;
        [SerializeField] private MeshFilter _back;

        private void Awake()
        {
            SixFaces<MeshFilter> filters = new(
                new(_top, FaceDirection.Up),
                new(_bottom, FaceDirection.Down),
                new(_right, FaceDirection.Right),
                new(_left, FaceDirection.Left),
                new(_front, FaceDirection.Forward),
                new(_back, FaceDirection.Back)
            );

            _meshSetter = new(filters);
        }

        public void SetMesh(ChunkMesh mesh) =>
            _meshSetter.SetMesh(mesh);
    }
}
