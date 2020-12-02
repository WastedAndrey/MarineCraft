using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject missile;
    public float damage;
    public float attackSpeed;
    public float attackDistance;
    public float attacking;


    float nextAttackTimer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (nextAttackTimer > 0) nextAttackTimer -= Time.deltaTime;
    }
}
