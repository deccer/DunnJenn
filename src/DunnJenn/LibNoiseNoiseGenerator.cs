using LibNoise;
using LibNoise.Filter;
using LibNoise.Primitive;
using Microsoft.Xna.Framework;
using Serilog;

namespace DunnJenn
{
    public class LibNoiseNoiseGenerator : INoiseGenerator
    {
        private readonly ILogger _logger;
        private RidgedMultiFractal _noise;

        public LibNoiseNoiseGenerator(
            ILogger logger,
            ChunkCreatorOptions chunkCreatorOptions)
        {
            _logger = logger;
            _noise = new LibNoise.Filter.RidgedMultiFractal();
            _noise.SpectralExponent = 0.5f;
            _noise.Offset = 1f;
            _noise.Gain = 2.0f;
            _noise.Primitive2D = new SimplexPerlin(chunkCreatorOptions.Seed, NoiseQuality.Best);
            _noise.Primitive3D = new SimplexPerlin(chunkCreatorOptions.Seed, NoiseQuality.Best);
            _noise.Primitive4D = new SimplexPerlin(chunkCreatorOptions.Seed, NoiseQuality.Best);
            _noise.Frequency = Frequency;
            _noise.Lacunarity = Lacunarity;
            _noise.OctaveCount = Octaves;
            _noise.Gain = Gain;
        }

        public void GetNoiseData(Point chunkPosition, out NoiseData noiseData)
        {
            _noise.Frequency = Frequency;
            _noise.Lacunarity = Lacunarity;
            _noise.OctaveCount = Octaves;
            _noise.Gain = Gain;

            _logger.Information("Get noise for chunk {@ChunkX},{@ChunkY}", chunkPosition.X, chunkPosition.Y);
            noiseData = new NoiseData(Chunk.ChunkSize, Chunk.ChunkSize);

            for (var y = 0; y < Chunk.ChunkSize; y++)
            {
                for (var x = 0; x < Chunk.ChunkSize; x++)
                {
                    var tx = chunkPosition.X * Chunk.ChunkSize + x;
                    var ty = chunkPosition.Y * Chunk.ChunkSize + y;

                    var noiseValue = (float)_noise.GetValue(tx, ty);
                    if (noiseValue > noiseData.Max)
                    {
                        noiseData.Max = noiseValue;
                    }

                    if (noiseValue < noiseData.Min)
                    {
                        noiseData.Min = noiseValue;
                    }

                    noiseData.Data[y * Chunk.ChunkSize + x] = noiseValue;
                }
            }
        }

        public float Frequency { get; set; }
        public int Octaves { get; set; }
        public float Lacunarity { get; set; }
        public float Gain { get; set; }
        public FastNoiseLite.FractalType FractalType { get; set; }
        public FastNoiseLite.NoiseType NoiseType { get; set; }
        public float CellularJitter { get; set; }
        public FastNoiseLite.CellularDistanceFunction CellularDistanceFunction { get; set; }
        public FastNoiseLite.CellularReturnType CellularReturnType { get; set; }
    }
}
