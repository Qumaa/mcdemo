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
            _pool.Get(lod, previous) ?? new BlocksIterator(lod.ChunkSize());

        private IBlocksIterator PopulateIterator(ChunkPosition position, IBlocksIterator iterator,
            IBlocksIterator source)
        {
            int size = iterator.Size;
            BlocksGeneratingHelper.Session blocksGenerator =
                _generatingHelper.StartGenerating(position, iterator, source);

            var enumerable = FlatIndexXYZ.Enumerate(size);
            var enumerator = enumerable.GetEnumerator();
            
            while(enumerator.MoveNext())
            // foreach (FlatIndexXYZ index in FlatIndexXYZ.Enumerate(size))
            {
                var index = enumerator.Current;
                iterator[index.Flat] = blocksGenerator.GetBlock(index.x, index.y, index.z);
            }
            return iterator;
        }
    }
}
