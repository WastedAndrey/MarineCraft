using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct UnitStats
{
    public string unitName;
    public float healthMax;
    public float healthCurrent;

    public float landDamage;
    public float landAttackSpeed;
    public float landAttackDistance;
    public float landMissileSpeed;

    public float airDamage;
    public float airAttackSpeed;
    public float airAttackDistance;
    public float airMissileSpeed;

    public float defense;
    public float moveSpeed;


}
