namespace Project.World
{
    public readonly struct Directional<T>
    {
        public readonly FaceDirection Direction;
        public readonly T Value;

        public Directional(T value, FaceDirection direction)
        {
            Direction = direction;
            Value = value;
        }
    }
}
