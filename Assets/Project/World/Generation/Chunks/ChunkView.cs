using Project.World.Generation.Blocks;

namespace Project.World.Generation.Chunks
{
    public class ChunkView : IChunkView
    {
        public ChunkView(ChunkFaceViews faceViews)
        {
            Faces = faceViews;
        }

        public ChunkFaceViews Faces { get; }

        public ChunkMesh Mesh { get; private set; }

        public void SetMesh(ChunkMesh mesh)
        {
            Mesh?.Dispose();
            Mesh = mesh;

            foreach (FaceDirection direction in FaceDirections.Array)
                Faces[direction].SetMesh(mesh.Faces[direction]?.Mesh);
        }
    }
}
