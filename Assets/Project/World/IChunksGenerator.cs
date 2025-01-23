namespace Project.World
{
    public interface IChunksGenerator
    {
        Chunks Generate(ChunkPosition center, int loadDistance);
    }
}
