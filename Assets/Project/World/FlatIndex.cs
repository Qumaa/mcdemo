namespace Project.World
{
    public readonly ref struct FlatIndex
    {
        public readonly int Value;
        
        public FlatIndex(int value) 
        {
            Value = value;
        }
        
        public static FlatIndex FromXYZ(int size, int x, int y, int z) =>
            x + (y * size) + (z * size * size);

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

                public FlatIndex Current => _enumerator.Current.FlatIndex;

                public Enumerator(int arraySize)
                {
                    _enumerator = new(arraySize);
                }

                public bool MoveNext() =>
                    _enumerator.MoveNext();

                public void Reset() =>
                    _enumerator.Reset();
            }
        }

        public static implicit operator int(FlatIndex index) =>
            index.Value;

        public static implicit operator FlatIndex(int value) =>
            new(value);
    }
}
