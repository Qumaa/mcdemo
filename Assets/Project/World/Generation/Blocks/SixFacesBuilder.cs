using System;

namespace Project.World.Generation.Blocks
{
    public ref struct SixFacesBuilder<T>
    {
        private Directional<T> _top;
        private Directional<T> _bottom;
        private Directional<T> _left;
        private Directional<T> _right;
        private Directional<T> _back;
        private Directional<T> _front;

        public void AppendFace(Directional<T> face)
        {
            switch (face.Direction)
            {
                case FaceDirection.Up:
                    _top = face;
                    break;

                case FaceDirection.Down:
                    _bottom = face;
                    break;

                case FaceDirection.Right:
                    _right = face;
                    break;

                case FaceDirection.Left:
                    _left = face;
                    break;

                case FaceDirection.Forward:
                    _front = face;
                    break;

                case FaceDirection.Back:
                    _back = face;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public SixFaces<T> Build() =>
            new(_top, _bottom, _left, _right, _back, _front);
    }
}
