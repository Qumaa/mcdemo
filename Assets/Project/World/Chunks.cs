using System;
using Project.World.Generation.Chunks;

namespace Project.World
{
    public class Chunks
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

        public void Set(ChunkPosition position, in LODChunk lodChunk)
        {
            FlatIndexXYZ index = WorldToLocal(position);

            SetDirect(index.Flat, lodChunk);
        }

        public void SetDirect(FlatIndex index, in LODChunk lodChunk) =>
            _chunks[index] = lodChunk;

        public FlatIndexHandle GetIndexHandle(in ChunkPosition position)
        {
            FlatIndexXYZ index = WorldToLocal(position);
            return new(Size, index.x, index.y, index.z);
        }

        public ChunkPosition IndexToWorld(in FlatIndexXYZ index) =>
            new(index.x - Extent, index.y - Extent, index.z - Extent);

        private FlatIndexXYZ WorldToLocal(ChunkPosition position)
        {
            position = position.OffsetCopy(Extent);
            return new(Size, position.x, position.y, position.z);
        }

        public LODChunk Get(in FlatIndexXYZ index) =>
            _chunks[index.Flat];

        public LODChunk GetDirect(FlatIndex index) =>
            _chunks[index];

        private static int LoadDistanceToWorldSize(int loadDistance) =>
            loadDistance > 0 ? loadDistance * 2 + 1 : 1;
    }
}
