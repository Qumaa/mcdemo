using Project.World.Generation.Chunks;

namespace Project.World.Generation.Blocks
{
    public sealed class ChunkFaceViews : SixFaces<ChunkFaceView>
    {
        public ChunkFaceViews(Directional<ChunkFaceView> face1, Directional<ChunkFaceView> face2, Directional<ChunkFaceView> face3,
            Directional<ChunkFaceView> face4, Directional<ChunkFaceView> face5, Directional<ChunkFaceView> face6) : base(
            face1,
            face2,
            face3,
            face4,
            face5,
            face6
        ) { }
    }
}
