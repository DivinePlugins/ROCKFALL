namespace ESExtermination;

using System;
using System.Linq;

using Divine.Entity;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;
using Divine.Game;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Numerics;
using Divine.Particle;
using Divine.Update;

internal class TargetSelector : IDisposable
{
    public static Hero CurrentTarget { get; set; }

    private Context context;
    private Hero localHero;

    public TargetSelector(Context context)
    {
        this.context = context;
        localHero = context.LocalHero;
    }

    public void Start()
    {
        context.ComboKey.ValueChanged += ComboKey_ValueChanged;
    }

    public void Dispose()
    {
        ParticleManager.DestroyParticle("TargetParticle");
        context.ComboKey.ValueChanged -= ComboKey_ValueChanged;
        UpdateManager.DestroyIngameUpdate(TargetUpdater);
    }

    private void ComboKey_ValueChanged(MenuHoldKey holdKey, HoldKeyChangedEventArgs e)
    {
        if (e.Value)
        {
            UpdateManager.CreateIngameUpdate(25, TargetUpdater);
        }
        else
        {
            CurrentTarget = null;
            ParticleManager.DestroyParticle("TargetParticle");
            UpdateManager.DestroyIngameUpdate(TargetUpdater);
        }
    }

    private Hero GetNearestToMouse()
    {
        var mousePos = GameManager.MousePosition;

        Hero target = EntityManager.GetEntities<Hero>()
            .Where(x => x.Distance2D(mousePos) < 600
                    && x.IsAlive
                    && (!x.IsIllusion || x.HasModifier("modifier_morphling_replicate"))
                    && x.IsEnemy(context.LocalHero)
                    && x.IsVisible)
            .OrderBy(x => x.Distance2D(mousePos))
            .FirstOrDefault();

        return target;
    }

    private void TargetUpdater()
    {
        CurrentTarget = GetNearestToMouse();

        if (CurrentTarget == null)
        {
            if (!context.LockedTarget.Value)
            {
                UpdateManager.DestroyIngameUpdate(TargetParticleUpdater);
            }

            ParticleManager.DestroyParticle("TargetParticle");
        }
        else
        {
            if (context.LockedTarget.Value)
            {
                UpdateManager.DestroyIngameUpdate(TargetUpdater);
                UpdateManager.CreateIngameUpdate(25, TargetParticleUpdater);

            }

            ParticleManager.CreateTargetLineParticle("TargetParticle", localHero, CurrentTarget.Position, Color.Aqua);
        }
    }

    private void TargetParticleUpdater()
    {
        if (CurrentTarget == null)
        {
            ParticleManager.DestroyParticle("TargetParticle");
        }
        else
        {
            ParticleManager.CreateTargetLineParticle("TargetParticle", localHero, CurrentTarget.Position, Color.Aqua);
        }
    }
}