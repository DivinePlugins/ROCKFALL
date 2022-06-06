using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Menu;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;

namespace RockRubick
{
    internal sealed class Menu
    {
        private static MenuSwitcher RubickEnabled;
        private SpellStealMain spellStealMain;

        public void MenuBootstrap()
        {
            var rootmenu = MenuManager.CreateRootMenu("Rock.Rubick").SetHeroImage(HeroId.npc_dota_hero_rubick);

            RubickEnabled = rootmenu.CreateSwitcher("Spell Stealer").SetAbilityImage(AbilityId.rubick_spell_steal);

            RubickEnabled.ValueChanged += RubickEnabled_ValueChanged;
        }

        private void RubickEnabled_ValueChanged(MenuSwitcher switcher, SwitcherEventArgs e)
        {
            if (e.Value)
            {
                spellStealMain = new SpellStealMain();
            }
            else
            {
                spellStealMain.Dispose();
            }
        }

    }
}
