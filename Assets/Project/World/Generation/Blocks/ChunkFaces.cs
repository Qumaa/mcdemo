using Project.World.Generation.Chunks;

namespace Project.World.Generation.Blocks
{
    public sealed class ChunkFaces : SixFaces<ChunkFace>
    {
        public ChunkFaces(Directional<ChunkFace> face1, Directional<ChunkFace> face2, Directional<ChunkFace> face3,
            Directional<ChunkFace> face4, Directional<ChunkFace> face5, Directional<ChunkFace> face6) : base(
            face1,
            face2,
            face3,
            face4,
            face5,
            face6
        ) { }
    }
}
