using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Menu;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;

namespace RockRubick
{
    internal sealed class Context
    {
        private static MenuSwitcher RubickEnabled;
        private SpellStealMain spellStealMain;

        public Context()
        {
            var rootmenu = MenuManager.HeroesMenu.AddMenu("Rock.Rubick").SetImage(HeroId.npc_dota_hero_rubick);
            RubickEnabled = rootmenu.AddSwitcher("Spell Stealer").SetImage(AbilityId.rubick_spell_steal);
        }

        public void Activate()
        {
            RubickEnabled.ValueChanged += RubickEnabled_ValueChanged;
        }

        private void RubickEnabled_ValueChanged(MenuSwitcher switcher, SwitcherChangedEventArgs e)
        {
            if (e.Value)
            {
                new General();
                spellStealMain = new SpellStealMain();
            }
            else
            {
                spellStealMain.Dispose();
            }
        }

    }
}
