using UnityEngine;

namespace Project.World
{
    public ref struct FlatIndexHandle
    {
        private readonly int _size, _size2, _size3;
        private int _x, _y, _z;
        private int _flatIndex;
        
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
            _size2 = _size * _size;
            _size3 = _size2 * _size;

            _x = _y = _z = _flatIndex = 0;
        }
        
        public int IncrementZ()
        {
            _z++;
            _flatIndex += _size2;
            return z;
        }
        public int DecrementZ()
        {
            _z--;
            _flatIndex -= _size2;
            return z;
        }
        public void ResetZ()
        {
            _z -= _size;
            _flatIndex -= _size3;
        }

        public int IncrementY()
        {
            _y++;
            _flatIndex += _size;
            return y;
        }
        public int DecrementY()
        {
            _y--;
            _flatIndex -= _size;
            return y;
        }
        public void ResetY()
        {
            _y -= _size;
            _flatIndex -= _size2;
        }

        public int IncrementX()
        {
            _x++;
            _flatIndex++;
            return x;
        }
        public int DecrementX()
        {
            _x--;
            _flatIndex--;
            return x;
        }
        public void ResetX()
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

            public Enumerator(int arraySize)
            {
                _handle = new(arraySize, 0, 0, -1);
            }

            public Enumerator(in FlatIndexHandle handle)
            {
                _handle = handle;
                Reset();
            }

            public bool MoveNext()
            {
                int size = _handle.Size;
                
                _handle.IncrementZ();
                if (_handle.z < size)
                    return true;

                _handle.ResetZ();

                _handle.IncrementY();
                if (_handle.y < size)
                    return true;

                _handle.ResetY();

                _handle.IncrementX();
                return _handle.x < size;
            }

            public void Reset()
            {
                _handle._x = _handle._y = 0;
                _handle._z = -1;
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
