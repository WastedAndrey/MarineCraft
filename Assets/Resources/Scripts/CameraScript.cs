using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public float cameraSpeed = 10;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePCInput();
        UpdateMobileInput();
    }

    void UpdatePCInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            Vector3 pos = this.transform.position;
            pos.y += Time.deltaTime * cameraSpeed;
            this.transform.position = pos;

        }
        if (Input.GetKey(KeyCode.S))
        {
            Vector3 pos = this.transform.position;
            pos.y -= Time.deltaTime * cameraSpeed;
            this.transform.position = pos;
        }
        if (Input.GetKey(KeyCode.A))
        {
            Vector3 pos = this.transform.position;
            pos.x -= Time.deltaTime * cameraSpeed;
            this.transform.position = pos;
        }
        if (Input.GetKey(KeyCode.D))
        {
            Vector3 pos = this.transform.position;
            pos.x += Time.deltaTime * cameraSpeed;
            this.transform.position = pos;
        }

        this.GetComponent<Camera>().orthographicSize -= Input.mouseScrollDelta.y * Time.deltaTime * cameraSpeed;
    }

    private void UpdateMobileInput()
    {
        if (Swipe.SwipePhase == SwipePhase.Moved)
        {
            Vector2 pos = Swipe.VectorLastFrame;
            Vector3 newPos = this.transform.position - (Vector3)pos / 450 * cameraSpeed;
            newPos.x = Mathf.Clamp(newPos.x, 0, 80);
            newPos.y = Mathf.Clamp(newPos.y, -10, 70);
            this.transform.position = newPos;
        }
    }

    public void ZoomIn()
    {
        this.GetComponent<Camera>().orthographicSize -= this.GetComponent<Camera>().orthographicSize / 4;
    }

    public void ZoomOut()
    {
        this.GetComponent<Camera>().orthographicSize += this.GetComponent<Camera>().orthographicSize / 4;
    }
}
