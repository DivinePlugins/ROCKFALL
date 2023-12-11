namespace ESExtermination;

using Divine.Entity;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items.Components;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Menu;
using Divine.Menu.Components;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;

using ESExtermination.Feature;

internal class Context
{
    private readonly MenuTogglerItem[] items =
    [
        (ItemId.item_veil_of_discord, true),
        (ItemId.item_blink, true),
        (ItemId.item_arcane_blink, true),
        (ItemId.item_overwhelming_blink, true),
        (ItemId.item_swift_blink, true),
        (ItemId.item_orchid, true),
        (ItemId.item_bloodthorn, true),
        (ItemId.item_sheepstick, true),
        (ItemId.item_abyssal_blade, true),
        (ItemId.item_shivas_guard, true),
        (ItemId.item_urn_of_shadows, true),
        (ItemId.item_spirit_vessel, true),
        (ItemId.item_heavens_halberd, true)
    ];

    public Hero LocalHero { get; private set; }

    public Menu RootMenu { get; }

    public MenuSwitcher Status { get; }

    public Combo Combo { get; private set; }
    public MenuHoldKey ComboKey { get; private set; }
    public MenuSwitcher LockedTarget { get; private set; }

    public static MenuItemToggler ComboItems;

    public MenuSlider autoUseMagnetize;
    public MenuSwitcher extendMagnetizeTime;
    public MenuSlider minStones;

    public MenuSwitcher smartSmash;
    public MenuSwitcher smartRoll;
    public MenuSwitcher smartGrip;

    public MenuSlider minHpPercentForSave;

    public MenuSelector underTowersMode;
    public MenuSlider underTowersAadditionalRange;

    public MenuSelector underAlliesMode;
    public MenuSlider underAlliesMinAllyHpPercent;

    public TargetSelector TargetSelector { get; private set; }

    public Ultimate Ultimate { get; private set; }
    public AutoPushInTowerDirection AutoPushInTowerDirection { get; private set; }
    public AutoPushInAlliesDirection AutoPushInAlliesDirection { get; private set; }
    public AutoSave AutoSave { get; private set; }
    public SmartSpells SmartSpells { get; private set; }

    public Context()
    {
        RootMenu = MenuManager.HeroesMenu.AddMenu("ESExtermination").SetImage(HeroId.npc_dota_hero_earth_spirit);

        Status = RootMenu.AddSwitcher("On/Off");

        ComboKey = RootMenu.AddHoldKey("Combo key");
        LockedTarget = RootMenu.AddSwitcher("Locked target");
        ComboItems = RootMenu.AddItemToggler("Items", items).SetTooltip("Select combo items");

        var ultimateFeaturesMenu = RootMenu.AddMenu("Ultimate").SetTooltip("Ultimate features").SetImage(AbilityId.earth_spirit_magnetize);
        autoUseMagnetize = ultimateFeaturesMenu.AddSlider("Min. enemies for auto ultimate", 2, 0, 5).SetTooltip("Set 0 for disable this feature");
        extendMagnetizeTime = ultimateFeaturesMenu.AddSwitcher("Extend magnetize debuff").SetTooltip("Place stone for update magnetize");
        minStones = ultimateFeaturesMenu.AddSlider("Min stones for extend magnetize debuff", 2, 0, 5).SetTooltip("If stones charges < this value then no stones will be placed").Hide();

        var smartSpellsMenu = RootMenu.AddMenu("Smart spells").SetTooltip("Auto place stone when it is necessary");
        smartSmash = smartSpellsMenu.AddSwitcher("When using smash").SetImage(AbilityId.earth_spirit_boulder_smash);
        smartRoll = smartSpellsMenu.AddSwitcher("When using roll").SetImage(AbilityId.earth_spirit_rolling_boulder);
        smartGrip = smartSpellsMenu.AddSwitcher("When using grip").SetImage(AbilityId.earth_spirit_geomagnetic_grip);

        minHpPercentForSave = RootMenu.AddSlider("Min. HP% for save with enchant", 0, 0, 50).SetTooltip("Uses enchant + smash for save").SetImage(AbilityId.item_ultimate_scepter);

        var underTowersMenu = RootMenu.AddMenu("Auto push under towers").SetImage(AbilityId.earth_spirit_boulder_smash);
        underTowersMode = underTowersMenu.AddSelector("Mode", ["Off", "On", "Also with agha"]).SetTooltip("Auto use smash for push under tower");
        underTowersAadditionalRange = underTowersMenu.AddSlider("Additional range around tower", 0, -500, 1000);

        var underAlliesMenu = RootMenu.AddMenu("Auto push under allies").SetImage(AbilityId.earth_spirit_boulder_smash);
        underAlliesMode = underAlliesMenu.AddSelector("Mode", ["Off", "On", "Also with agha"]).SetTooltip("Auto use smash for push in allies");
        underAlliesMinAllyHpPercent = underAlliesMenu.AddSlider("Min. ally hp %", 20, 0, 80);
    }

    public void Dispose()
    {
        Status.ValueChanged -= Status_ValueChanged;
    }

    public void Activate()
    {
        Status.ValueChanged += Status_ValueChanged;
    }

    private void Status_ValueChanged(MenuSwitcher switcher, SwitcherChangedEventArgs e)
    {
        if (e.Value)
        {
            if (LocalHero is null)
            {
                LocalHero = EntityManager.LocalHero;
                TargetSelector = new TargetSelector(this);
                Combo = new Combo(this);
                Ultimate = new Ultimate(this);
                SmartSpells = new SmartSpells(this);
                AutoSave = new AutoSave(this);
                AutoPushInTowerDirection = new AutoPushInTowerDirection(this);
                AutoPushInAlliesDirection = new AutoPushInAlliesDirection(this);
            }

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
