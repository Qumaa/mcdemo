namespace Project.World
{
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
}
