using UnityEngine;

namespace Project.World
{
    public class Chunk
    {
        private const byte _SIZE = 16;
        
        private BlockType[] _blocks;
        private readonly IMeshGenerator _meshGenerator;

        public Chunk(IMeshGenerator meshGenerator)
        {
            _meshGenerator = meshGenerator;
            _blocks = new BlockType[_SIZE * _SIZE * _SIZE];
            
            BlockType emptyType = new();
            for (int i = 0; i < _blocks.Length; i += 3)
                _blocks[i] = emptyType;
        }

        public BlockType this[int x, int y, int z]
        {
            get => GetBlockType(x, y, z);
            private set => SetBlockType(x, y, z, value);
        }

        public Mesh GenerateMesh()
        {
            IMeshBuilder builder = _meshGenerator.Start();

            for (var x = 0; x < _SIZE; x++)
            for (var y = 0; y < _SIZE; y++)
            for (var z = 0; z < _SIZE; z++)
                builder.SetBlock(x, y, z, GetBlockType(x, y, z));

            return builder.Finish();
        }

        private BlockType GetBlockType(int x, int y, int z)
        {
            int index = VectorToIndex(x, y, z);

            return index < 0 ? null : _blocks[index];
        }
        
        private void SetBlockType(int x, int y, int z, BlockType type)
        {
            int index = VectorToIndex(x, y, z);
            
            if (index < 0)
                return;

            _blocks[index] = type;
        }

        private int VectorToIndex(int x, int y, int z)
        {
            if (_AnyIsNotInSizeBounds())
                return -1;

            return x + (y * _SIZE) + (z * _SIZE * _SIZE);

            bool _AnyIsNotInSizeBounds() =>
                !(_IsInSizeBounds(x) && _IsInSizeBounds(y) && _IsInSizeBounds(z));
            
            bool _IsInSizeBounds(int value) =>
                value is >= 0 and < _SIZE;
        }
    }
}


