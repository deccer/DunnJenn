using System;

namespace DunnJenn
{
    public interface IMapProvider
    {
        Map GetMap(Guid mapId);
    }
}
