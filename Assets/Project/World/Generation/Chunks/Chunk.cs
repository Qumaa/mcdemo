using Project.World.Generation.Blocks;
using Project.World.Generation.Terrain;
using UnityEngine;

namespace Project.World.Generation.Chunks
{
    public class Chunk : IBlockIterator
    {
        private const byte _SIZE = 16;
        
        private readonly Block[,,] _blocks;
        private readonly IMeshGenerator _meshGenerator;
        private readonly IBlockMeshProvider _blockMeshProvider;
        private readonly IBlockGenerator _blockGenerator;

        public Chunk(IMeshGenerator meshGenerator, IBlockGenerator blockGenerator)
        {
            _meshGenerator = meshGenerator;
            _blockGenerator = blockGenerator;
            _blocks = new Block[_SIZE, _SIZE, _SIZE];
            
            for (var x = 0; x < _SIZE; x++)
            for (var y = 0; y < _SIZE; y++)
            for (var z = 0; z < _SIZE; z++)
                _blocks[x,y,z] = _blockGenerator.GetBlock(x, y, z);
        }

        public BlockType this[int x, int y, int z] => GetBlockType(x, y, z);

        public Mesh GenerateMesh()
        {
            IMeshBuilder builder = _meshGenerator.Start(this);

            for (var x = 0; x < _SIZE; x++)
            for (var y = 0; y < _SIZE; y++)
            for (var z = 0; z < _SIZE; z++)
                builder.SetBlock(x, y, z, GetBlockType(x, y, z));

            return builder.Finish();
        }

        private BlockType GetBlockType(int x, int y, int z) =>
            TryGetBlock(x, y, z, out Block block) ? block.Type : null;

        private bool TryGetBlock(int x, int y, int z, out Block block)
        {
            if (_AnyIsNotInSizeBounds())
            {
                block = default;
                return false;
            }
            
            block = _blocks[x, y, z];
            return true;
            
            bool _AnyIsNotInSizeBounds() =>
                !(_IsInSizeBounds(x) && _IsInSizeBounds(y) && _IsInSizeBounds(z));
            
            bool _IsInSizeBounds(int value) =>
                value is >= 0 and < _SIZE;
        }
    }
}


