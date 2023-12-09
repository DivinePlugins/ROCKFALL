using Divine.Entity;
using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Service;

namespace DawnBreaker_SkillHelper
{
    public class Bootstrap : Bootstrapper
    {
        private MenuSettings MenuSettings { get; set; }

        private Context Context { get; set; }

        protected override void OnMainActivate()
        {
            MenuSettings = new();
        }

        protected override void OnActivate()
        {
            if (EntityManager.LocalHero.Id != HeroId.npc_dota_hero_dawnbreaker)
            {
                return;
            }

            Context = new Context(MenuSettings);
        }

        protected override void OnDeactivate()
        {
            Context?.Dispose();
        }

    }
}
