using System;
using Project.World.Generation.Blocks;

namespace Project.World.Generation.Chunks
{
    public class ChunkMesh : IDisposable
    {
        public SixFaces<ChunkFace> Faces { get; }

        public ChunkMesh(SixFaces<ChunkFace> faces)
        {
            Faces = faces;
        }

        public void Dispose()
        {
            foreach (ChunkFace face in Faces)
                face.Dispose();
        }
    }
}
