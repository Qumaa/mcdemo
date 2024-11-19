namespace Project.World.Generation.Block
{
    public readonly struct BlockType
    {
        public readonly int ID;

        public bool IsEmpty => ID is 0;
        
        public BlockType(int id) {
            ID = id;
        }

        public static BlockType Empty => new();
    }
}
