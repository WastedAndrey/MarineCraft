using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField]
    GameObject prefabExplosion;

    Level level;
    public Unit owner;
    public Unit target;
    public float damage;
    public float speed;

    // Start is called before the first frame update
    protected void Start()
    {
        
    }

    // Update is called once per frame
    protected void Update()
    {
        UpdateTarget();
        UpdateMovement();
        UpdateCollision();
    }

    public void Init(Level level, Unit owner, Unit target, float damage, float speed)
    {
        this.level = level;
        this.owner = owner;
        this.target = target;
        this.damage = damage;
        this.speed = speed;

        this.transform.SetParent(level.gameObject.transform);

    }

    protected void UpdateTarget()
    {
        if (target == null) Destroy(this.gameObject);
    }

    protected void UpdateMovement()
    {
        if (target == null) return;

        Vector2 dir = (target.Position - (Vector2)this.transform.position).normalized;
        this.transform.position = this.transform.position + (Vector3)dir * speed * Time.deltaTime;

        float angle = MyMaths.GetAngle(this.transform.position, target.Position);
        Quaternion quat = Quaternion.Euler(new Vector3(0, 0, angle));
        this.transform.rotation = quat;

    }

    protected void UpdateCollision()
    {
        if (target == null) return;

        float dist = Vector2.Distance(target.Position, this.transform.position);
        if (dist < 0.1f)
        {
            target.RecieveDamage(damage);

            if (prefabExplosion != null)
            {
                var newExplosion = Instantiate(prefabExplosion, this.transform.position, Quaternion.identity);
                newExplosion.GetComponent<Explosion>().Init(level);
            }
               

            Destroy(this.gameObject);
        }
    }
}
