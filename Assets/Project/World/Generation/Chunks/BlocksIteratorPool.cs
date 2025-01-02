using Project.World.Generation.Blocks;

namespace Project.World.Generation.Chunks
{
    // todo: position hash + lifo;
    public class BlocksIteratorPool
    {
        private readonly BlocksIteratorBucket[] _blocksIteratorPool;

        public BlocksIteratorPool(int blocksPerLODLevelPoolLimit)
        {
            _blocksIteratorPool = new BlocksIteratorBucket[ChunkLODs.Number];

            for (int i = 0; i < _blocksIteratorPool.Length; i++)
            {
                int chunkSize = ChunkLODs.FromInt(i).ChunkSize();
                int blocksInChunk = chunkSize * chunkSize * chunkSize;

                _blocksIteratorPool[i] = new(blocksPerLODLevelPoolLimit / blocksInChunk);
            }
        }

        public IBlocksIterator Get(ChunkLOD lod, IBlocksIterator previous) =>
            _blocksIteratorPool[lod.ToInt()].Get(previous);

        private struct BlocksIteratorBucket
        {
            private readonly IBlocksIterator[] _items;
            private int _count;
            private int _base;
            private readonly int _limit => _items.Length;

            public BlocksIteratorBucket(int size) : this()
            {
                _items = new IBlocksIterator[size];
                _count = 0;
                _base = 0;
            }

            public IBlocksIterator Get(IBlocksIterator previous)
            {
                if (_count <= 0)
                {
                    Insert(previous);
                    return null;
                }

                int index = GetCurrentIndex();
                IBlocksIterator item = _items[index];

                if (previous is null)
                    DecreaseCount();
                else
                    _items[index] = previous;

                return item;
            }

            private void Insert(IBlocksIterator iterator)
            {
                if (iterator is null)
                    return;

                _items[GetCurrentIndex()] = iterator;

                IncreaseCount();
            }

            private void IncreaseCount()
            {
                if (_count < (_limit - 1))
                {
                    _count++;
                    return;
                }

                _base++;
                _base %= _limit;
            }

            private void DecreaseCount()
            {
                if (_count > 0)
                    _count--;
            }

            private int GetCurrentIndex() =>
                (_base + _count) % _limit;
        }
    }
}
