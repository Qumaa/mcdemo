using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Project.World.Generation.Chunks;
using UnityEngine.Profiling;

namespace Project.World
{
    public class World : IChunksSupervisor
    {
        private readonly ChunksGenerator _generator;
        private Chunks _chunks;

        public World(ChunkPosition center, int loadDistance, IChunkMeshGenerator meshGenerator,
            IBlocksIteratorProvider blocksProvider, IChunkLODProvider lodProvider, ChunkViewFactory factory)
        {
            _generator = new(meshGenerator, blocksProvider, lodProvider, factory);
            _chunks = _generator.Generate(center, loadDistance);
        }

        public void UpdateChunks(ChunkPosition newCenter)
        {
            
        }

        public void SetLoadDistance(int distance) =>
            throw new NotImplementedException();
        
        

        private void HideChunkFaces()
        {
            
        }
    }

    public class ChunksGenerator
    {
        private readonly IChunkMeshGenerator _meshGenerator;
        private readonly IBlocksIteratorProvider _blocksProvider;
        private readonly IChunkLODProvider _lodProvider;
        private readonly ChunkViewFactory _factory;
        
        public ChunksGenerator(IChunkMeshGenerator meshGenerator, IBlocksIteratorProvider blocksProvider, IChunkLODProvider lodProvider, ChunkViewFactory factory)
        {
            _meshGenerator = meshGenerator;
            _blocksProvider = blocksProvider;
            _lodProvider = lodProvider;
            _factory = factory;
        }

        public Chunks Generate(ChunkPosition center, int loadDistance) =>
            new GenerationCapture(this, center, loadDistance).Generate();

        private readonly ref struct GenerationCapture
        {
            private readonly ChunksGenerator _context;
            private readonly ChunkPosition _center;
            private readonly Chunks _chunks;
            
            public GenerationCapture(ChunksGenerator context, ChunkPosition center, int loadDistance)
            {
                _context = context;
                _center = center;

                _chunks = new(loadDistance);
            }

            public Chunks Generate()
            {
                int worldSize = _chunks.GetSize();

                ChunkPosition start = _center.OffsetCopy(-_chunks.Extent);
            
                for (int x = 0; x < worldSize; x++)
                for (int y = 0; y < worldSize; y++)
                for (int z = 0; z < worldSize; z++)
                {
                    ChunkPosition position = start.OffsetCopy(x, y, z);

                    LODChunk lodChunk = CreateChunk(position);
                    lodChunk.GenerateBlocks();
                
                    _chunks.Set(position, lodChunk);
                }

                foreach (LODChunk lodChunk in _chunks.Values)
                    lodChunk.GenerateMesh();

                return _chunks;
            }

            private LODChunk CreateChunk(ChunkPosition position)
            {
                IChunkView view =  _context._factory.Create(position);
                ChunkLOD lod = _context._lodProvider.GetLevel(_center, position);
                // todo: remove _chunks parameter (remove chunk's responsibility to delegate blocks and mesh generation)
                Chunk chunk = new(position, _context._meshGenerator, _context._blocksProvider, _chunks, view);
                return new(chunk, lod);
            }
        }
    }

    public class Chunks : IChunksIterator
    {
        private readonly LODChunk[] _chunks;
        private int _size;

        public int Extent { get; private set; }
        public LODChunk[] Values => _chunks;

        public Chunks(int initialExtent)
        {
            Extent = initialExtent;
            _size = GetSize();
            _chunks = new LODChunk[_size * _size * _size];
        }

        public bool TryGetNextChunk(ChunkPosition position, FaceDirection direction, out IChunk chunk)
        {
            position = WorldToIndex(position);
            int x = position.X;
            int y = position.Y;
            int z = position.Z;

            switch (direction)
            {
                case FaceDirection.Up:
                    if (++y < _size)
                        goto success;
                    break;

                case FaceDirection.Down:
                    if (--y >= 0)
                        goto success;
                    break;

                case FaceDirection.Left:
                    if (--x >= 0)
                        goto success;
                    break;

                case FaceDirection.Right:
                    if (++x < _size)
                        goto success;
                    break;

                case FaceDirection.Forward:
                    if (++z < _size)
                        goto success;
                    break;

                case FaceDirection.Back:
                    if (--z >= 0)
                        goto success;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }

            chunk = default;
            return false;
            
            success:
            chunk = _chunks[XYZLoopIndex.Flatten(_size, x, y, z)].Chunk;
            return true;
        }

        public int GetSize() =>
            LoadDistanceToWorldSize(Extent);

        private ChunkPosition WorldToIndex(ChunkPosition position) =>
            position.OffsetCopy(Extent);

        public static int LoadDistanceToWorldSize(int loadDistance) =>
            loadDistance > 0 ? loadDistance * 2 + 1 : 1;

        public void Set(ChunkPosition position, LODChunk lodChunk)
        {
            position = WorldToIndex(position);
            int index = XYZLoopIndex.Flatten(_size, position.X, position.Y, position.Z);

            _chunks[index] = lodChunk;
        }
    }
    
    public readonly ref struct XYZLoopIndex
    {
        private readonly int _arraySize;
        private XYZLoopIndex(int arraySize) 
        {
            _arraySize = arraySize;
        }

        public Flat3DIndexEnumerator GetEnumerator() =>
            new(_arraySize);

        public static XYZLoopIndex Enumerate(int arraySize) =>
            new(arraySize);
        
        public static int Flatten(int size, int x, int y, int z) =>
            x + (y * size) + (z * size * size);

        public ref struct Flat3DIndexEnumerator
        {
            private readonly int _size, _size2, _size3;
            private int _x, _y, _z;

            public int Current { get; private set; }

            public Flat3DIndexEnumerator(int arraySize)
            {
                _size = arraySize;
                _size2 = _size * _size;
                _size3 = _size2 * _size;

                _x = _y = _z = 0;
                Current = -_size2;
            }

            public bool MoveNext()
            {
                _z++;
                Current += _size2;
                if (_z < _size)
                    return true;

                _z -= _size;
                Current -= _size3;

                _y++;
                Current += _size;
                if (_y < _size)
                    return true;

                _y -= _size;
                Current -= _size2;

                _x++;
                Current++;
                return _x < _size;
            }

            public void Reset()
            {
                _x = _y = 0;
                _z = -1;
                Current = -_size2;
            }
        }
    }
}
