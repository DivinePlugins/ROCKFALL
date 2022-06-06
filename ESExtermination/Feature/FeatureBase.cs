using System;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Menu.Items;

namespace ESExtermination.Feature
{
    internal abstract class FeatureBase : IDisposable
    {
        private protected Context context;
        private protected Hero localHero;
        private protected Menu rootMenu;

        protected FeatureBase(Context context)
        {
            this.context = context;
            localHero = context.LocalHero;
            rootMenu = context.RootMenu;
        }

        public abstract void Start();

        public abstract void Dispose();
    }
}
