using Verse;

namespace WeaponUtilityIII;

internal class ProjectileMissile : Projectile_Explosive
{
    protected override void Tick()
    {
        if (intendedTarget.Thing != null)
        {
            destination = intendedTarget.Thing.DrawPos;
        }

        base.Tick();
    }
}