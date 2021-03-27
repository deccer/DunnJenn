using System;
using System.Collections.Generic;
using Serilog;

namespace DunnJenn
{
    public sealed class MapProvider : IMapProvider
    {
        private readonly ILogger _logger;
        private readonly IChunkProvider _chunkProvider;

        private readonly IDictionary<Guid, Map> _maps;

        public MapProvider(
            ILogger logger,
            IChunkProvider chunkProvider)
        {
            _logger = logger.ForContext<MapProvider>();
            _chunkProvider = chunkProvider;
            _maps = new Dictionary<Guid, Map>(16);
        }

        public Map GetMap(Guid mapId)
        {
            if (_maps.TryGetValue(mapId, out var map))
            {
                return map;
            }

            map = new Map(_chunkProvider, mapId);
            _maps.Add(mapId, map);
            return map;
        }
    }
}