namespace Project.World.Generation.Blocks
{
    public interface IBlocksIteratorInitializer
    {
        IBlocksIterator Finish();
        void FillBlocks(int standardChunkSize, IBlocksIterator source);
    }

    public static class BlocksIteratorInitializerExtensions
    {
        public static void FillBlocks(this IBlocksIteratorInitializer initializer, int standardChunkSize)
        {
            initializer.FillBlocks(standardChunkSize, null);
        }
    }
}
