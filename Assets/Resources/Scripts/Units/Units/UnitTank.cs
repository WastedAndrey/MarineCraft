using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTank : Unit
{
    Animator animator;

    protected override void Awake()
    {
        animator = this.GetComponent<Animator>();
    }

    protected override void MakeAttackGround()
    {
        base.MakeAttackGround();
        animator.Play("TankAttack", 0);
    }
}
