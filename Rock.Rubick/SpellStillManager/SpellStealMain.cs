using Divine.Extensions;
using Divine.Update;

using Rock.Rubick.SpellStillManager.Modes;

namespace RockRubick
{
    internal sealed class SpellStealMain
    {
        private StealModeBase _stealMode;

        public SpellStealMain()
        {
            new Dictionaries();
            new LastSpellManager();
            new CooldownManager();

            UpdateManager.CreateIngameUpdate(25, IngameUpdate);
        }


        public void Dispose()
        {
            LastSpellManager.Dispose();
            CooldownManager.Dispose();

            UpdateManager.DestroyIngameUpdate(IngameUpdate);
        }

        private void IngameUpdate()
        {
            if (General.localHero.HasAghanimsScepter() && !(_stealMode is AghanimMode))
            {
                _stealMode = new AghanimMode();
            }
            else if (!(_stealMode is NonAghanimMode))
            {
                _stealMode = new NonAghanimMode();
            }
            _stealMode.ExecuteLogic();
        }
    }
}

