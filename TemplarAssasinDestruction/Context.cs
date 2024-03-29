﻿using Divine.Menu.EventArgs;
using Divine.Menu.Items;

using TemplarAssasinDestruction.Abilities.Items;
using TemplarAssasinDestruction.Abilities.Spells;
using TemplarAssasinDestruction.Modes;

namespace TemplarAssasinDestruction
{
    class Context
    {
        public TemplarAssasin TemplarAssasin { get; set; }
        public PluginMenu PluginMenu { get; set; }
        public TargetManager TargetManager { get; set; }
        public SpellHelper SpellHelper { get; set; }
        public ItemHelper ItemHelper { get; set; }
        public Harass Harass { get; set; }
        public Combo Combo { get; set; }

        public Context()
        {
            PluginMenu = new PluginMenu();
        }

        public void Activate()
        {
            PluginMenu.PluginStatus.ValueChanged += PluginStatus_ValueChanged;
        }

        private void PluginStatus_ValueChanged(MenuSwitcher switcher, SwitcherChangedEventArgs e)
        {
            if (e.Value)
            {
                TemplarAssasin = new TemplarAssasin(this);
                TargetManager = new TargetManager(this);
                SpellHelper = new SpellHelper(this);
                ItemHelper = new ItemHelper(this);
                Harass = new Harass(this);
                Combo = new Combo(this);
            }
            else
            {
                TemplarAssasin.Dispose();
                TargetManager.Dispose();
                Harass.Dispose();
                Combo.Dispose();
                SpellHelper.Dispose();
                ItemHelper.Dispose();
            }
        }
    }
}
