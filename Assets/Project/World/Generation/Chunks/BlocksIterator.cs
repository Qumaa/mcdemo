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
            if (handle.TryGetNextIndex(faceDirection, out FlatIndexXYZ index))
                goto success;

            block = default;
            return false;
            
            success:
            block = this[index.Flat];
            return true;
        }
    }
}
