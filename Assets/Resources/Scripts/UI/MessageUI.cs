using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageUI : MonoBehaviour
{
    Animator animator;

    public float defaultMessageTime = 3;

    float hideTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hideTimer > 0)
        {
            hideTimer -= Time.deltaTime;
            if (hideTimer <= 0) animator.Play("MessageUIHide");
        }
    }

    private void recieveMessage(string text)
    {
        recieveMessage(text, defaultMessageTime);
    }
    private void recieveMessage(string text, float time)
    {
        animator.Play("MessageUIShow");
        this.GetComponent<Text>().text = text;
        hideTimer = time;
    }

    public static void RecieveMessage(string text)
    {
        var messageObjects = GameObject.FindGameObjectsWithTag(TagManager.Message);
        for (int i = 0; i < messageObjects.Length; i++)
        {
            if (messageObjects[i].GetComponent<MessageUI>() != null)
                messageObjects[i].GetComponent<MessageUI>().recieveMessage(text);
        }
    }

    public static void RecieveMessage(string text, float time)
    {
        var messageObjects = GameObject.FindGameObjectsWithTag(TagManager.Message);
        for (int i = 0; i < messageObjects.Length; i++)
        {
            if (messageObjects[i].GetComponent<MessageUI>() != null)
                messageObjects[i].GetComponent<MessageUI>().recieveMessage(text, time);
        }
    }
}
