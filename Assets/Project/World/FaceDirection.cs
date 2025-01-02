using System;
using UnityEngine;

namespace Project.World
{
    public enum FaceDirection
    {
        Up = 0,
        Down = 1,
        Right = 2,
        Left = 3,
        Forward = 4,
        Back = 5
    }

    public readonly struct Directional<T>
    {
        public readonly FaceDirection Direction;
        public readonly T Face;

        public Directional(T face, FaceDirection direction)
        {
            Direction = direction;
            Face = face;
        }
    }

    [Serializable]
    public struct SerializableDirectional<T>
    {
        [field: SerializeField] public FaceDirection Direction { get; private set; }
        [field: SerializeField] public T Face { get; private set; }

        public SerializableDirectional(T face, FaceDirection direction)
        {
            Direction = direction;
            Face = face;
        }

        public static implicit operator Directional<T>(SerializableDirectional<T> directional) =>
            new(directional.Face, directional.Direction);
    }

    public static class FaceDirections
    {
        public static FaceDirection[] Array =
        {
            FaceDirection.Up,
            FaceDirection.Down,
            FaceDirection.Left,
            FaceDirection.Right,
            FaceDirection.Back,
            FaceDirection.Forward
        };

        public static int ToInt(this FaceDirection faceDirection) =>
            faceDirection switch
            {
                FaceDirection.Up => 0,
                FaceDirection.Down => 1,
                FaceDirection.Right => 2,
                FaceDirection.Left => 3,
                FaceDirection.Forward => 4,
                FaceDirection.Back => 5,
                _ => throw new ArgumentOutOfRangeException(nameof(faceDirection), faceDirection, null)
            };

        public static FaceDirection FromInt(int direction) =>
            direction switch
            {
                0 => FaceDirection.Up,
                1 => FaceDirection.Down,
                2 => FaceDirection.Right,
                3 => FaceDirection.Left,
                4 => FaceDirection.Forward,
                5 => FaceDirection.Back,
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };

        public static FaceDirection Negate(this FaceDirection faceDirection) =>
            faceDirection switch
            {
                FaceDirection.Up => FaceDirection.Down,
                FaceDirection.Down => FaceDirection.Up,
                FaceDirection.Left => FaceDirection.Right,
                FaceDirection.Right => FaceDirection.Left,
                FaceDirection.Forward => FaceDirection.Back,
                FaceDirection.Back => FaceDirection.Forward,
                _ => throw new ArgumentOutOfRangeException(nameof(faceDirection), faceDirection, null)
            };
    }
}
