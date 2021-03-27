using System;
using Microsoft.Xna.Framework;
using Serilog;

namespace DunnJenn
{
    public class World
    {
        private readonly ILogger _logger;
        private readonly IMapProvider _mapProvider;

        public World(
            ILogger logger,
            IMapProvider mapProvider)
        {
            _logger = logger.ForContext<World>();
            _mapProvider = mapProvider;
        }

        public int Seed { get; set; }

        public Chunk GetChunk(Guid mapId, Point position)
        {
            var map = _mapProvider.GetMap(mapId);
            map.Seed = Seed;
            return map.GetChunk(position);
        }

        public Tile GetTile(Guid mapId, Point position)
        {
            var map = _mapProvider.GetMap(mapId);
            var mapChunk = map.GetChunk(position);

            return mapChunk.GetTile(position);
        }

        public void Tick()
        {
        }
    }
}