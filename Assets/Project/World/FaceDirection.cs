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

    public static class FaceDirections
    {
        public static readonly FaceDirection[] Array =
        {
            FaceDirection.Up,
            FaceDirection.Down,
            FaceDirection.Right,
            FaceDirection.Left,
            FaceDirection.Forward,
            FaceDirection.Back
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
        
        public static Vector3Int ToVectorInt(this FaceDirection faceDirection) =>
            faceDirection switch
            {
                FaceDirection.Up => Vector3Int.up,
                FaceDirection.Down => Vector3Int.down,
                FaceDirection.Right => Vector3Int.right,
                FaceDirection.Left => Vector3Int.left,
                FaceDirection.Forward => Vector3Int.forward,
                FaceDirection.Back => Vector3Int.back,
                _ => throw new ArgumentOutOfRangeException(nameof(faceDirection), faceDirection, null)
            };
        
        public static Vector3Int ToVectorInt(this FaceDirection faceDirection, int length) =>
            faceDirection switch
            {
                FaceDirection.Up => new(0, length, 0),
                FaceDirection.Down => new(0, -length, 0),
                FaceDirection.Right => new(length, 0, 0),
                FaceDirection.Left => new(-length, 0, 0),
                FaceDirection.Forward => new(0, 0, length),
                FaceDirection.Back => new(0, 0, -length),
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

        public static FaceDirection Opposite(this FaceDirection faceDirection) =>
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

        public static void Negate(this ref FaceDirection direction) =>
            direction = direction.Opposite();
    }
}
