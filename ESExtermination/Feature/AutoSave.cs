namespace ESExtermination.Feature;

using System.Linq;

using Divine.Entity;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items.Components;
using Divine.Entity.Entities.Units;
using Divine.Extensions;
using Divine.Menu;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Renderer;
using Divine.Update;

using ESExtermination.Abilities.Spells;
using ESExtermination.Extensions;

internal class AutoSave : FeatureBase
{
    private MenuSlider minHpPercentForSave;

    private readonly string fountainName = "dota_fountain";

    private Unit myFountain;

    private Smash smash;
    private Enchant enchant;

    public AutoSave(Context context)
        : base(context)
    {
        smash = context.Combo.Smash;
        enchant = context.Combo.Enchant;

        minHpPercentForSave = rootMenu
            .AddSlider("Min. HP% for save with enchant", 0, 0, 50)
            .SetTooltip("Uses enchant + smash for save")
            .SetImage(ItemId.item_ultimate_scepter, true);

        myFountain = EntityManager.GetEntities<Unit>().FirstOrDefault(x => x.Name == fountainName
                                                                        && x.IsAlly(localHero));
    }

    public override void Start()
    {
        minHpPercentForSave.ValueChanged += MinHpPercentForSave_ValueChanged;
    }

    public override void Dispose()
    {
        UpdateManager.DestroyIngameUpdate(AutoSaveUpdater);
        minHpPercentForSave.ValueChanged -= MinHpPercentForSave_ValueChanged;
    }

    private void MinHpPercentForSave_ValueChanged(MenuSlider slider, SliderChangedEventArgs e)
    {
        if (e.Value != 0)
        {
            UpdateManager.CreateIngameUpdate(200, AutoSaveUpdater);
        }
        else
        {
            UpdateManager.DestroyIngameUpdate(AutoSaveUpdater);
        }
    }

    private void AutoSaveUpdater()
    {
        if (localHero.HealthPercent() > (float)minHpPercentForSave.Value / 100)
        {
            return;
        }

        var isFacingFountain = localHero.InFront(localHero.Distance2D(myFountain)).Distance2D(myFountain.Position) < 500;

        if (!localHero.IsEnchanted() && enchant.CanBeCasted())
        {
            if (isFacingFountain)
            {
                enchant.UseAbility(localHero);
            }
            else
            {
                localHero.MoveToDirection(localHero.Position.Extend(myFountain.Position, 400));
            }
            return;
        }

        if (smash.CanBeCasted() && localHero.IsEnchanted() && isFacingFountain)
        {
            smash.Base.Cast();
        }
    }
}
