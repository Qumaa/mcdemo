using System;

namespace Project.World
{
    public struct ChunkHandle
    {
        public LODChunk Base;
        public LODChunk Up;
        public LODChunk Down;
        public LODChunk Right;
        public LODChunk Left;
        public LODChunk Forward;
        public LODChunk Back;
        
        public ChunkHandle(LODChunk lodChunk) : this()
        {
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
        }

        public RefReady Seal() =>
            new(ref this);

        public readonly struct RefReady
        {
            public readonly LODChunk Base;
            private readonly LODChunk _up;
            private readonly LODChunk _down;
            private readonly LODChunk _right;
            private readonly LODChunk _left;
            private readonly LODChunk _forward;
            private readonly LODChunk _back;

            public RefReady(ref ChunkHandle handle)
            {
                Base = handle.Base;
                _up = handle.Up;
                _down = handle.Down;
                _right = handle.Right;
                _left = handle.Left;
                _forward = handle.Forward;
                _back = handle.Back;
            }

            public bool TryGet(FaceDirection direction, out LODChunk lodChunk) =>
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
        }
    }
}
