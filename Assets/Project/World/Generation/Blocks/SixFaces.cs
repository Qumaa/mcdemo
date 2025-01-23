using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Project.World.Generation.Blocks
{
    public class SixFaces<T>
    {
        protected readonly T[] _faces;

        public T this[FaceDirection faceDirection] => _faces[faceDirection.ToInt()];

        public T Top => this[FaceDirection.Up];
        public T Bottom => this[FaceDirection.Down];
        public T Left => this[FaceDirection.Left];
        public T Right => this[FaceDirection.Right];
        public T Back => this[FaceDirection.Back];
        public T Front => this[FaceDirection.Forward];

        public SixFaces(Directional<T> face1, Directional<T> face2, Directional<T> face3, Directional<T> face4,
            Directional<T> face5, Directional<T> face6) : this()
        {
            Validate.Faces(face1, face2, face3, face4, face5, face6);
            
            AppendFace(face1);
            AppendFace(face2);
            AppendFace(face3);
            AppendFace(face4);
            AppendFace(face5);
            AppendFace(face6);
        }

        public SixFaces()
        {
            _faces = new T[6];
        }

        private SixFaces(Directional<T>[] faces) : this()
        {
            foreach (Directional<T> face in faces)
                AppendFace(face);
        }

        public void AppendFace(Directional<T> face) =>
            _faces[face.Direction.ToInt()] = face.Face;

        public T Opposite(FaceDirection faceDirection) =>
            this[faceDirection.Opposite()];

        public Enumerator GetEnumerator() =>
            new(this);

        public static SixFaces<T> FromDirectional(params Directional<T>[] faces)
        {
            Validate.Faces(faces);

            return new(faces);
        }

        public struct Enumerator
        {
            private readonly SixFaces<T> _parent;
            private int _index;
            
            public Enumerator(SixFaces<T> parent) 
            {
                _parent = parent;
                _index = -1;
            }

            public Directional<T> Current
            {
                get
                {
                    FaceDirection direction = FaceDirections.FromInt(_index);
                    return new(_parent[direction], direction);
                }
            }

            public bool MoveNext() =>
                ++_index < FaceDirections.Array.Length;
        } 

        private static class Validate
        {
            private const int _FACES_CORRECT_NUMBER = 6;

            [Conditional(Project.ConditionalStrings.VALIDATION_METHODS)]
            public static void Faces(params Directional<T>[] faces)
            {
                ValidateFacesNumber(faces);

                ValidateDirectionDuplicates(faces);
            }

            [Conditional(Project.ConditionalStrings.VALIDATION_METHODS)]
            private static void ValidateDirectionDuplicates(Directional<T>[] faceData)
            {
                Span<bool> facesValidationBuffer = stackalloc bool[_FACES_CORRECT_NUMBER];

                foreach (Directional<T> data in faceData)
                {
                    FaceDirection direction = data.Direction;
                    int faceIndex = direction.ToInt();

                    if (facesValidationBuffer[faceIndex])
                        Throw($"Duplicate {direction.ToString()} faces detected.");

                    facesValidationBuffer[faceIndex] = true;
                }
            }

            [Conditional(Project.ConditionalStrings.VALIDATION_METHODS)]
            private static void ValidateFacesNumber(Directional<T>[] faceDirections)
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

    public static class SixFaces
    {
        public static SixFaces<T> Empty<T>() where T : new() =>
            new(
                new(new(), FaceDirection.Up),
                new(new(), FaceDirection.Down),
                new(new(), FaceDirection.Right),
                new(new(), FaceDirection.Left),
                new(new(), FaceDirection.Forward),
                new(new(), FaceDirection.Back)
            );
    }
}
