using Divine.Entity;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Input;
using Divine.Menu;
using Divine.Menu.Items;

using ESExtermination.Feature;

namespace ESExtermination
{
    internal class Context
    {
        public Hero LocalHero { get; }

        public Menu RootMenu { get; }

        public MenuSwitcher Status { get; }

        public Combo Combo { get; private set; }
        public MenuHoldKey ComboKey { get; private set; }
        public MenuSwitcher LockedTarget { get; private set; }

        public TargetSelector TargetSelector { get; private set; }

        public Ultimate Ultimate { get; private set; }
        public AutoPushInTowerDirection AutoPushInTowerDirection { get; private set; }
        public AutoPushInAlliesDirection AutoPushInAlliesDirection { get; private set; }
        public AutoSave AutoSave { get; private set; }
        public SmartSpells SmartSpells { get; private set; }

        public Context()
        {
            LocalHero = EntityManager.LocalHero;
            RootMenu = MenuManager.CreateRootMenu("ESExterminationV1", "ESExtermination")
                 .SetHeroImage(HeroId.npc_dota_hero_earth_spirit);

            Status = RootMenu.CreateSwitcher("On/Off");

            ComboKey = RootMenu.CreateHoldKey("Combo key", Key.None);
            LockedTarget = RootMenu.CreateSwitcher("Locked target");

            TargetSelector = new TargetSelector(this);
            Combo = new Combo(this);
            Ultimate = new Ultimate(this);
            SmartSpells = new SmartSpells(this);
            AutoSave = new AutoSave(this);
            AutoPushInTowerDirection = new AutoPushInTowerDirection(this);
            AutoPushInAlliesDirection = new AutoPushInAlliesDirection(this);

            Status.ValueChanged += Status_ValueChanged;
        }

        private void Status_ValueChanged(MenuSwitcher switcher, Divine.Menu.EventArgs.SwitcherEventArgs e)
        {
            if (e.Value)
            {
                TargetSelector.Start();
                Combo.Start();
                SmartSpells.Start();
                AutoSave.Start();
                AutoPushInTowerDirection.Start();
                Ultimate.Start();
                AutoPushInAlliesDirection.Start();
            }
            else
            {
                TargetSelector.Dispose();
                Combo.Dispose();
                SmartSpells.Dispose();
                AutoSave.Dispose();
                AutoPushInTowerDirection.Dispose();
                Ultimate.Dispose();
                AutoPushInAlliesDirection.Dispose();
            }
        }
    }
}
