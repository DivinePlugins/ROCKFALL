using Divine.Entity;
using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Service;

namespace ESExtermination
{
    public class Bootstrap : Bootstrapper
    {
        protected override void OnActivate()
        {
            if (EntityManager.LocalHero.HeroId != HeroId.npc_dota_hero_earth_spirit)
            {
                return;
            }

            _ = new Context();
        }
    }
}
