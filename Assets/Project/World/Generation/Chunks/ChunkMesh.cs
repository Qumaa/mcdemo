using UnityEngine;

namespace Project.World.Generation.Chunks
{
    public class ChunkMesh
    {
        public readonly Mesh Mesh;
        public ChunkMesh(Mesh mesh) {
            Mesh = mesh;
        }
        
        public static implicit operator ChunkMesh(Mesh mesh) => new(mesh);
    }
}
