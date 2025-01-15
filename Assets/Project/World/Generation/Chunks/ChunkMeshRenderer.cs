using System;
using Project.World.Generation.Blocks;
using UnityEngine;

namespace Project.World.Generation.Chunks
{
    public class ChunkMeshRenderer : MonoBehaviour, IChunkView
    {
        private ChunkView _view;

        [SerializeField] private ChunkFaceStruct _top;
        [SerializeField] private ChunkFaceStruct _bottom;
        [SerializeField] private ChunkFaceStruct _right;
        [SerializeField] private ChunkFaceStruct _left;
        [SerializeField] private ChunkFaceStruct _front;
        [SerializeField] private ChunkFaceStruct _back;

        private void Awake()
        {
            ChunkFaces faces = new(
                new(_top.ToFaceClass(), FaceDirection.Up),
                new(_bottom.ToFaceClass(), FaceDirection.Down),
                new(_right.ToFaceClass(), FaceDirection.Right),
                new(_left.ToFaceClass(), FaceDirection.Left),
                new(_front.ToFaceClass(), FaceDirection.Forward),
                new(_back.ToFaceClass(), FaceDirection.Back)
            );

            _view = new(faces);
        }

        public ChunkFaces Faces => _view.Faces;
        public ChunkMesh Mesh => _view.Mesh;

        public void SetMesh(ChunkMesh mesh) =>
            _view.SetMesh(mesh);

        [Serializable]
        private struct ChunkFaceStruct
        {
            public MeshRenderer Renderer;
            public MeshFilter Filter;

            public ChunkFace ToFaceClass() =>
                new(Filter, Renderer);
        }
    }
}
