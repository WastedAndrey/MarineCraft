using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Explosion : MonoBehaviour
{
    public float lifeTime;

    // Start is called before the first frame update
    void Awake()
    {
        StartCoroutine(Die());
        ParticleSystem pSystem = this.GetComponent<ParticleSystem>();
        var pMain = pSystem.main;
        pMain.duration = lifeTime;
        pMain.startLifetimeMultiplier = lifeTime;
        pSystem.Play();
    }


    public void Init(Level level)
    {
        this.transform.SetParent(level.gameObject.transform); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Die()
    {
        yield return new WaitForSeconds(lifeTime);

        Destroy(this.gameObject);
    }
}
