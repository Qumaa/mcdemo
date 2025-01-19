using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Project.World.Generation.Chunks
{
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
