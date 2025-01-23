using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Project.World
{
    public readonly struct DirectionFlags
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

        public static DirectionFlags All => new(_maskFull);

        public bool this[FaceDirection direction] => GetFlag(_values, direction);

        public DirectionFlags(bool top, bool bottom, bool right, bool left, bool forward, bool back)
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
        
        private DirectionFlags(int values) {
            _values = values;
        }

        public Mutable MutableCopy() =>
            new(_values);

        public DirectionFlags Combine(in DirectionFlags other, FlagsCombination combination)
        {
            int values = combination switch
            {
                FlagsCombination.OR => _values | other._values,
                FlagsCombination.NOR => ~(_values | other._values),
                FlagsCombination.AND => _values & other._values,
                FlagsCombination.NAND => ~(_values & other._values),
                FlagsCombination.XOR => _values ^ other._values,
                FlagsCombination.NXOR => ~(_values ^ other._values),
                _ => throw new ArgumentOutOfRangeException(nameof(combination), combination, null)
            };

            return new(values);
        }

        public Enumerable EnumerateIncludedDirections() =>
            new(this, true);
        public Enumerable EnumerateExcludedFaces() =>
            new(this, false);

        public static DirectionFlags FromSignedDifference(Vector3Int signedDifference)
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

        public static DirectionFlags FromSignedDifference(ChunkPosition center, ChunkPosition other) =>
            FromSignedDifference(center.SignedDifference(other));

        private static bool GetFlag(int values, FaceDirection direction) =>
            GetFlag(values, direction.ToInt());

        private static bool GetFlag(int values, int direction) =>
            (values >> direction & 1) is 1;

        private static int GetMask(FaceDirection direction) =>
            GetMask(direction.ToInt());
        
        private static int GetMask(int direction) =>
            1 << direction;

        public struct Mutable
        {
            private int _values;
            
            public Mutable(int values) 
            {
                _values = values;
            }

            public DirectionFlags Finish() =>
                new(_values);

            public bool GetFlag(FaceDirection direction) =>
                DirectionFlags.GetFlag(_values, direction);

            public void SetFlag(FaceDirection direction, bool value) =>
                SetFlag(direction.ToInt(), value);

            private void SetFlag(int direction, bool value)
            {
                if (value)
                    _values |= GetMask(direction);
                else
                    _values &= ~GetMask(direction);
            }
        }

        [StructLayout(LayoutKind.Auto)]
        public readonly ref struct Enumerable
        {
            private readonly DirectionFlags _flags;
            private readonly bool _enumerateVisible;

            public Enumerable(DirectionFlags flags, bool enumerateVisible)
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
                        if (_xorSwitch ^ GetFlag(_values, _current))
                            return true;

                    return false;
                }
            }
        }
    }

    public enum FlagsCombination
    {
        OR,
        NOR,
        AND,
        NAND,
        XOR,
        NXOR
    }
}
