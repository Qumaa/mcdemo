using System;
using Project.World.Generation.Blocks;

namespace Project.World.Generation.Chunks
{
    public class BlocksIterator : IBlocksIterator
    {
        private readonly Block[] _blocks;

        public int Size { get; }

        public Block this[int x, int y, int z]
        {
            get => _blocks[FlattenIndex(x, y, z)];
            set => _blocks[FlattenIndex(x, y, z)] = value;
        }

        public BlocksIterator(int dimension)
        {
            Size = dimension;
            _blocks = new Block[dimension * dimension * dimension];
        }

        public bool TryGetNextBlock(int x, int y, int z, FaceDirection faceDirection, out Block block)
        {
            switch (faceDirection)
            {
                case FaceDirection.Up:
                    if (++y < Size)
                    {
                        block = this[x, y, z];
                        return true;
                    }
                    break;

                case FaceDirection.Down:
                    if (--y >= 0)
                    {
                        block = this[x, y, z];
                        return true;
                    }
                    break;

                case FaceDirection.Left:
                    if (--x >= 0)
                    {
                        block = this[x, y, z];
                        return true;
                    }
                    break;

                case FaceDirection.Right:
                    if (++x < Size)
                    {
                        block = this[x, y, z];
                        return true;
                    }
                    break;

                case FaceDirection.Forward:
                    if (++z < Size)
                    {
                        block = this[x, y, z];
                        return true;
                    }
                    break;

                case FaceDirection.Back:
                    if (--z >= 0)
                    {
                        block = this[x, y, z];
                        return true;
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(faceDirection), faceDirection, null);
            }

            block = default;
            return false;
        }

        private int FlattenIndex(int x, int y, int z) =>
            x + (y * Size) + (z * Size * Size);
    }
}
