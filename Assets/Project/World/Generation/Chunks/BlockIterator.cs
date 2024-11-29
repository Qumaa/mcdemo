using System;
using Project.World.Generation.Blocks;
using Project.World.Generation.Terrain;

namespace Project.World.Generation.Chunks
{
    public class BlockIterator : IBlockIterator
    {
        private readonly Block[] _blocks;
        private readonly IBlockGenerator _blockGenerator;

        public int Dimensions { get; }

        public Block this[int x, int y, int z] => _blocks[FlattenIndex(x, y, z)];

        public BlockIterator(int dimensions, int standardDimensions, IBlockGenerator blockGenerator)
        {
            Dimensions = dimensions;
            _blockGenerator = blockGenerator;
            _blocks = new Block[dimensions * dimensions * dimensions];
            
            FillBlocks(standardDimensions);
        }

        public bool TryGetNext(int x, int y, int z, Direction direction, out Block block)
        {
            switch (direction)
            {
                case Direction.Up:
                    if (y + 1 < Dimensions)
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
                    if (x + 1 < Dimensions)
                    {
                        block = this[x + 1, y, z];
                        return true;
                    }
                    break;
                
                case Direction.Forward:
                    if (z + 1 < Dimensions)
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

        private void FillBlocks(int standardDimensions)
        {
            int blockPositionScaler = standardDimensions / Dimensions;
            
            for(var x = 0; x < Dimensions; x++)
                for (var y = 0; y < Dimensions; y++)
                    for (var z = 0; z < Dimensions; z++)
                        _blocks[FlattenIndex(x, y, z)] = _blockGenerator.GenerateBlock(
                            x * blockPositionScaler,
                            y * blockPositionScaler,
                            z * blockPositionScaler
                        );
        }

        private int FlattenIndex(int x, int y, int z) =>
            x + (y * Dimensions) + (z * Dimensions * Dimensions);
    }
}
