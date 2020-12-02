using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnit
{
    /// <summary>
    /// Inits unit. Must be callsed from Level before work with unit.
    /// </summary>
    /// <param name="level">Level owner of unit.</param>
    void Init(Level level);

    /// <summary>
    /// Makes unit to recieve damage.
    /// </summary>
    /// <param name="damage">Amount of damage</param>
    /// <returns>Item1 - final damage after defense, Item2 - is target dead after damage.</returns>
    (float, bool) RecieveDamage(float damage);

    /// <summary>
    /// Makes unit to attack target.
    /// </summary>
    /// <param name="target">Target of attack.</param>
    void Attack(IUnit target);

    /// <summary>
    /// Makes unit to stop attack.
    /// </summary>
    void StopAttack();

    /// <summary>
    /// Makes unit to move to target position.
    /// </summary>
    /// <param name="position">Target position.</param>
    void MoveTo(Vector2Int position);

    /// <summary>
    /// Instantly moves unit to target position.
    /// </summary>
    /// <param name="position">Target position.</param>
    void TeleportTo(Vector2Int position);

    /// <summary>
    /// Makes unit to die.
    /// </summary>
    void Die();

    /// <summary>
    /// Returns integer unit position.
    /// </summary>
    /// <returns></returns>
    Vector2Int Position();
   
}
