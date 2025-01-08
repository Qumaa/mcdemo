using System;
using Project.World.Generation.Chunks;
using UnityEngine;

namespace Project.World
{
    public class ChunkLODLevelProvider
    {
        private Vector3Int _basePosition;
        private const int _RADIUS = 4;

        public ChunkLODLevelProvider(Vector3Int basePosition)
        {
            _basePosition = basePosition;
        }

        public ChunkLOD GetLevel(Vector3Int chunkPosition)
        {
            int distance = GetDistance(chunkPosition);

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

        private int GetDistance(Vector3Int chunkPosition)
        {
            Vector3Int difference = chunkPosition - _basePosition;

            return Math.Max(Math.Max(Math.Abs(difference.x), Math.Abs(difference.z)), Math.Abs(difference.y));
        }
        
    }
}
