using System;
using Project.World.Generation.Chunks;
using UnityEngine;

namespace Project.World
{
    public readonly struct ChunkPosition : IEquatable<ChunkPosition>
    {
        private const int _CHUNK_SIZE = Chunk.STANDARD_SIZE;
        

        // ReSharper disable InconsistentNaming
        public readonly int x;
        public readonly int y;
        public readonly int z;
        // ReSharper restore InconsistentNaming
        
        public ChunkPosition(Vector3Int position) : this(position.x, position.y, position.z)
        {
        }

        public ChunkPosition(int x, int y, int z) : this()
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public ChunkPosition OffsetCopy(int x, int y, int z) =>
            new(this.x + x, this.y + y, this.z + z);

        public ChunkPosition OffsetCopy(int xyz) =>
            new(x + xyz, y + xyz, z + xyz);

        public Vector3Int ToWorld(Vector3Int localPosition) =>
            new Vector3Int(x, y, z) * _CHUNK_SIZE + localPosition;

        public Vector3Int ToWorld() =>
            ToWorld(Vector3Int.zero);

        public Vector3Int SignedDifference(ChunkPosition other) =>
            SignedDifference(this, other);

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
            Vector3Int difference = SignedDifference(origin, other);

            return new(Math.Abs(difference.x), Math.Abs(difference.z), Math.Abs(difference.y));
        }

        public static Vector3Int SignedDifference(ChunkPosition origin, ChunkPosition other)
        {
            return new(
                other.x - origin.x,
                other.y - origin.y,
                other.z - origin.z
                );
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

        public static ChunkPosition operator -(in ChunkPosition left, in ChunkPosition right) =>
            new(
                left.x - right.x,
                left.y - right.y,
                left.z - right.z
            );

        public static ChunkPosition operator -(in ChunkPosition left, Vector3Int right) =>
            new(
                left.x - right.x,
                left.y - right.y,
                left.z - right.z
            );

        public static ChunkPosition operator +(in ChunkPosition left, in ChunkPosition right) =>
            new(
                left.x + right.x,
                left.y + right.y,
                left.z + right.z
            );
        
        public static ChunkPosition operator +(in ChunkPosition left, Vector3Int right) =>
            new(
                left.x + right.x,
                left.y + right.y,
                left.z + right.z
            );

        public static ChunkPosition operator *(in ChunkPosition val, int scaler) =>
            new(
                val.x * scaler,
                val.y * scaler,
                val.z * scaler
            );
        
        public static ChunkPosition operator /(in ChunkPosition val, int divider) =>
            new(
                val.x / divider,
                val.y / divider,
                val.z / divider
            );

        public bool Equals(ChunkPosition other) =>
            x == other.x && y == other.y && z == other.z;

        public override bool Equals(object obj) =>
            obj is ChunkPosition other && Equals(other);

        public override int GetHashCode() =>
            HashCode.Combine(x, y, z);

        public static bool operator ==(ChunkPosition left, ChunkPosition right) =>
            left.Equals(right);

        public static bool operator !=(ChunkPosition left, ChunkPosition right) =>
            !left.Equals(right);
    }
}
