using System;
using System.Collections.Generic;
using System.Linq;
using Divine.Entity;
using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;
using Divine.Game;
using Divine.Helpers;
using Divine.Menu.Items;
using Divine.Orbwalker;
using Divine.Update;
using ESExtermination.Abilities.Items;
using ESExtermination.Abilities.Spells;
using ESExtermination.Extensions;

namespace ESExtermination
{
    internal class Combo : IDisposable
    {
        private Dictionary<AbilityId, bool> items = new Dictionary<AbilityId, bool>
        {
            { AbilityId.item_veil_of_discord, true },
            { AbilityId.item_blink, true },
            { AbilityId.item_arcane_blink, true },
            { AbilityId.item_overwhelming_blink,true },
            { AbilityId.item_swift_blink, true },
            { AbilityId.item_orchid, true },
            { AbilityId.item_bloodthorn, true },
            { AbilityId.item_sheepstick, true },
            { AbilityId.item_abyssal_blade, true },
            { AbilityId.item_shivas_guard, true },
            { AbilityId.item_urn_of_shadows, true },
            { AbilityId.item_spirit_vessel, true },
            { AbilityId.item_heavens_halberd, true }
        };

        public Grip Grip { get; private set; }
        public Roll Roll { get; private set; }
        public Smash Smash { get; private set; }
        public Stone Stone { get; private set; }
        public Enchant Enchant { get; private set; }
        public Magnetize Magnetize { get; private set; }

        public VeilOfDiscord VeilOfDiscord { get; private set; }
        public SpiritVessel SpiritVessel { get; private set; }
        public Shivas Shivas { get; private set; }
        public Orchid Orchid { get; private set; }
        public Hex Hex { get; private set; }
        public Abyssal Abyssal { get; private set; }
        public Blink Blink { get; private set; }
        public Halberd Halberd { get; private set; }

        private Context context;

        private Sleeper AbilitySleeper = new Sleeper();
        private Sleeper OrbWalkerSleeper = new Sleeper();

        private Hero localHero;

        public static MenuItemToggler ComboItems;

        public Combo(Context context)
        {
            this.context = context;

            localHero = context.LocalHero;
            ComboItems = context.RootMenu.CreateItemToggler("Items", items)
                                         .SetTooltip("Select combo items");

            Smash = new Smash(localHero.GetAbilityById(AbilityId.earth_spirit_boulder_smash));
            Roll = new Roll(localHero.GetAbilityById(AbilityId.earth_spirit_rolling_boulder));
            Grip = new Grip(localHero.GetAbilityById(AbilityId.earth_spirit_geomagnetic_grip));
            Stone = new Stone(localHero.GetAbilityById(AbilityId.earth_spirit_stone_caller));
            Enchant = new Enchant(localHero.GetAbilityById(AbilityId.earth_spirit_petrify));
            Magnetize = new Magnetize(localHero.GetAbilityById(AbilityId.earth_spirit_magnetize));
        }

        public void Start()
        {
            context.ComboKey.ValueChanged += ComboKey_ValueChanged;
            UpdateManager.CreateIngameUpdate(1000, ItemUpdater);
        }

        public void Dispose()
        {
            UpdateManager.DestroyIngameUpdate(ItemUpdater);
            context.ComboKey.ValueChanged -= ComboKey_ValueChanged;
            UpdateManager.IngameUpdate -= UpdateManager_IngameUpdate;
        }

        private void ComboKey_ValueChanged(MenuHoldKey holdKey, Divine.Menu.EventArgs.HoldKeyEventArgs e)
        {
            if (e.Value)
            {
                UpdateManager.IngameUpdate += UpdateManager_IngameUpdate;
            }
            else
            {
                UpdateManager.IngameUpdate -= UpdateManager_IngameUpdate;
            }
        }

        private bool IsOrchid(Ability ability)
        {
            var abilId = ability.Id;

            if (abilId == AbilityId.item_orchid
                || abilId == AbilityId.item_bloodthorn)
            {
                return true;
            }

            return false;
        }

        private bool IsBlink(Ability ability)
        {
            var abilId = ability.Id;

            if (abilId == AbilityId.item_blink
                || abilId == AbilityId.item_arcane_blink
                || abilId == AbilityId.item_overwhelming_blink
                || abilId == AbilityId.item_swift_blink)
            {
                return true;
            }

            return false;
        }

