using Project.World;
using Project.World.Generation.Blocks;
using Project.World.Generation.Chunks;
using Project.World.Generation.Terrain;
using UnityEngine;

public class BlocksGeneratingHelper
{
    private readonly IBlockGenerator _blockGenerator;

    public BlocksGeneratingHelper(IBlockGenerator blockGenerator)
    {
        _blockGenerator = blockGenerator;
    }

    public Session StartGenerating(ChunkPosition position, IBlocksIterator iterator, 
        IBlocksIterator copyFrom) =>
        new(position, iterator, copyFrom, _blockGenerator);

    public readonly ref struct Session
    {
        private const int _CHUNK_SIZE = Chunk.STANDARD_SIZE;
        private const int _INVALID_VALUE = 0;

        private readonly IBlocksIterator _copyFrom;
        private readonly IBlockGenerator _blockGenerator;
        private readonly int _x;
        private readonly int _y;
        private readonly int _z;
        private readonly int _ratioNewToOld; // positive = multiply; negative = divide;
        private readonly int _blockPositionScaler;

        public Session(ChunkPosition position, IBlocksIterator current, IBlocksIterator previous,
            IBlockGenerator blockGenerator) : this()
        {
            Vector3Int worldPosition = position.ToWorld();
            _x = worldPosition.x;
            _y = worldPosition.y;
            _z = worldPosition.z;
            _blockGenerator = blockGenerator;
            _blockPositionScaler = _CHUNK_SIZE / current.Size;

            if (previous is null)
            {
                _ratioNewToOld = _INVALID_VALUE;
                return;
            }

            _ratioNewToOld = CalculateRatio(current.Size, previous.Size);
            _copyFrom = previous;
        }

        private static int CalculateRatio(int currentDimension, int copyFromDimension) =>
            currentDimension >= copyFromDimension ?
                currentDimension / copyFromDimension :
                -(copyFromDimension / currentDimension);

        public Block GetBlock(int x, int y, int z) =>
            TryCopy(x, y, z, out Block result) ? result : GenerateBlock(x, y, z);

        private Block GenerateBlock(int x, int y, int z) =>
            _blockGenerator.GenerateBlock(
                _x + (x * _blockPositionScaler),
                _y + (y * _blockPositionScaler),
                _z + (z * _blockPositionScaler)
            );

        private bool TryCopy(int x, int y, int z, out Block copied)
        {
            // copying from one 3d array to another with varying size
            // if source array is smaller, all values are copied but "padded" 
            // if source array is larger, only a fraction of values is copied
            copied = default;

            switch (_ratioNewToOld)
            {
                // can't copy
                case _INVALID_VALUE:
                    return false;

                // equal
                case 1:
                    break;

                // new is larger
                case > 0:
                    if (CanNotCopyFromPrevious(x, y, z))
                        return false;
                    
                    x /= _ratioNewToOld;
                    y /= _ratioNewToOld;
                    z /= _ratioNewToOld;
                    break;

                // old is larger
                default:
                    x *= -_ratioNewToOld;
                    y *= -_ratioNewToOld;
                    z *= -_ratioNewToOld;
                    break;
            }

            copied = _copyFrom[FlatIndex.FromXYZ(_copyFrom.Size, x, y, z)];
            return true;
        }

        private bool CanNotCopyFromPrevious(int x, int y, int z) =>
            x % _ratioNewToOld is not 0 || y % _ratioNewToOld is not 0 || z % _ratioNewToOld is not 0;
    }
}
