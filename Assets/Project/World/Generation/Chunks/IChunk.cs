namespace Project.World.Generation.Chunks
{
    public interface IChunk
    {
        ChunkMesh GenerateMesh(ChunkLOD lod);
    }
}
