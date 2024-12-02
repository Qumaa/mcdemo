using System;

namespace Project.World.Generation.Chunks
{
    public enum ChunkLOD
    {
        Full = 0,     // 16x16
        Half = 1,     // 8x8
        Quarter = 2,  // 4x4
        Eighth = 3,   // 2x2
        Sixteenth = 4 // 1x1
    }

    public static class ChunkLODs
    {
        public static readonly int Number = Enum.GetNames(typeof(ChunkLOD)).Length;

        public static int ToInt(this ChunkLOD lod) =>
            lod switch
            {
                ChunkLOD.Full => 0,
                ChunkLOD.Half => 1,
                ChunkLOD.Quarter => 2,
                ChunkLOD.Eighth => 3,
                ChunkLOD.Sixteenth => 4,
                _ => throw new ArgumentOutOfRangeException(nameof(lod), lod, null)
            };

        public static ChunkLOD FromInt(int lod) =>
            lod switch
            {
                0 => ChunkLOD.Full,
                1 => ChunkLOD.Half,
                2 => ChunkLOD.Quarter,
                3 => ChunkLOD.Eighth,
                4 => ChunkLOD.Sixteenth,
                _ => throw new ArgumentOutOfRangeException(nameof(lod), lod, null)
            };

        public static int ChunkSize(this ChunkLOD lod) =>
            lod switch
            {
                ChunkLOD.Full => 16,
                ChunkLOD.Half => 8,
                ChunkLOD.Quarter => 4,
                ChunkLOD.Eighth => 2,
                ChunkLOD.Sixteenth => 1,
                _ => throw new ArgumentOutOfRangeException(nameof(lod), lod, null)
            };
    }
}
