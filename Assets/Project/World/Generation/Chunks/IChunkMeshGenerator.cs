namespace Project.World.Generation.Chunks
{
    public interface IChunkMeshGenerator
    {
        ChunkMesh Generate(in ChunkHandle handle);
    }
}
