using System;
using Project.World.Generation.Blocks;
using Project.World.Generation.Terrain;
using UnityEngine;

namespace Project.World.Generation.Chunks
{
    public class BlocksIterator : IBlocksIterator, IBlocksIteratorInitializer
    {
        private readonly Block[] _blocks;
        private readonly IBlockGenerator _blockGenerator;

        public int Size { get; }

        public Block this[int x, int y, int z]
        {
            get => _blocks[FlattenIndex(x, y, z)];
            private set => _blocks[FlattenIndex(x, y, z)] = value;
        }
        
        public BlocksIterator(int dimension, IBlockGenerator blockGenerator)
        {
            Size = dimension;
            _blockGenerator = blockGenerator;
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

        public void FillBlocks(int standardChunkSize)
        {
            FillBlocks(standardChunkSize, null);
            return;
            
            int blockPositionScaler = standardChunkSize / Size;
            
            for(var x = 0; x < Size; x++)
                for (var y = 0; y < Size; y++)
                    for (var z = 0; z < Size; z++)
                        _blocks[FlattenIndex(x, y, z)] = _blockGenerator.GenerateBlock(
                            x * blockPositionScaler,
                            y * blockPositionScaler,
                            z * blockPositionScaler
                        );
        }

        public IBlocksIterator Finish() =>
            this;

        public void FillBlocks(int standardChunkSize, IBlocksIterator source)
        {
            int blockPositionScaler = standardChunkSize / Size;
            CopyingHelper helper = source is null ? CopyingHelper.CanNeverCopy : new CopyingHelper(Size, source.Size);
            
            for (var x = 0; x < Size; x++)
            for (var y = 0; y < Size; y++)
            for (var z = 0; z < Size; z++)
                _blocks[FlattenIndex(x, y, z)] = helper.CanCopy(x, y, z, out Vector3Int sourceIndex) ?
                    source![sourceIndex.x, sourceIndex.y, sourceIndex.z] :
                    _blockGenerator.GenerateBlock(
                        x * blockPositionScaler,
                        y * blockPositionScaler,
                        z * blockPositionScaler
                    );
        }

        private int FlattenIndex(int x, int y, int z) =>
            x + (y * Size) + (z * Size * Size);

        private readonly ref struct CopyingHelper
        {
            private const int _INVALID_VALUE = 0;
            private readonly int _ratioNewToOld; // positive = multiply; negative = divide;

            public static CopyingHelper CanNeverCopy => new(_INVALID_VALUE);

            public CopyingHelper(int currentDimension, int copyFromDimension) : this(
                CalculateRatio(currentDimension, copyFromDimension)
            ) { }

            private static int CalculateRatio(int currentDimension, int copyFromDimension) =>
                currentDimension >= copyFromDimension ?
                    currentDimension / copyFromDimension :
                    -(copyFromDimension / currentDimension);

            private CopyingHelper(int ratioNewToOld)
            {
                _ratioNewToOld = ratioNewToOld;
            }

            public bool CanCopy(int x, int y, int z, out Vector3Int sourceIndex)
            {
                sourceIndex = new Vector3Int(x, y, z);
                
                switch (_ratioNewToOld)
                {
                    case _INVALID_VALUE:
                        return false;

                    // equal
                    case 1:
                        return true;

                    // new is larger
                    case > 0 when x % _ratioNewToOld is not 0 || y % _ratioNewToOld is not 0 || z % _ratioNewToOld is not 0:
                        return false;

                    case > 0:
                        sourceIndex /= _ratioNewToOld;
                        return true;

                    // old is larger
                    default:
                        sourceIndex *= -_ratioNewToOld;
                        return true;
                }
            }
        }
    }
}
