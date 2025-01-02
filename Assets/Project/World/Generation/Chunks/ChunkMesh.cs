using System;
using Project.World.Generation.Blocks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Project.World.Generation.Chunks
{
    public class ChunkMesh : IDisposable
    {
        public SixFaceData<Mesh> Meshes { get; }

        public ChunkMesh(params Directional<Mesh>[] meshes)
        {
            Meshes = SixFaceData<Mesh>.FromDirectional(meshes);
        }

        public ChunkMesh(SixFaceData<Mesh> meshes)
        {
            Meshes = meshes;
        }

        public void Dispose()
        {
            foreach (Mesh mesh in Meshes)
                Object.Destroy(mesh);
        }
    }
}
