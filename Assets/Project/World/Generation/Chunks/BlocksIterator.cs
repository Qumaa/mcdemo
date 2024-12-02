using System;
using Project.World.Generation.Blocks;
using UnityEngine;

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

        public bool TryGetNext(int x, int y, int z, Direction direction, out Block block)
        {
            switch (direction)
            {
                case Direction.Up:
                    if (y + 1 < Size)
                    {
                        block = this[x, y + 1, z];
                        return true;
                    }
                    break;
                
                case Direction.Down:
                    if (y - 1 >= 0)
                    {
                        block = this[x, y - 1, z];
                        return true;
                    }
                    break;
                
                case Direction.Left:
                    if (x - 1 >= 0)
                    {
                        block = this[x - 1, y, z];
                        return true;
                    }
                    break;
                
                case Direction.Right:
                    if (x + 1 < Size)
                    {
                        block = this[x + 1, y, z];
                        return true;
                    }
                    break;
                
                case Direction.Forward:
                    if (z + 1 < Size)
                    {
                        block = this[x, y, z + 1];
                        return true;
                    }
                    break;
                
                case Direction.Back:
                    if (z - 1 >= 0)
                    {
                        block = this[x, y, z - 1];
                        return true;
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }

            block = default;
            return false;
        }

        private int FlattenIndex(int x, int y, int z) =>
            x + (y * Size) + (z * Size * Size);
    }
}
