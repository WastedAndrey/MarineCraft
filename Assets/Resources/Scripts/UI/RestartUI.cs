using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RestartUI : MonoBehaviour
{
    Button button;
    Level level;

    void Awake()
    {
        button = this.GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        if (level == null)
        {
            var levelObj = GameObject.FindGameObjectWithTag(TagManager.Level);
            if (levelObj != null && levelObj.GetComponent<Level>() != null)
            {
                level = levelObj.GetComponent<Level>();
                button.onClick.AddListener(level.Restart);
            }
                
        }
    }
}
