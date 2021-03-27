using Microsoft.Xna.Framework;
using Serilog;

namespace DunnJenn
{
    public sealed class FastNoiseNoiseGenerator : INoiseGenerator
    {
        private readonly ILogger _logger;

        private readonly FastNoiseLite _noise;

        private float _frequency = 0.01f;
        private float _lacunarity = 1.0f;
        private float _gain = 1.0f;
        private int _octaves = 5;
        private FastNoiseLite.NoiseType _noiseType = FastNoiseLite.NoiseType.OpenSimplex2;
        private FastNoiseLite.FractalType _fractalType = FastNoiseLite.FractalType.Ridged;
        private FastNoiseLite.CellularReturnType _cellularReturnType = FastNoiseLite.CellularReturnType.Distance;
        private FastNoiseLite.CellularDistanceFunction _cellularDistanceFunction = FastNoiseLite.CellularDistanceFunction.Euclidean;
        private float _cellularJitter = 0.1f;

        public FastNoiseNoiseGenerator(ILogger logger, ChunkCreatorOptions chunkCreatorOptions)
        {
            _logger = logger;
            _noise = new FastNoiseLite(chunkCreatorOptions.Seed);
            _noise.SetNoiseType(_noiseType);
            _noise.SetSeed(chunkCreatorOptions.Seed);
            _noise.SetFrequency(_frequency);

            _noise.SetFractalOctaves(_octaves);
            _noise.SetFractalLacunarity(_lacunarity);
            _noise.SetFractalType(_fractalType);
            _noise.SetFractalGain(_gain);

            _noise.SetCellularReturnType(_cellularReturnType);
            _noise.SetCellularJitter(_cellularJitter);
            _noise.SetCellularDistanceFunction(_cellularDistanceFunction);
        }

        public FastNoiseLite.CellularDistanceFunction CellularDistanceFunction
        {
            get { return _cellularDistanceFunction; }
            set
            {
                _cellularDistanceFunction = value;
                _noise.SetCellularDistanceFunction(_cellularDistanceFunction);
            }
        }

        public FastNoiseLite.CellularReturnType CellularReturnType
        {
            get { return _cellularReturnType; }
            set
            {
                _cellularReturnType = value;
                _noise.SetCellularReturnType(_cellularReturnType);
            }
        }

        public float CellularJitter
        {
            get { return _cellularJitter; }
            set
            {
                _cellularJitter = value;
                _noise.SetCellularJitter(_cellularJitter);
            }
        }

        public FastNoiseLite.FractalType FractalType
        {
            get { return _fractalType; }
            set
            {
                _fractalType = value;
                _noise.SetFractalType(_fractalType);
            }
        }

        public FastNoiseLite.NoiseType NoiseType
        {
            get { return _noiseType; }
            set
            {
                _noiseType = value;
                _noise.SetNoiseType(_noiseType);
            }
        }

        public float Frequency
        {
            get { return _frequency;}
            set
            {
                _frequency = value;
                _noise.SetFrequency(_frequency);
            }
        }

        public int Octaves
        {
            get { return _octaves; }
            set
            {
                _octaves = value;
                _noise.SetFractalOctaves(_octaves);
            }
        }

        public float Lacunarity
        {
            get { return _lacunarity; }
            set
            {
                _lacunarity = value;
                _noise.SetFractalLacunarity(_lacunarity);
            }
        }

        public float Gain
        {
            get { return _gain; }
            set
            {
                _gain = value;
                _noise.SetFractalGain(_gain);
            }
        }

        public void GetNoiseData(Point chunkPosition, out NoiseData noiseData)
        {
            _logger.Information("Get noise for chunk {@ChunkX},{@ChunkY}", chunkPosition.X, chunkPosition.Y);
            noiseData = new NoiseData(Chunk.ChunkSize, Chunk.ChunkSize);

            for (var y = 0; y < Chunk.ChunkSize; y++)
            {
                for (var x = 0; x < Chunk.ChunkSize; x++)
                {
                    var tx = chunkPosition.X * Chunk.ChunkSize + x;
                    var ty = chunkPosition.Y * Chunk.ChunkSize + y;

                    var noiseValue = (float)_noise.GetNoise(tx / (float)Chunk.ChunkSize, ty / (float)Chunk.ChunkSize);
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
    }
}
