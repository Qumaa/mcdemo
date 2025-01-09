using System;
using Project.World.Generation.Chunks;
using UnityEngine;

namespace Project.World
{
    public readonly struct ChunkPosition : IEquatable<ChunkPosition>
    {
        private const int _CHUNK_SIZE = Chunk.STANDARD_SIZE;
        
        private readonly Vector3Int _position;

        public int X => _position.x;
        public int Y => _position.y;
        public int Z => _position.z;
        
        public ChunkPosition(Vector3Int position) 
        {
            _position = position;
        }

        public ChunkPosition(int x, int y, int z) : this(new(x, y, z)) { }

        public ChunkPosition OffsetCopy(int x, int y, int z) =>
            new(X + x, Y + y, Z + z);

        public Vector3Int ToWorld(Vector3Int localPosition) =>
            _position * _CHUNK_SIZE + localPosition;

        public Vector3Int ToWorld() =>
            ToWorld(Vector3Int.zero);

        public Vector3Int Difference(ChunkPosition other) =>
            Difference(this, other);

        public int Distance(ChunkPosition other) =>
            Distance(this, other);

        public static int Distance(ChunkPosition origin, ChunkPosition other)
        {
            Vector3Int difference = Difference(origin, other);

            return Math.Max(Math.Max(difference.x, difference.z), difference.y);
        }

        public static Vector3Int Difference(ChunkPosition origin, ChunkPosition other)
        {
            Vector3Int difference = (other - origin)._position;

            return new(Math.Abs(difference.x), Math.Abs(difference.z), Math.Abs(difference.y));
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
        
        public static ChunkPosition operator -(ChunkPosition left, Vector3Int right) =>
            new(left._position - right);

        public static ChunkPosition operator +(ChunkPosition left, ChunkPosition right) =>
            new(left._position + right._position);
        
        public static ChunkPosition operator +(ChunkPosition left, Vector3Int right) =>
            new(left._position + right);

        public static ChunkPosition operator *(ChunkPosition val, int scaler) =>
            new(val._position * scaler);
        
        public static ChunkPosition operator /(ChunkPosition val, int divider) =>
            new(val._position / divider);

        public bool Equals(ChunkPosition other) =>
            _position.Equals(other._position);

        public override bool Equals(object obj) =>
            obj is ChunkPosition other && Equals(other);

        public override int GetHashCode() =>
            _position.GetHashCode();

        public static bool operator ==(ChunkPosition left, ChunkPosition right) =>
            left.Equals(right);

        public static bool operator !=(ChunkPosition left, ChunkPosition right) =>
            !left.Equals(right);
    }
}
