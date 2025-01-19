using Project.World.Generation.Blocks;

namespace Project.World.Generation.Chunks
{
    public class SingleBlockIterator : IBlocksIterator
    {
        private Block _block;
        public int Size => 1;


        public Block this[FlatIndex index]
        {
            get => _block;
            set => _block = value;
        }
        
        public bool TryGetNextBlock(FlatIndexHandle handle, FaceDirection faceDirection, out Block block)
        {
            block = default;
            return false;
        }
    }
}
