namespace Project.World
{
    public interface IChunksSupervisor
    {
        void UpdateChunks(ChunkPosition newCenter);
        void SetLoadDistance(int distance);
    }
}
