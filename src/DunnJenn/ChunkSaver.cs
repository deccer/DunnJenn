using System.IO;

namespace DunnJenn
{
    public sealed class ChunkSaver : IChunkSaver
    {
        public void SaveChunk(Chunk chunk, string filePath)
        {
            var planetId = chunk.MapId.ToString("N");
            var planetDirectory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(planetDirectory))
            {
                Directory.CreateDirectory(planetDirectory);
            }

            using var fileStream = File.Create(filePath, 8192, FileOptions.WriteThrough);
            using var writer = new BinaryWriter(fileStream);

            writer.Write((string) planetId);
            writer.Write(chunk.Position.X);
            writer.Write(chunk.Position.Y);
            for (var i = 0; i < chunk.Tiles.Length; i++)
            {
                writer.Write(chunk.Tiles[i].Id);
            }
        }
    }
}
