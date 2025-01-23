namespace Project.World
{
    public readonly ref struct FlatIndexXYZ
    {
        public readonly FlatIndex Flat;
        public readonly int x, y, z;
        
        public FlatIndexXYZ(FlatIndex flat, int x, int y, int z)
        {
            Flat = flat;
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public FlatIndexXYZ(FlatIndexHandle handle) : this(handle.FlatIndex, handle.x, handle.y, handle.z) { }
        public FlatIndexXYZ(int size, int x, int y, int z) : this(FlatIndex.FromXYZ(size, x, y, z), x, y, z) { }

        public static Enumerable Enumerate(int arraySize) =>
            new(arraySize);

        public readonly ref struct Enumerable
        {
            private readonly int _size;

            public Enumerable(int size)
            {
                _size = size;
            }

            public Enumerator GetEnumerator() =>
                new(_size);

            public ref struct Enumerator
            {
                private FlatIndexHandle.Enumerator _enumerator;

                public FlatIndexXYZ Current => new(_enumerator.Current);

                public Enumerator(int arraySize)
                {
                    _enumerator = new(arraySize);
                }

                public bool MoveNext() =>
                    _enumerator.MoveNext();
            }
        }
    }

    public static class FlatIndexXYZExtensions
    {
        public static ChunkPosition ToChunkPosition(this ref FlatIndexXYZ index) =>
            new(index.x, index.y, index.z);
    }
}
