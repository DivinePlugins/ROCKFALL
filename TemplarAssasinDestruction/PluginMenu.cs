using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items.Components;
using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Menu;
using Divine.Menu.Components;
using Divine.Menu.Items;

namespace TemplarAssasinDestruction
{
    internal class PluginMenu
    {
        public MenuSwitcher PluginStatus { get; set; }
        public MenuHoldKey ComboKey { get; set; }
        public MenuItemToggler ComboItems { get; set; }
        public Menu HarassMenu { get; set; }
        public MenuHoldKey HarassKey { get; set; }
        public MenuSelector HarassMode { get; set; }

        private Menu RootMenu;

        private readonly MenuTogglerItem[] Items =
        [
            (ItemId.item_blink, true),
            (ItemId.item_swift_blink, true),
            (ItemId.item_overwhelming_blink, true),
            (ItemId.item_arcane_blink, true),
            (ItemId.item_black_king_bar, true),
            (ItemId.item_sheepstick, true),
            (ItemId.item_manta, true),
            (ItemId.item_nullifier, true),
            (ItemId.item_orchid, true),
            (ItemId.item_bloodthorn, true)
        ];

        public PluginMenu()
        {
            RootMenu = MenuManager.HeroesMenu.AddMenu("TADestruction")
                .SetImage(HeroId.npc_dota_hero_templar_assassin)
                .SetTooltip("V1.2 BETA");

            PluginStatus = RootMenu.AddSwitcher("On/Off");
            ComboKey = RootMenu.AddHoldKey("Combo Key");
            ComboItems = RootMenu.AddItemToggler("Items", Items);
            HarassMenu = RootMenu.AddMenu("Harass Settings");
            HarassMode = HarassMenu.AddSelector("Harass Mode", ["Closest Harass Position", " To Mouse"]);
            HarassKey = HarassMenu.AddHoldKey("Harass Key");

        }
    }
}