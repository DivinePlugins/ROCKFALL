using Divine.Entity;
using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Service;

namespace RockRubick
{
    internal sealed class Bootstrap : Bootstrapper
    {
        private Context Context;

        protected override void OnMainActivate()
        {
            Context = new Context();
        }

        protected override void OnActivate()
        {
            if (EntityManager.LocalHero.Id != HeroId.npc_dota_hero_rubick)
            {
                return;
            }

            Context.Activate();
        }
    }
}
