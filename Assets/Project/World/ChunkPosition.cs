using System;
using Project.World.Generation.Chunks;
using UnityEngine;

namespace Project.World
{
    public readonly struct ChunkPosition
    {
        private const int _CHUNK_SIZE = Chunk.STANDARD_SIZE;
        
        private readonly Vector3Int _position;

        public int x => _position.x;
        public int y => _position.y;
        public int z => _position.z;
        
        public ChunkPosition(Vector3Int position) 
        {
            _position = position;
        }

        public ChunkPosition(int x, int y, int z) : this(new(x, y, z)) { }

        public Vector3Int ToWorld(Vector3Int localPosition) =>
            _position + localPosition;

        public int Distance(ChunkPosition other) =>
            Distance(this, other);

        public static int Distance(ChunkPosition origin, ChunkPosition other)
        {
            ChunkPosition difference = other - origin;

            return Math.Max(Math.Max(Math.Abs(difference.x), Math.Abs(difference.z)), Math.Abs(difference.y));
        }

        public static ChunkPosition FromWorld(Vector3Int worldPosition) =>
            new(FloorToChunkSize(worldPosition));

        public static ChunkPosition FromWorld(Vector3 worldPosition) =>
            FromWorld(Vector3Int.FloorToInt(worldPosition));

        private static Vector3Int FloorToChunkSize(Vector3Int worldPosition) =>
            new(
                FloorToChunkSize(worldPosition.x),
                FloorToChunkSize(worldPosition.y),
                FloorToChunkSize(worldPosition.z)
            );
        
        private static int FloorToChunkSize(int worldPosition) =>
            worldPosition / _CHUNK_SIZE;

        public static ChunkPosition operator -(ChunkPosition left, ChunkPosition right) =>
            new(left._position - right._position);

        public static ChunkPosition operator +(ChunkPosition left, ChunkPosition right) =>
            new(left._position + right._position);

        public static ChunkPosition operator *(ChunkPosition val, int scaler) =>
            new(val._position * scaler);
        
        public static ChunkPosition operator /(ChunkPosition val, int divider) =>
            new(val._position / divider);
    }
}
