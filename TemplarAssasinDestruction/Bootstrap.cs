using Divine.Entity;
using Divine.Service;
using Divine.Entity.Entities.Units.Heroes.Components;

namespace TemplarAssasinDestruction
{
    public class Bootstrap : Bootstrapper
    {
        private Context Context;

        protected override void OnMainActivate()
        {
            Context = new Context();
        }

        protected override void OnActivate()
        {
            if (EntityManager.LocalHero.Id != HeroId.npc_dota_hero_templar_assassin)
            {
                return;
            }

            Context.Activate();
        }
    }
}
