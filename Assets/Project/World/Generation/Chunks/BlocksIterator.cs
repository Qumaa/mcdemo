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

        public QueryResult<Block> QueryNextBlock(int x, int y, int z, FaceDirection faceDirection)
        {
            switch (faceDirection)
            {
                case FaceDirection.Up:
                    if ((y + 1) < Size)
                        return _Successful(this[x, y + 1, z]);

                    break;

                case FaceDirection.Down:
                    if ((y - 1) >= 0)
                        return _Successful(this[x, y - 1, z]);

                    break;

                case FaceDirection.Left:
                    if ((x - 1) >= 0)
                        return _Successful(this[x - 1, y, z]);

                    break;

                case FaceDirection.Right:
                    if ((x + 1) < Size)
                        return _Successful(this[x + 1, y, z]);

                    break;

                case FaceDirection.Forward:
                    if ((z + 1) < Size)
                        return _Successful(this[x, y, z + 1]);

                    break;

                case FaceDirection.Back:
                    if ((z - 1) >= 0)
                        return _Successful(this[x, y, z - 1]);

                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(faceDirection), faceDirection, null);
            }

            return _Failed();
            
            QueryResult<Block> _Failed() =>
                QueryResult<Block>.Failed;

            QueryResult<Block> _Successful(Block block) =>
                new(block, QueryStatus.Successful);
        }

        private int FlattenIndex(int x, int y, int z) =>
            x + (y * Size) + (z * Size * Size);
    }
}
