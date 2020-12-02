using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAI : MonoBehaviour
{
    Level level;
    Unit unit;

    public float FindEnemyCooldown = 2;
    float findEnemyTimer = 0;


    private void Update()
    {
        FindEnemies();
    }

    public void Init(Level level, Unit unit)
    {
        this.level = level;
        this.unit = unit;
    }

    void FindEnemies()
    {
        if (findEnemyTimer > 0)
        {
            findEnemyTimer -= Time.deltaTime;
            return;
        }
        

        findEnemyTimer = FindEnemyCooldown;
        float dist = -1;
        Unit target = null;
        //Проходит по словарю с юнитами разных команд(свою команду не перебирает). Перебирает их и находит ближайшего.
        foreach (KeyValuePair<int, List<Unit>> team in level.units)
        {
            if (team.Key != unit.team)
            {
                for (int i = 0; i < team.Value.Count; i++)
                {
                    float newDist = Vector2.Distance(team.Value[i].Position, unit.Position);
                    if (unit.IsTargetAvailable(team.Value[i]) &&
                        (newDist < dist || dist < 0))
                    {
                        dist = newDist;
                        target = team.Value[i];
                    }
                }
            }
        }

        if (target != null)
            unit.Attack(target);
    }
}
