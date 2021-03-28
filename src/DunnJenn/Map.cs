using System;
using Microsoft.Xna.Framework;

namespace DunnJenn
{
    public class Map
    {
        private readonly IChunkProvider _chunkProvider;

        internal Map(IChunkProvider chunkProvider, Guid id)
            : this(id)
        {
            _chunkProvider = chunkProvider;
        }

        public Map(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }

        public int Seed { get; set; }

        public Chunk GetChunk(Point position)
        {
            return _chunkProvider.GetChunk(Id, position);
        }
    }
}
