using System;
using Project.World.Generation.Chunks;

namespace Project.World
{
    public class Chunks : IChunksIterator
    {
        private readonly LODChunk[] _chunks;

        public int Extent { get; private set; }
        public LODChunk[] Values => _chunks;
        public int Size { get; }

        public ChunkPosition Center { get; private set; }

        public Chunks(int initialExtent, ChunkPosition center)
        {
            Extent = initialExtent;
            Center = center;
            Size = LoadDistanceToWorldSize(initialExtent);
            _chunks = new LODChunk[Size * Size * Size];
        }

        public bool TryGetNextChunk(ChunkPosition position, FaceDirection direction, out IChunk chunk)
        {
            FlatIndexHandle handle = GetIndexHandle(position);
            
            if (handle.TryGetNextIndex(direction, out FlatIndexXYZ index))
                goto success;

            chunk = default;
            return false;
            
            success:
            chunk = _chunks[index.Flat].Chunk;
            return true;
        }

        public void Set(ChunkPosition position, LODChunk lodChunk)
        {
            position = WorldToIndex(position);
            FlatIndex index = FlatIndex.FromXYZ(Size, position.x, position.y, position.z);

            SetDirect(index, lodChunk);
        }

        public void SetDirect(FlatIndex index, LODChunk lodChunk) =>
            _chunks[index] = lodChunk;

        public FlatIndexHandle GetIndexHandle(ChunkPosition position)
        {
            position = WorldToIndex(position);
            return new(Size, position.x, position.y, position.z);
        }

        public ChunkPosition IndexToWorld(in FlatIndexXYZ index) =>
            new(index.x - Extent, index.y - Extent, index.z - Extent);

        private ChunkPosition WorldToIndex(ChunkPosition position) =>
            position.OffsetCopy(Extent);

        public static int LoadDistanceToWorldSize(int loadDistance) =>
            loadDistance > 0 ? loadDistance * 2 + 1 : 1;
    }
}
