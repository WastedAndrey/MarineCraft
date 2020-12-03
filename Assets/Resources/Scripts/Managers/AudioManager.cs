using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    List<AudioClip> clips = new List<AudioClip>();

    private static AudioManager _instance;

    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<AudioManager>();
            }

            return _instance;
        }
    }

    public void PlayExplosion()
    {
        this.GetComponent<AudioSource>().PlayOneShot(clips[0]);
    }

    public void PlayWin()
    {
        this.GetComponent<AudioSource>().PlayOneShot(clips[1]);
    }

    public void PlayLoose()
    {
        this.GetComponent<AudioSource>().PlayOneShot(clips[2]);
    }

   
}
