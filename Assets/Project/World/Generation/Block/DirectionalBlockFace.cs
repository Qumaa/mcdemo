namespace Project.World.Generation.Block
{
    public readonly struct DirectionalBlockFace
    {
        public readonly BlockFace Face;
        public readonly Direction Direction;
        
        public DirectionalBlockFace(BlockFace face, Direction direction)
        {
            Face = face;
            Direction = direction;
        }
    }
}
