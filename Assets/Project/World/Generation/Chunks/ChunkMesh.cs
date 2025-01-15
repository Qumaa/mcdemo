using System;
using Project.World.Generation.Blocks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Project.World.Generation.Chunks
{
    public class ChunkMesh : IDisposable
    {
        public SixFaces<Mesh> Meshes { get; private set; }

        public ChunkMesh(SixFaces<Mesh> meshes)
        {
            Meshes = meshes;
        }

        public void Dispose()
        {
            foreach (Mesh mesh in Meshes)
                Object.Destroy(mesh);

            Meshes = null;
        }
    }
}
