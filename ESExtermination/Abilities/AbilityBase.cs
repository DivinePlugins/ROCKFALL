using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;
using Divine.Helpers;
using Divine.Numerics;

namespace ESExtermination.Abilities
{
    internal abstract class AbilityBase
    {
        public Ability Base { get; }
        public Hero Owner { get; }

        public Sleeper Sleeper { get; } = new Sleeper();

        public virtual string DebuffName { get; }
        public virtual float Speed { get; } = float.MaxValue;
        public virtual float Radius { get; } = 0;
        public virtual float Range => Base.CastRange;
        public virtual bool IsIgnoringBkb { get; } = false;

        protected AbilityBase(Ability baseAbility)
        {
            Base = baseAbility;
            Owner = Base.Owner as Hero;
        }

        virtual public bool UseAbility()
        {
            if (!CanBeCasted())
            {
                return false;
            }

            Base.Cast();
            return true;
        }

        virtual public bool UseAbility(Vector3 pos)
        {
            if (!CanBeCasted())
            {
                return false;
            }

            if (!CanHit(pos))
            {
                return false;
            }

            Base.Cast(pos);
            return true;
        }

        virtual public bool UseAbility(Unit target)
        {
            if (!CanBeCasted())
            {
                return false;
            }

            if (!CanHit(target))
            {
                return false;
            }

            Base.Cast(target);
            return true;
        }

        public virtual bool CanHit(Vector3 postition)
        {
            if (Owner.Distance2D(postition) > Range)
            {
                return false;
            }

            return true;
        }

        public virtual bool CanHit(Unit target)
        {
            if (target.IsInvulnerable())
            {
                return false;
            }

            if (target.IsMagicImmune() && !IsIgnoringBkb)
            {
                return false;
            }

            return CanHit(target.Position);
        }

        virtual public bool CanBeCasted()
        {
            if (Sleeper.Sleeping)
            {
                return false;
            }

            if (Base.ManaCost > Owner.Mana)
            {
                return false;
            }

            if (Base.Cooldown != 0)
            {
                return false;
            }

            if (Base.Charges > 0 && Base.CurrentCharges == 0
                || ((Base.Id == AbilityId.item_urn_of_shadows || Base.Id == AbilityId.item_spirit_vessel) && Base.CurrentCharges == 0))
            {
                return false;
            }

            if (Owner.IsStunned() || Owner.IsHexed() || Owner.IsMuted())
            {
                return false;
            }

            return true;

        }
    }
}
