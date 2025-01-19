using System;
using Project.World.Generation.Chunks;

namespace Project.World
{
    public class Chunks : IChunksIterator
    {
        private readonly LODChunk[] _chunks;
        private int _size;

        public int Extent { get; private set; }
        public LODChunk[] Values => _chunks;
        public ChunkPosition Center { get; private set; }

        public Chunks(int initialExtent, ChunkPosition center)
        {
            Extent = initialExtent;
            Center = center;
            _size = GetSize();
            _chunks = new LODChunk[_size * _size * _size];
        }

        public bool TryGetNextChunk(ChunkPosition position, FaceDirection direction, out IChunk chunk)
        {
            position = WorldToIndex(position);
            int x = position.x;
            int y = position.y;
            int z = position.z;

            switch (direction)
            {
                case FaceDirection.Up:
                    if (++y < _size)
                        goto success;
                    break;

                case FaceDirection.Down:
                    if (--y >= 0)
                        goto success;
                    break;

                case FaceDirection.Left:
                    if (--x >= 0)
                        goto success;
                    break;

                case FaceDirection.Right:
                    if (++x < _size)
                        goto success;
                    break;

                case FaceDirection.Forward:
                    if (++z < _size)
                        goto success;
                    break;

                case FaceDirection.Back:
                    if (--z >= 0)
                        goto success;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }

            chunk = default;
            return false;
            
            success:
            chunk = _chunks[FlatIndex.FromXYZ(_size, x, y, z)].Chunk;
            return true;
        }

        public int GetSize() =>
            LoadDistanceToWorldSize(Extent);

        private ChunkPosition WorldToIndex(ChunkPosition position) =>
            position.OffsetCopy(Extent);

        public static int LoadDistanceToWorldSize(int loadDistance) =>
            loadDistance > 0 ? loadDistance * 2 + 1 : 1;

        public void Set(ChunkPosition position, LODChunk lodChunk)
        {
            position = WorldToIndex(position);
            FlatIndex index = FlatIndex.FromXYZ(_size, position.x, position.y, position.z);

            SetDirect(index, lodChunk);
        }

        public void SetDirect(FlatIndex index, LODChunk lodChunk) =>
            _chunks[index] = lodChunk;
    }
}
