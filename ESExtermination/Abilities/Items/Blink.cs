using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Units;
using Divine.Extensions;
using Divine.Numerics;

namespace ESExtermination.Abilities.Items
{
    internal class Blink : ItemBase
    {
        public override float Range
        {
            get
            {
                return 1200 + Owner.BonusCastRange;
            }
        }
        public Blink(Ability baseAbility)
            : base(baseAbility)
        {

        }

        public override bool UseAbility(Vector3 pos)
        {
            var range = this.Range;

            if (Owner.Distance2D(pos) < range)
            {
                return base.UseAbility(pos);
            }

            return base.UseAbility(Owner.Position.Extend(pos, range));
        }

        public override bool UseAbility(Unit target)
        {
            return UseAbility(target.InFront(150));
        }

    }
}
