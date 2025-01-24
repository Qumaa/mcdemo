using System;

namespace Project.World
{
    public readonly struct ChunkHandle
    {
        public readonly DirectionFlags Flags;
        public readonly LODChunk Base;
        private readonly LODChunk _up;
        private readonly LODChunk _down;
        private readonly LODChunk _right;
        private readonly LODChunk _left;
        private readonly LODChunk _forward;
        private readonly LODChunk _back;

        private ChunkHandle(ref Mutable handle)
        {
            Flags = handle.Flags.Seal();
            Base = handle.Base;
            _up = handle.Up;
            _down = handle.Down;
            _right = handle.Right;
            _left = handle.Left;
            _forward = handle.Forward;
            _back = handle.Back;
        }

        public bool TryGetNext(FaceDirection direction, out LODChunk lodChunk) =>
            (lodChunk = direction switch
            {
                FaceDirection.Up => _up,
                FaceDirection.Down => _down,
                FaceDirection.Right => _right,
                FaceDirection.Left => _left,
                FaceDirection.Forward => _forward,
                FaceDirection.Back => _back,
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            }).Chunk is not null;

        public Enumerator GetEnumerator() =>
            new(in this);
        
        public static Mutable GetBuilder(in LODChunk lodChunk) =>
            new(in lodChunk);

        public struct Mutable
        {
            public DirectionFlags.Mutable Flags;
            public LODChunk Base;
            public LODChunk Up;
            public LODChunk Down;
            public LODChunk Right;
            public LODChunk Left;
            public LODChunk Forward;
            public LODChunk Back;

            public Mutable(in LODChunk lodChunk) : this()
            {
                Flags = new();
                Base = lodChunk;
            }

            public void Set(FaceDirection direction, in LODChunk lodChunk)
            {
                switch (direction)
                {
                    case FaceDirection.Up:
                        Up = lodChunk;
                        break;

                    case FaceDirection.Down:
                        Down = lodChunk;
                        break;

                    case FaceDirection.Right:
                        Right = lodChunk;
                        break;

                    case FaceDirection.Left:
                        Left = lodChunk;
                        break;

                    case FaceDirection.Forward:
                        Forward = lodChunk;
                        break;

                    case FaceDirection.Back:
                        Back = lodChunk;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                Flags.SetFlag(direction, true);
            }

            public ChunkHandle Seal() =>
                new(ref this);
        }

        public ref struct Enumerator
        {
            private readonly ChunkHandle _handle;
            private DirectionFlags.Enumerable.Enumerator _enumerator;
            public Enumerator(in ChunkHandle handle) 
            {
                _handle = handle;
                _enumerator = handle.Flags.EnumerateIncludedDirections().GetEnumerator();

                Current = default;
            }

            public Directional<LODChunk> Current { get; private set; }

            public bool MoveNext()
            {
                if (!_enumerator.MoveNext())
                    return false;

                FaceDirection direction = _enumerator.Current;
                _handle.TryGetNext(direction, out LODChunk lodChunk);
                Current = new(lodChunk, direction);

                return true;
            }
        }
    }
}
