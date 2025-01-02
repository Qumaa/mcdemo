namespace Project.World.Generation.Blocks
{
    public struct Block
    {
        private readonly BlockType _type;

        public BlockType Type => _type;

        public Block(BlockType type)
        {
            _type = type;
        }
    }
}