        private bool IsSpiritVessel(Ability ability)
        {
            var abilId = ability.Id;

            if (abilId == AbilityId.item_spirit_vessel
                || abilId == AbilityId.item_urn_of_shadows)
            {
                return true;
            }

            return false;
        }

        private void ItemUpdater()
        {
            var mainItems = localHero.Inventory?.MainItems;

            if (mainItems == null)
            {
                return;
            }

            foreach (var item in mainItems)
            {
                if (IsBlink(item))
                {
                    Blink = new Blink(item);
                }

                if (IsOrchid(item))
                {
                    Orchid = new Orchid(item);
                }

                if (IsSpiritVessel(item))
                {
                    SpiritVessel = new SpiritVessel(item);
                }

                if (VeilOfDiscord == null
                    && item.Id == AbilityId.item_veil_of_discord)
                {
                    VeilOfDiscord = new VeilOfDiscord(item);
                }

                if (Shivas == null
                    && item.Id == AbilityId.item_shivas_guard)
                {
                    Shivas = new Shivas(item);
                }

                if (Hex == null
                    && item.Id == AbilityId.item_sheepstick)
                {
                    Hex = new Hex(item);
                }

                if (Abyssal == null
                    && item.Id == AbilityId.item_abyssal_blade)
                {
                    Abyssal = new Abyssal(item);
                }

                if (Halberd == null
                    && item.Id == AbilityId.item_heavens_halberd)
                {
                    Halberd = new Halberd(item);
                }
            }
        }

