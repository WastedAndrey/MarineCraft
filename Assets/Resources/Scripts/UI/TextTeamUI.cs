using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextTeamUI : MonoBehaviour
{
    public int teamNumber;

    Text text;
    Level level;

    void Awake()
    {
        text = this.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (level == null)
        {
            var levelObj = GameObject.FindGameObjectWithTag(TagManager.Level);
            if (levelObj != null && levelObj.GetComponent<Level>() != null)
                level = levelObj.GetComponent<Level>();
        }

        if (level != null)
        {
            text.text = level.GetUnitsCount(teamNumber).ToString();
        }
    }
}
