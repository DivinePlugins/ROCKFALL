﻿namespace DawnBreaker_SkillHelper
{
    class Context
    {
        public MenuSettings Menu { get; set; }
        public Dawnbreaker Dawnbreaker { get; set; }
        public StarbreakerHelper StarbreakerHelper { get; set; }
        public HammerHelper HammerHelper { get; set; }

        public Context()
        {
            Dawnbreaker = new Dawnbreaker();
            Menu = new MenuSettings(this);
            StarbreakerHelper = new StarbreakerHelper(this);
            HammerHelper = new HammerHelper(this);
        }

        internal void Dispose()
        {
            Dawnbreaker.Dispose();
            Menu.Dispose();
            StarbreakerHelper.Dispose();
            HammerHelper.Dispose();
        }
    }
}
