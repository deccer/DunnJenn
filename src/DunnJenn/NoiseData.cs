namespace DunnJenn
{
    public sealed class NoiseData
    {
        public float[] Data { get; }

        public float Min { get; internal set; }

        public float Max { get; internal set; }

        public NoiseData(int width, int height)
        {
            Data = new float[width * height];
            Min = float.MaxValue;
            Max = float.MinValue;
        }
    }
}
