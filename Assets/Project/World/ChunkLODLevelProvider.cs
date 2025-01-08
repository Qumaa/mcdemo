using Project.World.Generation.Chunks;

namespace Project.World
{
    public class ChunkLODLevelProvider : IChunkLODLevelProvider
    {
        private ChunkPosition _basePosition;
        private const int _RADIUS = 4;

        public ChunkLODLevelProvider(ChunkPosition basePosition)
        {
            _basePosition = basePosition;
        }

        public ChunkLOD GetLevel(ChunkPosition chunkPosition)
        {
            int distance = chunkPosition.Distance(_basePosition);

            int lodLevel = 0;
            int threshold = _RADIUS;
            while (distance > threshold)
            {
                lodLevel++;

                if (lodLevel >= ChunkLODs.Number - 1)
                    break;

                threshold += _RADIUS << lodLevel;
            } 

            return ChunkLODs.FromInt(lodLevel);
        }
    }
}
