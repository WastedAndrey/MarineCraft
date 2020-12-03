using EpPathFinding.cs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField]
    protected GameObject modelObj; // ссылка на объект с 3д моделью
    [SerializeField]
    protected GameObject hpBarObj; // ссылка на хп бар над головой
    [SerializeField]
    protected GameObject missileAnchor; // ссылка на объект, который нужен для поворота позиции вылета снарядов
    [SerializeField]
    protected GameObject missileSpawnPos; // ссылка на объект, из позиции которого будут вылетать снаряды
    [SerializeField]
    protected GameObject prefabUnitDeathExplosion; // префаб взрыва после смерти

    protected HPBar hpBar; // ссылка на скрипт хп бара

    protected Level level; // ссылка на уровень, к которому принадлежит юнит
    protected UnitAI unitAI; //ссылка на скрип ИИ

    public string unitName; // должно быть уникальным
    public UnitStats stats; // статы юнита, которые можно упаковать в Json
    public GameObject preafabLandMissile; // префаб снаряда. Если он есть, будет стрелять снарядами по наземным целям
    public GameObject prefabAirMissile; // префаб снаряда. Если он есть, будет стрелять снарядами по воздушным целям
    protected float landAttackTimer; // таймер кулдауна атаки
    protected float airAttackTimer; // таймер кулдауна атаки


    public UnitMovementType movementType; // тип передвижения
    public UnitAttackType attackType; // тип атаки
    public int team; // номер команды юнита. Разные команды враждебны
    public Color color; // цвет команды. Забрасывает его в модель 3д, а модель разбирается, что с ним делать

    public Unit targetOfAttack; // текущая цель атаки. если есть
    public float pathfindTimer; // таймер, по которому по мере приближения к цели переодически заново строит путь к ней
    public bool moving; // двигается или нет
    public float angle; // угол поворота

    public UnitEvent underAttack; // событие, что юнит под атакой
    public UnitEvent died; // событие, что юнит погиб

    public List<GridPos> path = new List<GridPos>(); // список точек движения
    public List<GameObject> missiles = new List<GameObject>(); // список ракет, которые создал этот объект

    /// <summary>
    /// Возвращает позицию объекта в 2д пространстве
    /// </summary>
    public Vector2 Position
    {
        get
        {
            return this.transform.position;
        }
       
    }

    /// <summary>
    /// Возвращает позицию объекта в 2д пространстве округленную до целых чисел
    /// </summary>
    public Vector2Int PositionInt
    {
        get
        {
            Vector2 pos = this.transform.position;
            Vector2Int positionInt = new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
            return positionInt;
        }
    }

    protected virtual void Awake()
    {
    }

    protected virtual void Start()
    {
    }

    protected virtual void Update()
    {
        UpdateAttack();
        UpdateMovement();
        UpdateChildren();
    }


    /// <summary>
    /// Inits unit. Must be callsed from Level before work with unit.
    /// </summary>
    /// <param name="level">Level owner of unit.</param>
    public virtual void Init(Level level)
    {
        this.level = level;
        this.stats = level.GetStats(unitName);
        this.hpBar = hpBarObj.GetComponent<HPBar>();
        this.hpBar.Init(this);
        this.unitAI = this.GetComponent<UnitAI>();
        if (unitAI != null) unitAI.Init(level, this);
        modelObj.GetComponent<ModelColor>().SetColor(color);
    }



    /// <summary>
    /// Makes unit to attack target.
    /// </summary>
    /// <param name="target">Target of attack.</param>
    public virtual void Attack(Unit target)
    {
        if (IsTargetAvailable(target))
            targetOfAttack = target;
    }

    /// <summary>
    /// Makes unit to stop attack.
    /// </summary>
    public void StopAttack()
    {
        targetOfAttack = null;
    }

    /// <summary>
    /// Makes unit to recieve damage.
    /// </summary>
    /// <param name="damage">Amount of damage</param>
    /// <returns>Item1 - final damage after defense, Item2 - is target dead after damage.</returns>
    public virtual (float, bool) RecieveDamage(float damage)
    {
        float finalDamage;
        bool dead = false;

        finalDamage = damage - stats.defense;
        stats.healthCurrent -= finalDamage;
        stats.healthCurrent = stats.healthCurrent < 0 ? 0 : stats.healthCurrent;

        if (stats.healthCurrent <= 0) 
        {
            Die();
            dead = true;
        }

        underAttack?.Invoke(this);

        return (finalDamage, dead);
    }

    /// <summary>
    /// Makes unit to die.
    /// </summary>
    public virtual void Die()
    {
        died?.Invoke(this);
        if (prefabUnitDeathExplosion != null)
        {
            GameObject explosion = Instantiate(prefabUnitDeathExplosion, this.transform.position, Quaternion.identity);
            explosion.GetComponent<Explosion>().Init(level);
        }
        AudioManager.Instance.PlayExplosion();

        Destroy(this.gameObject);
    }

    /// <summary>
    /// Makes unit to move to target position.
    /// </summary>
    /// <param name="position">Target position.</param>
    public virtual void MoveTo(Vector2Int position)
    {
        FindPath(movementType, position);
    }

    /// <summary>
    /// Makes unit to stop;
    /// </summary>
    public virtual void Stop()
    {
        moving = false;
    }

    /// <summary>
    /// Finds path to target point
    /// </summary>
    /// <param name="movementType"></param>
    /// <param name="position"></param>
    protected void FindPath(UnitMovementType movementType, Vector2Int position)
    {
        BaseGrid searchGrid = null; 
        switch (movementType)
        {
            case UnitMovementType.Ground:
                searchGrid = level.groundGrid; 
                break;
            case UnitMovementType.Air:
                searchGrid = level.airGrid;
                break;
            default:
                searchGrid = level.groundGrid;
                break;
        }


        GridPos startPos = MyMaths.Vector2ToGridPos(PositionInt);
        GridPos endPos = MyMaths.Vector2ToGridPos(position);
        JumpPointParam jpParam = new JumpPointParam(searchGrid, startPos, endPos, EndNodeUnWalkableTreatment.ALLOW, DiagonalMovement.IfAtLeastOneWalkable);
        jpParam.Reset(startPos, endPos);

        path = JumpPointFinder.FindPath(jpParam);
        if (path.Count > 1) path.RemoveAt(0);
        moving = true;
    }

    /// <summary>
    /// Instantly moves unit to target position.
    /// </summary>
    /// <param name="position">Target position.</param>
    public virtual void TeleportTo(Vector2Int position)
    {
        this.transform.position = (Vector2)position;
    }

    /// <summary>
    /// Tryes to move to target point
    /// </summary>
    protected virtual void UpdateMovement()
    {
        if (moving == false) return;
        if (path.Count == 0) return;

        Vector2 pos = this.gameObject.transform.position;
        Vector2 targetPos = MyMaths.GridPosToVector2(path[0]);
        Vector2 direction = (targetPos - pos).normalized;
        Vector2 newPos = (Vector2)this.gameObject.transform.position + direction * stats.moveSpeed * Time.deltaTime;
        this.gameObject.transform.position = newPos;
        angle = MyMaths.GetAngle(direction);

        if (Vector2.Distance(pos, targetPos) <= stats.moveSpeed * Time.deltaTime * 2)
        {
            this.gameObject.transform.position = MyMaths.GridPosToVector2(path[0]);
            path.RemoveAt(0);
        } 
        if (path.Count == 0) moving = false;
    }


    /// <summary>
    /// Tryes to attack target depending of distance and attack cooldown.
    /// </summary>
    protected virtual void UpdateAttack()
    {
        if (landAttackTimer > 0) landAttackTimer -= Time.deltaTime;
        if (airAttackTimer > 0) airAttackTimer -= Time.deltaTime;
        if (pathfindTimer > 0) pathfindTimer -= Time.deltaTime;

        if (targetOfAttack == null) return;

        switch (targetOfAttack.movementType)
        {
            case UnitMovementType.Ground:
                {
                    TryAttackGround();
                    break;
                }
               
            case UnitMovementType.Air:
                {
                    TryAttackAir();
                    break;
                }
            default: break;
        }  
    }

    protected virtual void TryAttackGround()
    {
        if (landAttackTimer > 0) return;

        float dist = Vector2.Distance(PositionInt, targetOfAttack.PositionInt);

        // If distance to target is more then distance of attack - tryes to move to target;
        if (dist > stats.landAttackDistance && pathfindTimer <= 0)
        {
            MoveTo(targetOfAttack.PositionInt);
            pathfindTimer = 1;
        }

        // If distance is ok - makes attack
        if (dist <= stats.landAttackDistance)
        {
            MakeAttackGround();
        }
    }

    protected virtual void MakeAttackGround()
    {
        Stop();

        landAttackTimer = 1f / stats.landAttackSpeed;
        Vector2 direction = targetOfAttack.Position - Position;
        angle = MyMaths.GetAngle(direction);

        if (preafabLandMissile == null)
            targetOfAttack.RecieveDamage(stats.landDamage);
        else
        {
            float angle = MyMaths.GetAngle(Position, targetOfAttack.Position);
            Quaternion quat = Quaternion.Euler(new Vector3(0, 0, angle));
            GameObject newMissile = Instantiate(preafabLandMissile, missileSpawnPos.transform.position, quat);
            missiles.Add(newMissile);
            newMissile.GetComponent<Missile>().Init(level, this, targetOfAttack, stats.landDamage, stats.landMissileSpeed);
        }
    }

    protected virtual void TryAttackAir()
    {
        if (airAttackTimer > 0) return;

        float dist = Vector2.Distance(PositionInt, targetOfAttack.PositionInt);

        // If distance to target is more then distance of attack - tryes to move to target;
        if (dist > stats.airAttackDistance && pathfindTimer <= 0)
        {
            MoveTo(targetOfAttack.PositionInt);
            pathfindTimer = 1;
        }

        // If distance is ok - makes attack
        if (dist <= stats.airAttackDistance)
        {
            MakeAttackAir();
        }
    }

    protected virtual void MakeAttackAir()
    {
        Stop();

        airAttackTimer = 1f / stats.airAttackSpeed;
        Vector2 direction = targetOfAttack.Position - Position;
        angle = MyMaths.GetAngle(direction);

        if (prefabAirMissile == null)
            targetOfAttack.RecieveDamage(stats.airDamage);
        else
        {
            float angle = MyMaths.GetAngle(Position, targetOfAttack.Position);
            Quaternion quat = Quaternion.Euler(new Vector3(0, 0, angle));
            GameObject newMissile = Instantiate(prefabAirMissile, missileSpawnPos.transform.position, quat);
            missiles.Add(newMissile);
            newMissile.GetComponent<Missile>().Init(level, this, targetOfAttack, stats.airDamage, stats.airMissileSpeed);
        }
    }

    

    /// <summary>
    /// Updates unit's 3d model and missile anchor angle
    /// </summary>
    protected void UpdateChildren()
    {
        modelObj.transform.rotation = Quaternion.Euler(0, 0, angle);
        missileAnchor.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    /// <summary>
    /// Returns true, if target type is available for attack of this unit
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    public bool IsTargetAvailable(Unit unit)
    {
        bool result = false;

        if (attackType == UnitAttackType.Both)
            return true;

        if (attackType == UnitAttackType.Ground && unit.movementType == UnitMovementType.Ground)
            return true;

        if (attackType == UnitAttackType.Air && unit.movementType == UnitMovementType.Air)
            return true;

        return result;
    }
}
