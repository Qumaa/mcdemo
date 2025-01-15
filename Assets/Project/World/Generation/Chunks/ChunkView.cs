using Project.World.Generation.Blocks;

namespace Project.World.Generation.Chunks
{
    public class ChunkView : IChunkView
    {
        public ChunkView(ChunkFaces faces)
        {
            Faces = faces;
        }

        public ChunkFaces Faces { get; }

        public ChunkMesh Mesh { get; private set; }

        public void SetMesh(ChunkMesh mesh)
        {
            Mesh?.Dispose();
            Mesh = mesh;

            foreach (FaceDirection direction in FaceDirections.Array)
                Faces[direction].SetMesh(mesh.Meshes[direction]);
        }
    }
}
