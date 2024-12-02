using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Project.World.Generation.Chunks
{
    public class ChunkMesh : IDisposable
    {
        public readonly Mesh Mesh;
        public ChunkMesh(Mesh mesh) 
        {
            Mesh = mesh;
        }
        
        public static implicit operator ChunkMesh(Mesh mesh) => new(mesh);
        public void Dispose()
        {
            Object.Destroy(Mesh);
        }
    }
}
