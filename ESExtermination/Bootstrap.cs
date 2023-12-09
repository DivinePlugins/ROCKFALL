using Divine.Entity;
using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Service;

namespace ESExtermination
{
    public class Bootstrap : Bootstrapper
    {
        private Context Context;

        protected override void OnMainActivate()
        {
            Context = new Context();
        }

        protected override void OnMainDeactivate()
        {
            Context.Dispose();
        }

        protected override void OnActivate()
        {
            if (EntityManager.LocalHero.Id != HeroId.npc_dota_hero_earth_spirit)
            {
                return;
            }

            Context.Activate();
        }
    }
}
