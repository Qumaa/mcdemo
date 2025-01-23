using System.Runtime.InteropServices;
using UnityEngine;

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

        public Enumerable EnumerateVisibleFaces() =>
            new(this, true);
        public Enumerable EnumerateHiddenFaces() =>
            new(this, false);

        public static ChunkCullingFlags FromSignedDifference(Vector3Int signedDifference)
        {
            int x = signedDifference.x;
            int y = signedDifference.y;
            int z = signedDifference.z;

            return new(
                y <= 0,
                y >= 0,
                x <= 0,
                x >= 0,
                z <= 0,
                z >= 0
            );
        }

        public static ChunkCullingFlags FromSignedDifference(ChunkPosition center, ChunkPosition other) =>
            FromSignedDifference(center.SignedDifference(other));

        private static bool GetVisibility(int values, FaceDirection direction) =>
            GetVisibility(values, direction.ToInt());

        private static bool GetVisibility(int values, int direction) =>
            (values >> direction & 1) is 1;

        private static int GetMask(FaceDirection direction) =>
            1 << direction.ToInt();

        [StructLayout(LayoutKind.Auto)]
        public readonly ref struct Enumerable
        {
            private readonly ChunkCullingFlags _flags;
            private readonly bool _enumerateVisible;

            public Enumerable(ChunkCullingFlags flags, bool enumerateVisible)
            {
                _flags = flags;
                _enumerateVisible = enumerateVisible;
            }

            public Enumerator GetEnumerator() =>
                new(_flags._values, _enumerateVisible);

            [StructLayout(LayoutKind.Auto)]
            public ref struct Enumerator
            {
                private readonly int _values;
                private readonly bool _xorSwitch;
                private int _current;

                public FaceDirection Current => FaceDirections.Array[_current];

                public Enumerator(int values, bool enumeratePositiveFlags)
                {
                    _current = -1;
                    _values = values;
                    _xorSwitch = !enumeratePositiveFlags;
                }

                public bool MoveNext()
                {
                    while (++_current < FaceDirections.Array.Length)
                        if (_xorSwitch ^ GetVisibility(_values, _current))
                            return true;

                    return false;
                }
            }
        }
    }
}
