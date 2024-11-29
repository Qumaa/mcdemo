using System;
using Project.World.Generation.Blocks;
using Project.World.Generation.Terrain;

namespace Project.World.Generation.Chunks
{
    public class BlockIteratorProvider : IBlockIteratorProvider
    {
        private readonly IBlockGenerator _blockGenerator;
        public BlockIteratorProvider(IBlockGenerator blockGenerator) {
            _blockGenerator = blockGenerator;
        }

        public IBlockIterator GetBlockIterator(ChunkLOD lod) =>
            new BlockIterator(GetDimensions(lod), GetDimensions(ChunkLOD.Full), _blockGenerator);

        private static int GetDimensions(ChunkLOD lod) =>
            lod switch
            {
                ChunkLOD.Full => 16,
                ChunkLOD.Half => 8,
                ChunkLOD.Quarter => 4,
                ChunkLOD.Eighth => 2,
                ChunkLOD.Sixteenth => 1,
                _ => throw new ArgumentOutOfRangeException(nameof(lod), lod, null)
            };
    }
}
