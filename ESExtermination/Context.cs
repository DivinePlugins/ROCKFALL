namespace ESExtermination;

using Divine.Entity;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Menu;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Renderer;

using ESExtermination.Feature;

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
        RootMenu = MenuManager.HeroesMenu.AddMenu("ESExtermination").SetImage((HeroId.npc_dota_hero_earth_spirit.ToString(), ImageType.Unit));

        Status = RootMenu.AddSwitcher("On/Off");

        ComboKey = RootMenu.AddHoldKey("Combo key");
        LockedTarget = RootMenu.AddSwitcher("Locked target");

        TargetSelector = new TargetSelector(this);
        Combo = new Combo(this);
        Ultimate = new Ultimate(this);
        SmartSpells = new SmartSpells(this);
        AutoSave = new AutoSave(this);
        AutoPushInTowerDirection = new AutoPushInTowerDirection(this);
        AutoPushInAlliesDirection = new AutoPushInAlliesDirection(this);

        Status.ValueChanged += Status_ValueChanged;
    }

    private void Status_ValueChanged(MenuSwitcher switcher, SwitcherChangedEventArgs e)
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
