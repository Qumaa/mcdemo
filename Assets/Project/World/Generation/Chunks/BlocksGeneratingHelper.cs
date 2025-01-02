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

    public Session StartGenerating(Vector3Int position, IBlocksIterator iterator, IBlocksIterator copyFrom) =>
        new(position.x, position.y, position.z, iterator, copyFrom, _blockGenerator);

    public readonly ref struct Session
    {
        private const int _INVALID_VALUE = 0;

        private readonly IBlocksIterator _previous;
        private readonly IBlockGenerator _blockGenerator;
        private readonly int _x;
        private readonly int _y;
        private readonly int _z;
        private readonly int _ratioNewToOld; // positive = multiply; negative = divide;
        private readonly int _blockPositionScaler;

        public Session(int x, int y, int z, IBlocksIterator current, IBlocksIterator previous,
            IBlockGenerator blockGenerator) : this()
        {
            _x = x;
            _y = y;
            _z = z;
            _blockGenerator = blockGenerator;
            _blockPositionScaler = Chunk.STANDARD_SIZE / current.Size;

            if (previous is null)
            {
                _ratioNewToOld = _INVALID_VALUE;
                return;
            }

            _ratioNewToOld = CalculateRatio(current.Size, previous.Size);
            _previous = previous;
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

        private bool TryCopy(int x, int y, int z, out Block result)
        {
            result = default;

            switch (_ratioNewToOld)
            {
                // can't copy
                case _INVALID_VALUE:
                    return false;

                // equal
                case 1:
                    break;

                // new is larger
                case > 0 when x % _ratioNewToOld is not 0 || y % _ratioNewToOld is not 0 || z % _ratioNewToOld is not 0:
                    return false;

                case > 0:
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

            result = _previous[x, y, z];
            return true;
        }
    }
}
