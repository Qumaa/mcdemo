using Project.World.Generation.Blocks;

namespace Project.World.Generation.Chunks
{
    public interface IChunkView
    {
        ChunkFaceViews Faces { get; }
        ChunkMesh Mesh { get; }
        void SetMesh(ChunkMesh mesh);
        void Cull(DirectionFlags flags);
    }
}
