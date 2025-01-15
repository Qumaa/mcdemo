using UnityEngine;

namespace Project.World
{
    public ref struct FlatIndexHandle
    {
        private int _flatIndex;
        private int _x;
        private int _y;
        private int _z;
        private int _size;
        
        // ReSharper disable InconsistentNaming
        // ReSharper disable ConvertToAutoProperty
        public int Size => _size;
        public FlatIndex FlatIndex => _flatIndex;
        public int x => _x;
        public int y => _y;
        public int z => _z;
        // ReSharper restore ConvertToAutoProperty
        // ReSharper restore InconsistentNaming

        public FlatIndexHandle(int arraySize, int x, int y, int z) : this(arraySize)
        {
            _x = x;
            _y = y;
            _z = z;

            _flatIndex = FlatIndex.FromXYZ(arraySize, x, y, z);
        }

        public FlatIndexHandle(int arraySize)
        {
            _size = arraySize;

            _x = _y = _z = 0;
            _flatIndex = 0;
        }
        
        public bool TryIncrementZ()
        {
            _z++;
            _flatIndex += _size * _size;
            return _z < _size;
        }


        public bool TryDecrementZ()
        {
            _z--;
            _flatIndex -= _size * _size;
            return _z >= 0;
        }

        public bool TryIncrementY()
        {
            _y++;
            _flatIndex += _size;
            return _y < _size;
        }

        public bool TryDecrementY()
        {
            _y--;
            _flatIndex -= _size;
            return _y >= 0;
        }

        private void ResetY()
        {
            _y -= _size;
            _flatIndex -= _size * _size;
        }

        public bool TryIncrementX()
        {
            _x++;
            _flatIndex++;
            return _x < _size;
        }
        public bool TryDecrementX()
        {
            _x--;
            _flatIndex--;
            return _x >= 0;
        }

        private void ResetX()
        {
            _x -= _size;
            _flatIndex -= _size;
        }
        
        public Enumerator GetEnumerator() =>
            new(_size);

        public static FlatIndexHandle Enumerate(int arraySize) =>
            new(arraySize);
        
        public ref struct Enumerator
        {
            private FlatIndexHandle _handle;

            public FlatIndexHandle Current => _handle;

            public Enumerator(int arraySize) : this(new FlatIndexHandle(arraySize)) { }

            public Enumerator(in FlatIndexHandle handle)
            {
                _handle = handle;
                ResetHandle();
            }

            public bool MoveNext()
            {
                if (_handle.TryIncrementX())
                    return true;
                _handle.ResetX();

                if (_handle.TryIncrementY())
                    return true;
                _handle.ResetY();

                return _handle.TryIncrementZ();
            }

            private void ResetHandle()
            {
                _handle._z = _handle._y  = 0;
                _handle._x = _handle._flatIndex = -1;
            }
        }
    }

    public static class FlatIndexHandleExtensions
    {
        public static Vector3Int ToVectorInt(this FlatIndexHandle handle) =>
            new(handle.x, handle.y, handle.z);

        public static Vector3 ToVector(this FlatIndexHandle handle) =>
            new(handle.x, handle.y, handle.z);
    }
}
