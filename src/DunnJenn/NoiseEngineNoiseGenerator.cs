using Microsoft.Xna.Framework;

namespace DunnJenn
{
    public class NoiseEngineNoiseGenerator : INoiseGenerator
    {
        public void GetNoiseData(Point chunkPosition, out NoiseData noiseData)
        {
            throw new System.NotImplementedException();
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
