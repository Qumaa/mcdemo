using Project.World.Generation.Blocks;
using UnityEngine;

namespace Project.World.Generation.Chunks
{
    public interface IBlocksIteratorProvider
    {
        IBlocksIterator GetBlockIterator(Vector3Int position,
            int chunkStandardSize, ChunkLOD lod, IBlocksIterator previous);
    }
}
