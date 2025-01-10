using System;
using Project.World.Generation.Chunks;

namespace Project.World
{
    public class ChunkLODProvider : IChunkLODProvider
    {
        private const int _RADIUS = 4;

        public ChunkLOD GetLevel(ChunkPosition basePosition, ChunkPosition chunkPosition)
        {
            int distance = chunkPosition.Distance(basePosition);

            int lodLevel = 0;
            int lodLevelLimit = ChunkLODs.Number - 1;
            int threshold = _RADIUS;
            while (distance > threshold)
            {
                lodLevel++;

                if (lodLevel >= lodLevelLimit)
                    break;

                threshold += _RADIUS << lodLevel;
            } 

            return ChunkLODs.FromInt(lodLevel);
        }
    }
    
    public class IncrementalLODProvider : IChunkLODProvider
    {
        public ChunkLOD GetLevel(ChunkPosition basePosition, ChunkPosition chunkPosition) =>
            ChunkLODs.FromInt(Math.Min(chunkPosition.Distance(basePosition), ChunkLODs.Number - 1));
    }
}
