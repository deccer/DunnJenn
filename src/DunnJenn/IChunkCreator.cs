using System;
using Microsoft.Xna.Framework;

namespace DunnJenn
{
    public interface IChunkCreator
    {
        Chunk CreateChunk(Guid mapId, Point position);
    }
}