        private void UpdateManager_IngameUpdate()
        {

            var target = TargetSelector.CurrentTarget;

            if (target == null || !target.IsAlive || target.IsEnchanted())
            {
                return;
            }

            if (!OrbWalkerSleeper.Sleeping)
            {
                OrbwalkerManager.OrbwalkTo(target, GameManager.MousePosition);
                OrbWalkerSleeper.Sleep(200);
                return;
            }

            if (AbilitySleeper.Sleeping || localHero.IsRooted())
            {
                return;
            }

            var distanceToTarget = localHero.Distance2D(target);
            var blinkRange = Blink?.Range;

            //blink logic
            if (Blink != null
                && Blink.CanBeCasted())
            {
                if (Roll.CanBeCasted()
                    && blinkRange < distanceToTarget
                    && blinkRange + Roll.Range > distanceToTarget
                    && Blink.UseAbility(target))
                {
                    Stone.Sleeper.Sleep(300);
                    Roll.Sleeper.Sleep(300);
                    AbilitySleeper.Sleep(200);
                    return;
                }

                if (!Roll.CanBeCasted()
                    && distanceToTarget > blinkRange / 3
                    && distanceToTarget < blinkRange
                    && Blink.UseAbility(target))
                {

                    Stone.Sleeper.Sleep(300);
                    Roll.Sleeper.Sleep(300);
                    AbilitySleeper.Sleep(200);
                    return;
                }
            }

            if (VeilOfDiscord != null
                && VeilOfDiscord.UseAbility(target))
            {
                AbilitySleeper.Sleep(100);
                return;
            }

            if (Hex != null
                && Hex.UseAbility(target))
            {
                AbilitySleeper.Sleep(100);
                return;
            }

            if (Abyssal != null
                && Abyssal.UseAbility(target))
            {
                AbilitySleeper.Sleep(100);
                return;
            }

            if (target.IsMagicImmune())
            {
                return;
            }

            var firstStoneNearEnemy = StoneExtensions.FirstUnitInRange(target.Position, StoneExtensions.StoneName, 500);
            var firstStoneNearHero = StoneExtensions.FirstUnitInRange(localHero.Position, StoneExtensions.StoneName, 250);

            // roll logic
            if (Roll.CanBeCasted())
            {
                var rudePredict = target.IsMoving ? target.InFront(150) : target.Position;
                var prediction = Smash.GetPrediction(target);
                var distToPredict = prediction.Distance2D(localHero.Position);

                if (firstStoneNearHero == null
                    && firstStoneNearEnemy == null
                    && !target.IsInvulnerable()
                    && !context.AutoPushInTowerDirection.CanPushInTower
                    && distToPredict < 600f
                    && Grip.CanBeCasted()
                    && Smash.CanBeCasted()
                    && Stone.UseAbility(localHero.Position.Extend(rudePredict, 100)))
                {
                    Blink?.Sleeper.Sleep(300);
                    Grip.Sleeper.Sleep(distToPredict / Grip.Speed * 1000);
                    Stone.Sleeper.Sleep(500);
                    Roll.Sleeper.Sleep(300 + distToPredict / Grip.Speed * 1000);
                    AbilitySleeper.Sleep(100);
                    return;
                }

                var stoneBetween = StoneExtensions.FirstUnitBetween(localHero.Position, rudePredict, StoneExtensions.StoneName, 250, 350);

                if (firstStoneNearEnemy != null
                    && firstStoneNearEnemy.Distance2D(localHero) < 1000f
                    && stoneBetween == null
                    && Grip.UseAbility(firstStoneNearEnemy.Position))
                {
                    Blink?.Sleeper.Sleep(300);
                    AbilitySleeper.Sleep(100);
                    return;
                }
                else if (firstStoneNearEnemy == null
                    && distToPredict < 900f
                    && !target.IsInvulnerable()
                    && stoneBetween == null
                    && !localHero.IsInRollPhase()
                    && Grip.CanBeCasted()
                    && Stone.UseAbility(rudePredict))
                {
                    Blink?.Sleeper.Sleep(300);
                    Stone.Sleeper.Sleep(500);
                    AbilitySleeper.Sleep(200);
                    return;
                }

                if (!localHero.IsInRollPhase()
                    && Roll.UseAbility(target))
                {
                    Smash.Sleeper.Sleep(1000);
                    Blink?.Sleeper.Sleep(300);
                    AbilitySleeper.Sleep(800);
                    return;
                }
            }

            var intersectingStone = EntityManager.GetEntities<Unit>()
                                              .FirstOrDefault(x => x.Distance2D(target) < Grip.Range
                                                      && x.Name == StoneExtensions.StoneName
                                                      && x.IsAlive
                                                      && StoneExtensions.FirstUnitBetween(localHero.Position, x.Position, target.Name, 200, Grip.Range) == target);

            if (intersectingStone != null
                && !target.IsInvulnerable()
                && Grip.UseAbility(intersectingStone.Position))
            {

                Smash.Sleeper.Sleep(intersectingStone.Distance2D(localHero) / Grip.Speed * 1100);
                AbilitySleeper.Sleep(200);
                return;
            }

            if (Orchid != null
                && Orchid.UseAbility(target))
            {
                AbilitySleeper.Sleep(100);
                return;
            }

            if (Halberd != null
                && Halberd.UseAbility(target))
            {
                AbilitySleeper.Sleep(100);
                return;
            }

            if (Shivas != null
                && Shivas.UseAbility(target))
            {
                AbilitySleeper.Sleep(100);
                return;
            }


            if (distanceToTarget < Smash.Range * 0.4f
                && !target.IsInvulnerable()
                && Smash.UseAbilityOnStone(target))
            {
                AbilitySleeper.Sleep(200);
                return;
            }

            if (distanceToTarget < Smash.Range * 0.4f
                && !localHero.IsInRollPhase()
                && !target.IsInvulnerable()
                && Grip.CanBeCasted()
                && Smash.CanBeCasted()
                && Stone.UseAbility())

            {
                Stone.Sleeper.Sleep(300);
                AbilitySleeper.Sleep(200);
                return;
            }

            if (distanceToTarget < Smash.Range * 0.4f
                && firstStoneNearHero == null
                && !target.IsInvulnerable()
                && (Grip.Base.Cooldown > Smash.Base.CooldownLength
                    || (!Grip.CanBeCasted() && Grip.Base.Cooldown == 0))
                && !localHero.IsInRollPhase()
                && Smash.CanBeCasted()
                && !context.AutoPushInTowerDirection.CanPushInTower
                && Stone.UseAbility())
            {
                Stone.Sleeper.Sleep(300);
                AbilitySleeper.Sleep(100);
                return;
            }

            if (SpiritVessel != null
                && SpiritVessel.UseAbility(target))
            {
                AbilitySleeper.Sleep(100);
                return;
            }
        }
    }
}
