using System;
using Project.World.Generation.Blocks;

namespace Project.World.Generation.Chunks
{
    public class BlocksIterator : IBlocksIterator
    {
        private readonly Block[] _blocks;

        public int Size { get; }

        public Block this[FlatIndex index]
        {
            get => _blocks[index];
            set => _blocks[index] = value;
        }

        public BlocksIterator(int dimension)
        {
            Size = dimension;
            _blocks = new Block[dimension * dimension * dimension];
        }

        public bool TryGetNextBlock(FlatIndexHandle handle, FaceDirection faceDirection, out Block block)
        {
            switch (faceDirection)
            {
                case FaceDirection.Up:
                    if (handle.IncrementY() < Size)
                        goto success;
                    break;

                case FaceDirection.Down:
                    if (handle.DecrementY() >= 0)
                        goto success;
                    break;

                case FaceDirection.Left:
                    if (handle.DecrementX() >= 0)
                        goto success;
                    break;

                case FaceDirection.Right:
                    if (handle.IncrementX() < Size)
                        goto success;
                    break;

                case FaceDirection.Forward:
                    if (handle.IncrementZ() < Size)
                        goto success;
                    break;

                case FaceDirection.Back:
                    if (handle.DecrementZ() >= 0)
                        goto success;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(faceDirection), faceDirection, null);
            }

            block = default;
            return false;
            
            success:
            block = this[handle.FlatIndex];
            return true;
        }
    }
}
