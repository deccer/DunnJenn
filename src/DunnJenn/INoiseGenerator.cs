using Microsoft.Xna.Framework;

namespace DunnJenn
{
    public interface INoiseGenerator
    {
        void GetNoiseData(Point chunkPosition, out NoiseData noiseData);
        
        float Frequency { get; set; }
        
        int Octaves { get; set; }
        
        float Lacunarity { get; set; }
        
        float Gain { get; set; }
        
        FastNoiseLite.FractalType FractalType { get; set; }
        
        FastNoiseLite.NoiseType NoiseType { get; set; }
        
        float CellularJitter { get; set; }
        
        FastNoiseLite.CellularDistanceFunction CellularDistanceFunction { get; set; }
        
        FastNoiseLite.CellularReturnType CellularReturnType { get; set; }
    }
}
