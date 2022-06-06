using System.Collections.Generic;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;

namespace ESExtermination.Extensions
{
    internal static class HeroExtensions
    {
        public static HashSet<string> InvulnerableModifiers = new HashSet<string>
        {
            "modifier_eul_cyclone",
            "modifier_wind_waker",
            "modifier_bane_nightmare_invulnerable",
            "modifier_juggernaut_omnislash_invulnerability",
            "modifier_obsidian_destroyer_astral_imprisonment_prison",
            "modifier_riki_tricks_of_the_trade_phase",
            "modifier_tusk_snowball_movement",
            "modifier_invoker_tornado",
            "modifier_puck_phase_shift",
            "naga_siren_song_of_the_siren",
            "modifier_void_spirit_dissimilate_phase"
        };

        public static bool IsInRollPhase(this Hero hero)
        {
            return hero.HasModifier("modifier_earth_spirit_rolling_boulder_caster");
        }

        public static bool IsEnchanted(this Hero hero)
        {
            return hero.HasModifier("modifier_earthspirit_petrify");
        }

        public static bool IsMagnetized(this Hero hero)
        {
            return hero.HasModifier("modifier_earth_spirit_magnetize");
        }
    }
}
