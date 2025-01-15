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

        public FlatIndexXYZ(in FlatIndexHandle handle) : this(handle.FlatIndex, handle.x, handle.y, handle.z) { }

        public static implicit operator FlatIndex(FlatIndexXYZ xyz) =>
            xyz.Flat;

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
}
