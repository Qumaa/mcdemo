using Project.World.Generation.Blocks;
using Project.World.Generation.Terrain;
using UnityEngine;

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

        public IBlocksIterator GetBlockIterator(Vector3Int position, ChunkLOD lod, IBlocksIterator previous) =>
            PopulateIterator(position, GetIterator(lod, previous), previous);

        private IBlocksIterator GetIterator(ChunkLOD lod, IBlocksIterator previous) =>
            _pool.Get(lod, previous) ?? new BlocksIterator(lod.ChunkSize());

        private IBlocksIterator PopulateIterator(Vector3Int position, IBlocksIterator iterator, IBlocksIterator source)
        {
            int size = iterator.Size;
            BlocksGeneratingHelper.Session blocksGenerator =
                _generatingHelper.StartGenerating(position, iterator, source);

            for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
            for (int z = 0; z < size; z++)
                iterator[x, y, z] = blocksGenerator.GetBlock(x, y, z);

            return iterator;
        }
    }
}
