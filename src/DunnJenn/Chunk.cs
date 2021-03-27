using System;
using Microsoft.Xna.Framework;

namespace DunnJenn
{
    public class Chunk
    {
        public const int ChunkSize = 32;
        public const int ChunkSizeSquared = ChunkSize * ChunkSize;

        public Chunk(Guid mapId, Point position, Tile[] tiles)
        {
            MapId = mapId;
            Position = position;
            Tiles = tiles;
        }

        public DateTime LastLoadTime { get; set; }

        public Guid MapId { get; }

        public Point Position { get; }

        public Tile[] Tiles { get; }

        public Tile GetTile(Point position)
        {
            var localPosition = new Point(position.X & ChunkSize, position.Y & ChunkSize);
            var tileIndex = localPosition.Y * ChunkSize + localPosition.X;

            return Tiles[tileIndex];
        }
    }
}
