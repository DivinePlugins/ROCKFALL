﻿using Rock.Rubick.SpellStillManager.Modes;

using System.Linq;

namespace RockRubick
{
    internal sealed class NonAghanimMode : StealModeBase
    {
        public override void ExecuteLogic()
        {
            ClearingUseless();
            base.ExecuteLogic();
        }

        private void ClearingUseless()
        {
            for (int i = 0; i < Dictionaries.LastSpell.Count; i++)
            {
                var element = Dictionaries.LastSpell.ElementAtOrDefault(i);

                if (!Dictionaries.SpellList.ContainsKey(element.Value))
                {
                    Dictionaries.LastSpell.Remove(element.Key);
                }
                else if (Dictionaries.SpellList.Where(x => x.Key == element.Value).FirstOrDefault().Value <= 2)
                {
                    Dictionaries.LastSpell.Remove(element.Key);
                }

                if (i + 1 > Dictionaries.LastSpell.Count)
                {
                    break;
                }
            }
        }
    }
}
