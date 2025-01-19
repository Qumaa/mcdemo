using Project.World.Generation.Blocks;
using Project.World.Generation.Terrain;

namespace Project.World.Generation.Chunks
{
    public class BlocksIteratorProvider : IBlocksIteratorProvider
    {
        private const int _BLOCKS_PER_LOD_LEVEL_POOL_LIMIT = 1 << 24;

        private readonly BlocksIteratorPool _pool;
        private readonly BlocksGeneratingHelper _generatingHelper;

        public BlocksIteratorProvider(IBlockGenerator blockGenerator)
        {
            _pool = new(_BLOCKS_PER_LOD_LEVEL_POOL_LIMIT);
            _generatingHelper = new(blockGenerator);
        }

        public IBlocksIterator GetBlockIterator(ChunkPosition position, ChunkLOD lod,
            IBlocksIterator previous) =>
            PopulateIterator(position, GetIterator(lod, previous), previous);

        private IBlocksIterator GetIterator(ChunkLOD lod, IBlocksIterator previous) =>
            _pool.Get(lod, previous) ?? CreateIterator(lod);

        private static IBlocksIterator CreateIterator(ChunkLOD lod)
        {
            int size = lod.ChunkSize();
            return size > 1 ? new BlocksIterator(size) : new SingleBlockIterator();
        }

        private IBlocksIterator PopulateIterator(ChunkPosition position, IBlocksIterator iterator,
            IBlocksIterator source)
        {
            int size = iterator.Size;
            BlocksGeneratingHelper.Session blocksGenerator =
                _generatingHelper.StartGenerating(position, iterator, source);

            foreach (FlatIndexXYZ index in FlatIndexXYZ.Enumerate(size))
            {
                Block block = blocksGenerator.GetBlock(index.x, index.y, index.z);
                
                iterator[index.Flat] = block;
                UpdateIteratorEdgesData(iterator, index);
            }

            return iterator;
        }

        private void UpdateIteratorEdgesData(IBlocksIterator iterator, FlatIndexXYZ index)
        {
            int size = iterator.Size - 1;

            if (index.x is 0)
            {
                
            }
        }
    }
}
