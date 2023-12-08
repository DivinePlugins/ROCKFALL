using Divine.Menu;
using Divine.Menu.Items;

namespace DawnBreaker_SkillHelper
{
    class MenuSettings
    {
        public Menu RootMenu { get; set; }

        public MenuSwitcher StarbreakerHelper { get; set; }
        public MenuSwitcher HammerHelper { get; set; }

        public MenuSettings(Context Context)
        {
            RootMenu = MenuManager.HeroesMenu.AddMenu("DawnBreaker Helper").SetImage(Context.Dawnbreaker.LocalHero.Id);

            StarbreakerHelper = RootMenu.AddSwitcher("Starbreaker Helper")
                                        .SetImage(Context.Dawnbreaker.Starbreaker.Id)
                                        .SetTooltip("Tries to hit as many heroes as possible");

            HammerHelper = RootMenu.AddSwitcher("Hammer Helper")
                                   .SetImage(Context.Dawnbreaker.Hammer.Id)
                                   .SetTooltip("Using hammer on the hero will lead to maximum rapprochement");

        }

        public void Dispose()
        {
            RootMenu.Remove("DawnBreaker Helper");
            RootMenu.Remove("Starbreaker Helper");
            RootMenu.Remove("Hammer Helper");
        }

    }
}
