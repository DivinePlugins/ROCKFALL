﻿using Divine.Entity;
using Divine.Update;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;
using Divine.Entity.Entities.Units.Heroes;
using System;
using TemplarAssasinDestruction.Abilities.Items;
using TemplarAssasinDestruction.Abilities.Spells;
using Divine.Entity.Entities.Abilities.Items.Components;

namespace TemplarAssasinDestruction
{
    internal class TemplarAssasin : IDisposable
    {
        public Hero LocalHero { get; private set; }

        #region Spells
        public Refraction Refraction { get; private set; }
        public Meld Meld { get; private set; }
        public Trap Trap { get; private set; }
        public PsionicProj PsionicProj { get; private set; }
        public PsionicTrap PsionicTrap { get; private set; }
        #endregion

        #region Items
        public BlackKingBar BlackKingBar { get; private set; }
        public Blink Blink { get; private set; }
        public Manta Manta { get; private set; }
        public Hex Hex { get; private set; }
        public Orchid Orchid { get; private set; }
        public Nullifier Nullifier { get; private set; }
        #endregion

        private Context Context;

        public TemplarAssasin(Context context)
        {
            Context = context;
            LocalHero = EntityManager.LocalHero;

            var spellBook = LocalHero.Spellbook;
            Refraction = new Refraction(spellBook.GetSpellById(AbilityId.templar_assassin_refraction), Context);
            Meld = new Meld(spellBook.GetSpellById(AbilityId.templar_assassin_meld), Context);
            Trap = new Trap(spellBook.GetSpellById(AbilityId.templar_assassin_trap), Context);
            PsionicProj = new PsionicProj(spellBook.GetSpellById(AbilityId.templar_assassin_trap_teleport), Context);
            PsionicTrap = new PsionicTrap(spellBook.GetSpellById(AbilityId.templar_assassin_psionic_trap), Context);

            UpdateManager.CreateIngameUpdate(1000, ItemChecker);
        }

        public void Dispose()
        {
            UpdateManager.DestroyIngameUpdate(ItemChecker);
        }

        private bool IsBlink(Item item)
        {
            return item.ItemId == ItemId.item_blink
                || item.ItemId == ItemId.item_swift_blink
                || item.ItemId == ItemId.item_overwhelming_blink
                || item.ItemId == ItemId.item_arcane_blink;
        }

        private bool IsOrchid(Item item)
        {
            return item.ItemId == ItemId.item_orchid
                || item.ItemId == ItemId.item_bloodthorn;
        }

        private void ItemChecker()
        {
            foreach (var item in LocalHero.Inventory.MainItems)
            {
                if (IsBlink(item))
                    Blink = new Blink(item, Context);
                if (IsOrchid(item))
                    Orchid = new Orchid(item, Context);
                if (item.ItemId == ItemId.item_black_king_bar && BlackKingBar == null)
                    BlackKingBar = new BlackKingBar(item, Context);
                if (item.ItemId == ItemId.item_manta && Manta == null)
                    Manta = new Manta(item, Context);
                if (item.ItemId == ItemId.item_sheepstick && Hex == null)
                    Hex = new Hex(item, Context);
                if (item.ItemId == ItemId.item_nullifier && Nullifier == null)
                    Nullifier = new Nullifier(item, Context);



            }
        }


    }
}
