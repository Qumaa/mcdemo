namespace Project.World
{
    public readonly struct ChunkCullingFlags
    {
        private static readonly int _maskTop = GetMask(FaceDirection.Up);
        private static readonly int _maskBottom = GetMask(FaceDirection.Down);
        private static readonly int _maskRight = GetMask(FaceDirection.Right);
        private static readonly int _maskLeft = GetMask(FaceDirection.Left);
        private static readonly int _maskForward = GetMask(FaceDirection.Forward);
        private static readonly int _maskBack = GetMask(FaceDirection.Back);
        private static readonly int _maskFull =
            _maskTop | _maskBottom | _maskRight | _maskLeft | _maskForward | _maskBack;
        
        private readonly int _values;

        public static ChunkCullingFlags Full => new(_maskFull);

        public bool this[FaceDirection direction] => GetVisibility(_values, direction);

        public ChunkCullingFlags(bool top, bool bottom, bool right, bool left, bool forward, bool back)
        {
            _values = 0;

            if (top)
                _values |= _maskTop;

            if (bottom)
                _values |= _maskBottom;

            if (right)
                _values |= _maskRight;

            if (left)
                _values |= _maskLeft;

            if (forward)
                _values |= _maskForward;

            if (back)
                _values |= _maskBack;
        }
        
        private ChunkCullingFlags(int values) {
            _values = values;
        }

        public Enumerator GetEnumerator() =>
            new(_values);

        private static bool GetVisibility(int values, FaceDirection direction) =>
            GetVisibility(values, direction.ToInt());
        private static bool GetVisibility(int values, int direction) =>
            (values >> direction & 1) is 1;

        private static int GetMask(FaceDirection direction) =>
            1 << direction.ToInt();

        public ref struct Enumerator
        {
            private int _current;
            private int _values;
            
            public FaceDirection Current => FaceDirections.Array[_current];

            public Enumerator(int values)
            {
                _current = -1;
                _values = values;
            }

            public bool MoveNext()
            {
                while (++_current < FaceDirections.Array.Length)
                    if (GetVisibility(_values, Current))
                        return true;

                return false;
            }
        }
    }
}
