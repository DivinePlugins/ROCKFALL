using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Menu;
using Divine.Menu.Items;

namespace DawnBreaker_SkillHelper
{
    class MenuSettings
    {
        public Menu RootMenu { get; set; }

        public MenuSwitcher StarbreakerHelper { get; set; }
        public MenuSwitcher HammerHelper { get; set; }

        public MenuSettings()
        {
            RootMenu = MenuManager.HeroesMenu.AddMenu("DawnBreaker Helper").SetImage(HeroId.npc_dota_hero_dawnbreaker);

            StarbreakerHelper = RootMenu.AddSwitcher("Starbreaker Helper")
                                        .SetImage(AbilityId.dawnbreaker_fire_wreath)
                                        .SetTooltip("Tries to hit as many heroes as possible");

            HammerHelper = RootMenu.AddSwitcher("Hammer Helper")
                                   .SetImage(AbilityId.dawnbreaker_celestial_hammer)
                                   .SetTooltip("Using hammer on the hero will lead to maximum rapprochement");

        }
    }
}