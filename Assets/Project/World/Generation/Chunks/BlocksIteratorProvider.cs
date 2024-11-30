using System;
using Project.World.Generation.Blocks;
using Project.World.Generation.Terrain;
using UnityEngine;

namespace Project.World.Generation.Chunks
{
    public class BlocksIteratorProvider : IBlocksIteratorProvider
    {
        private const int _BLOCKS_PER_LOD_LEVEL_POOL_LIMIT = 1 << 24;

        private readonly IBlockGenerator _blockGenerator;
        private readonly BlocksIteratorBucket[] _blocksIteratorPool;

        public BlocksIteratorProvider(IBlockGenerator blockGenerator)
        {
            _blockGenerator = blockGenerator;
            _blocksIteratorPool = new BlocksIteratorBucket[Enum.GetNames(typeof(ChunkLOD)).Length];

            for (var i = 0; i < _blocksIteratorPool.Length; i++)
            {
                var lod = (ChunkLOD) i;
                int chunkSize = GetChunkSize(lod);
                
                _blocksIteratorPool[i] = new BlocksIteratorBucket(chunkSize);
            }
        }

        public IBlocksIterator GetBlockIterator(Vector3Int position, ChunkLOD lod, IBlocksIterator previous) =>
            GetFromPool(lod, previous) ?? CreateNew(lod, previous);

        // todo position hash + lifo;

        private IBlocksIterator CreateNew(ChunkLOD lod, IBlocksIterator previous)
        {
            IBlocksIteratorInitializer iterator = new BlocksIterator(GetChunkSize(lod), _blockGenerator);
            iterator.FillBlocks(GetChunkSize(ChunkLOD.Full), previous);

            return iterator.Finish();
        }

        private IBlocksIterator GetFromPool(ChunkLOD lod, IBlocksIterator previous) =>
            _blocksIteratorPool[LODLevelToInt(lod)].Get(previous);

        private static int LODLevelToInt(ChunkLOD lod) =>
            (int) lod;

        private static int GetChunkSize(ChunkLOD lod) =>
            lod switch
            {
                ChunkLOD.Full => 16,
                ChunkLOD.Half => 8,
                ChunkLOD.Quarter => 4,
                ChunkLOD.Eighth => 2,
                ChunkLOD.Sixteenth => 1,
                _ => throw new ArgumentOutOfRangeException(nameof(lod), lod, null)
            };

        private struct BlocksIteratorBucket
        {
            private readonly IBlocksIterator[] _items;
            private readonly int _size;
            private int _count;
            private int _base;
            private readonly int _limit => _items.Length;

            public BlocksIteratorBucket(int size) : this()
            {
                _size = size;
                _items = new IBlocksIterator[size * size * size];
                _count = 0;
                _base = 0;
            }

            public void Insert(IBlocksIterator iterator)
            {
                if (iterator is null)
                    return;

                _items[GetCurrentIndex()] = iterator;

                IncreaseCount();
            }

            public IBlocksIterator Get(IBlocksIterator previous)
            {
                if (_count <= 0)
                    return null;

                int index = GetCurrentIndex();

                var item = _items[index] as IBlocksIteratorInitializer;
                item.FillBlocks(_size, previous);
                _items[index] = previous;
                DecreaseCount();

                return item.Finish();
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
