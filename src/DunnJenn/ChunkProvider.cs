using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Serilog;

namespace DunnJenn
{
    public sealed class ChunkProvider : IChunkProvider
    {
        private readonly ILogger _logger;
        private readonly ChunkProviderOptions _chunkProviderOptions;
        private readonly IChunkLoader _chunkLoader;
        private readonly IChunkSaver _chunkSaver;
        private readonly IChunkCreator _chunkCreator;
        private readonly IDictionary<Point, Chunk> _chunks;

        public ChunkProvider(
            ILogger logger,
            ChunkProviderOptions chunkProviderOptions,
            IChunkLoader chunkLoader,
            IChunkSaver chunkSaver,
            IChunkCreator chunkCreator)
        {
            _logger = logger.ForContext<ChunkProvider>();
            _chunkProviderOptions = chunkProviderOptions;
            _chunkLoader = chunkLoader;
            _chunkSaver = chunkSaver;
            _chunkCreator = chunkCreator;
            _chunks = new Dictionary<Point, Chunk>(8192);

            if (!Directory.Exists(_chunkProviderOptions.StoragePath))
            {
                Directory.CreateDirectory(_chunkProviderOptions.StoragePath);
            }
        }

        public Chunk GetChunk(Guid mapId, Point position)
        {
            if (_chunks.TryGetValue(position, out var chunk))
            {
                _logger.Debug("Retrieving chunk for map {@Id} at {@X} {@Y} from cache", mapId, position.X, position.Y);
                chunk.LastLoadTime = DateTime.UtcNow;
                return chunk;
            }

            var chunkFilePath = Path.Combine(_chunkProviderOptions.StoragePath, mapId.ToString("N"), $"{position.X}-{position.Y}.chunk");
            if (TryLoadChunk(position, chunkFilePath, out chunk))
            {
                return chunk;
            }

            if (TryCreateChunk(mapId, position, chunkFilePath, out chunk))
            {
                return chunk;
            }

            _logger.Error("Unable to retrieve from cache/load from disk/generate chunk for map {@Id} at position {@X} {@Y}", mapId, position.X, position.Y);
            return null;
        }

        public void Reset(ResetOptions resetOptions)
        {
            if ((resetOptions & ResetOptions.ResetCache) == ResetOptions.ResetCache)
            {
                _chunks.Clear();
            }
            
            if ((resetOptions & ResetOptions.ResetFileSystem) == ResetOptions.ResetFileSystem)
            {
                Directory.Delete(_chunkProviderOptions.StoragePath, true);
                Directory.CreateDirectory(_chunkProviderOptions.StoragePath);
            }
        }

        private bool TryLoadChunk(Point position, string chunkFilePath, out Chunk chunk)
        {
            chunk = _chunkLoader.LoadChunk(chunkFilePath);
            if (chunk == null)
            {
                return false;
            }
            
            _logger.Debug("Retrieved chunk for map {@Id} at {@X} {@Y} from loader", chunk.MapId, position.X, position.Y);
            chunk.LastLoadTime = DateTime.Now;
            _chunks.Add(position, chunk);

            return true;
        }

        private bool TryCreateChunk(Guid mapId, Point position, string chunkFilePath, out Chunk chunk)
        {
            chunk = _chunkCreator.CreateChunk(mapId, position);
            if (chunk == null)
            {
                return false;
            }
            
            _logger.Debug("Generated chunk for map {@Id} at {@X} {@Y}", mapId, position.X, position.Y);
            _chunkSaver.SaveChunk(chunk, chunkFilePath);
            _chunks.Add(position, chunk);

            return true;

        }
    }
}
