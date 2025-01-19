using System;
using Project.World.Generation.Blocks;
using UnityEngine;
using Object = UnityEngine.Object;

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

    public class ChunkFace : IDisposable
    {
        public Mesh Mesh { get; private set; }
        public EdgeOpacity Opacity { get; }

        public ChunkFace(Mesh mesh, EdgeOpacity opacity)
        {
            Mesh = mesh;
            Opacity = opacity;
        }

        public void Dispose()
        {
            Object.Destroy(Mesh);
            Mesh = null;
        }
    }
}
