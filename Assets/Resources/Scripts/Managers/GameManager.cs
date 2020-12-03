using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Класс игрового менеджера. Правда тут ему особо нечем управлять. А так он мог бы управлять сценами и всякиим таким
/// </summary>
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    private AudioManager audioManager;
    public AudioManager Audio { get { return audioManager; } }
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameManager>();
            }

            return _instance;
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (this.GetComponent<AudioManager>() == null) this.gameObject.AddComponent<AudioManager>();
        audioManager = this.GetComponent<AudioManager>();

        if (this.GetComponent<AudioSource>() == null) this.gameObject.AddComponent<AudioSource>();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

}
