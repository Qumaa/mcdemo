namespace Project.World.Generation.Chunks
{
    public interface IChunkView
    {
        ChunkMesh Mesh { get; }
        void SetMesh(ChunkMesh mesh);
    }
}
