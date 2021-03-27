using System;
using Microsoft.Xna.Framework;

namespace DunnJenn
{
    public interface IChunkProvider
    {
        Chunk GetChunk(Guid mapId, Point position);

        void Reset(ResetOptions resetOptions);
    }
}
