﻿using Divine.Entity;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Spells;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;
using Divine.Game;
using Divine.Particle;
using Divine.Particle.EventArgs;
using Divine.Update;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RockRubick
{
    internal sealed class LastSpellManager
    {

        public LastSpellManager()
        {
            UpdateManager.CreateIngameUpdate(50, InGameUpdate);
            ParticleManager.ParticleAdded += ParticleSpecific;

        }


        public static void Dispose()
        {
            UpdateManager.DestroyGameUpdate(InGameUpdate);
            ParticleManager.ParticleAdded -= ParticleSpecific;
        }

        private static void AddSpecific(Hero hero, AbilityId abilityId)
        {
            if (!Dictionaries.LastSpell.ContainsKey(hero))
            {
                Dictionaries.LastSpell.Add(hero, abilityId);
            }
            else
            {
                Dictionaries.LastSpell.Remove(hero);
                Dictionaries.LastSpell.Add(hero, abilityId);
            }
        }

        private static void ParticleSpecific(ParticleAddedEventArgs e)
        {
            var particle = e.Particle;

            if (particle.Owner == General.localHero)
            {
                return;
            }


            Hero hero = null;
            if (particle.Owner is Hero == false)
            {
                return;
            }
            hero = particle.Owner as Hero;
            if (!hero.IsEnemy(General.localHero))
            {
                return;
            }

            if (particle.Name == "particles/units/heroes/hero_earthshaker/earthshaker_echoslam_start.vpcf")
            {
                AddSpecific(hero, AbilityId.earthshaker_echo_slam);
                return;
            }

            if (particle.Name == "particles/units/heroes/hero_void_spirit/aether_remnant/void_spirit_aether_remnant_pre.vpcf")
            {
                AddSpecific(hero, AbilityId.void_spirit_aether_remnant);
                return;
            }

            if (particle.Name == "particles/units/heroes/hero_shredder/shredder_whirling_death.vpcf")
            {
                AddSpecific(hero, AbilityId.shredder_whirling_death);
                return;
            }
            if (particle.Name == "particles/units/heroes/hero_pangolier/pangolier_swashbuckler_dash.vpcf")
            {

                AddSpecific(hero, AbilityId.pangolier_swashbuckle);
                return;
            }
            if (particle.Name == "particles/units/heroes/hero_pangolier/pangolier_tailthump_cast.vpcf")
            {

                AddSpecific(hero, AbilityId.pangolier_shield_crash);
                return;
            }
            if (particle.Name == "particles/units/heroes/hero_skywrath_mage/skywrath_mage_concussive_shot_cast.vpcf")
            {
                AddSpecific(hero, AbilityId.skywrath_mage_concussive_shot);
                return;
            }
            if (particle.Name == "particles/units/heroes/hero_pudge/pudge_rot.vpcf")
            {
                AddSpecific(hero, AbilityId.pudge_rot);
                return;
            }

        }

        private static void InGameUpdate()
        {
            var main = General.localHero.Spellbook.Spell4;
            foreach (var enemy in EntityManager.GetEntities<Hero>().Where(x => x.IsEnemy(General.localHero) && !x.IsIllusion && x.Spellbook.Spells.Any(y => y.IsInAbilityPhase)))
            {

                Spell ability = enemy.Spellbook.Spells.Where(x => x.IsInAbilityPhase).FirstOrDefault();

                if (enemy == null || ability == null || main == ability || !enemy.IsVisible)
                {
                    continue;
                }
                var e = enemy;
                var cp = ability.GetCastPoint();
                //if (!enemy.Spellbook.Spells.Any(x => Dictionaries.LastSpell.ContainsKey(enemy)))
                {
                    if (cp * 1000 > 50)
                    {
                        Task.Run(async () =>
                        {
                            await Task.Delay((int)(cp * 1000 - 75));
                            if (ability.IsInAbilityPhase)
                            {
                                Dictionaries.LastSpell.Remove(enemy);
                                Dictionaries.LastSpell.Add(enemy, ability.Id);
                            }
                        });
                    }
                    else
                    {
                        Dictionaries.LastSpell.Remove(enemy);
                        Dictionaries.LastSpell.Add(enemy, ability.Id);
                    }

                }
            }

            for (int i = 0; i < Dictionaries.LastSpell.Count; i++)
            {
                var element = Dictionaries.LastSpell.ElementAtOrDefault(i);
                try
                {
                    if (Dictionaries.RubickSpellCD.ContainsKey(element.Value))
                    {
                        Dictionaries.LastSpell.Remove(element.Key);
                        Dictionaries.Removed.Add((element.Key, element.Value), Dictionaries.RubickSpellCD.Where(x => x.Key == element.Value).FirstOrDefault().Value);
                    }
                }
                catch (ArgumentException)
                {
                    //TODO
                }
            }

            for (int i = 0; i < Dictionaries.Removed.Count; i++)
            {
                var element = Dictionaries.Removed.ElementAtOrDefault(i);
                if (Dictionaries.LastSpell.ContainsKey(element.Key.Item1))
                {
                    Dictionaries.Removed.Remove(element.Key);
                    continue;
                }
                else
                {
                    if (GameManager.GameTime > element.Value)
                    {
                        Dictionaries.LastSpell.Add(element.Key.Item1, element.Key.Item2);
                        Dictionaries.Removed.Remove(element.Key);
                    }
                }
                if (i + 1 >= Dictionaries.Removed.Count)
                {
                    break;
                }
            }

        }
    }
}
