using System;
using System.Diagnostics;
using System.Linq;

namespace Project.World
{
    public class BlockMesh
    {
        private readonly BlockFace[] _faces;
        
        public BlockMesh(params DirectionalBlockFace[] faces) 
        {
            _faces = UnpackFaceData(faces);
        }

        public BlockFace this[Direction direction] => _faces[FaceDirectionToIndex(direction)];

        public BlockFace Top => this[Direction.Up];
        public BlockFace Bottom => this[Direction.Down];
        public BlockFace Left => this[Direction.Left];
        public BlockFace Right => this[Direction.Right];
        public BlockFace Back => this[Direction.Back];
        public BlockFace Front => this[Direction.Forward];
        
        public BlockFace Opposite(Direction direction) => this[Negate(direction)];

        private static Direction Negate(Direction direction) =>
            direction switch
            {
                Direction.Up => Direction.Down,
                Direction.Down => Direction.Up,
                Direction.Left => Direction.Right,
                Direction.Right => Direction.Left,
                Direction.Forward => Direction.Back,
                Direction.Back => Direction.Forward,
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };

        private static BlockFace[] UnpackFaceData(DirectionalBlockFace[] faces)
        {
            Validate.Faces(faces);

            return faces.OrderBy(FaceDirectionToIndex).Select(x => x.Face).ToArray();
        }

        private static int FaceDirectionToIndex(Direction faceDirection) =>
            (int)faceDirection;

        private static int FaceDirectionToIndex(DirectionalBlockFace direction) =>
            FaceDirectionToIndex(direction.Direction);

        private static class Validate
        {
            private const int _FACES_CORRECT_NUMBER = 6;
            
            [Conditional(Project.ConditionalStrings.VALIDATION_METHODS)]
            public static void Faces(DirectionalBlockFace[] faces)
            {
                ValidateFacesNumber(faces);

                ValidateDirectionDuplicates(faces);
            }

            [Conditional(Project.ConditionalStrings.VALIDATION_METHODS)]
            private static void ValidateDirectionDuplicates(DirectionalBlockFace[] faceDirections)
            {
                Span<bool> facesValidationBuffer = stackalloc bool[_FACES_CORRECT_NUMBER];

                foreach (DirectionalBlockFace direction in faceDirections)
                {
                    int faceIndex = FaceDirectionToIndex(direction);

                    if (facesValidationBuffer[faceIndex])
                        Throw($"Duplicate {direction.Direction.ToString()} faces detected.");

                    facesValidationBuffer[faceIndex] = true;
                }
            }

            [Conditional(Project.ConditionalStrings.VALIDATION_METHODS)]
            private static void ValidateFacesNumber(DirectionalBlockFace[] faceDirections)
            {
                if (faceDirections.Length is not _FACES_CORRECT_NUMBER)
                    Throw(
                        $"Invalid number of faces: {faceDirections.Length}. Must be equal to {_FACES_CORRECT_NUMBER}"
                    );
            }

            [Conditional(Project.ConditionalStrings.VALIDATION_METHODS)]
            private static void Throw(string error) =>
                throw new ArgumentException($"Invalid face directions. {error}");
        }    
    }
}
