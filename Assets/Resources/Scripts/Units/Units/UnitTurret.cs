using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTurret : Unit
{
    /// <summary>
    /// Турельки не умеют бегать
    /// </summary>
    /// <param name="position">Target position.</param>
    public override void MoveTo(Vector2Int position)
    {
        
    }

    protected override void TryAttackGround()
    {
        if (landAttackTimer > 0) return;

        float dist = Vector2.Distance(PositionInt, targetOfAttack.PositionInt);

        // If distance is ok - makes attack
        if (dist <= stats.landAttackDistance)
        {
            MakeAttackGround();
        }
    }

    protected override void TryAttackAir()
    {
        if (airAttackTimer > 0) return;

        float dist = Vector2.Distance(PositionInt, targetOfAttack.PositionInt);

        // If distance is ok - makes attack
        if (dist <= stats.airAttackDistance)
        {
            MakeAttackAir();
        }
    }
}
