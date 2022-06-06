using System;
using System.Linq;
using Divine.Entity;
using Divine.Entity.Entities.Units;
using Divine.Extensions;
using Divine.Numerics;

namespace ESExtermination.Extensions
{
    internal static class StoneExtensions
    {
        public const string StoneName = "npc_dota_earth_spirit_stone";

        public static bool IsStone(this Unit unit)
        {
            return unit.Name == StoneName;
        }

        public static Unit FirstUnitInRange(Vector3 pos, string unitName, float searchRadius)
        {
            return EntityManager.GetEntities<Unit>()
                .Where(x => x.Distance2D(pos) < searchRadius
                     && x.Name == unitName
                     && x.IsAlive)
                .OrderBy(x => x.Distance2D(pos))
                .FirstOrDefault();
        }

        public static Unit FirstUnitBetween(Vector3 a, Vector3 b, string unitName, float radius = 180f, float maxRange = float.MaxValue)
        {
            float dist = a.Distance2D(b);

            float checkRange = dist > maxRange ? maxRange : dist;

            int iter = (int)Math.Ceiling(checkRange / radius);

            for (float i = 0; i < iter; i++)
            {
                var stone = FirstUnitInRange(a.Extend(b, i * radius), unitName, radius);
                if (stone != null)
                {
                    return stone;
                }
            }

            return null;
        }

    }
}
