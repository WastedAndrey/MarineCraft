using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public float offsetZ;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            Vector3 pos = this.transform.position;
            pos.y += Time.deltaTime * 5;
            this.transform.position = pos;

        }
        if (Input.GetKey(KeyCode.S))
        {
            Vector3 pos = this.transform.position;
            pos.y -= Time.deltaTime * 5;
            this.transform.position = pos;
        }
        if (Input.GetKey(KeyCode.A))
        {
            Vector3 pos = this.transform.position;
            pos.x -= Time.deltaTime * 5;
            this.transform.position = pos;
        }
        if (Input.GetKey(KeyCode.D))
        {
            Vector3 pos = this.transform.position;
            pos.x += Time.deltaTime * 5;
            this.transform.position = pos;
        }

        this.GetComponent<Camera>().orthographicSize -= Input.mouseScrollDelta.y * Time.deltaTime * 5;
    }
}
