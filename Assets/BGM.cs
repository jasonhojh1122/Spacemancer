using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour
{

    [SerializeField] AudioSource bgm;
    public static bool bgmOn = true;
    private float savedVolume; 
    // Start is called before the first frame update
    void Start()
    {
        savedVolume = bgm.volume;
        bgm.ignoreListenerVolume = true;
        bgm.volume = bgmOn? savedVolume : 0;
    }
    
    public bool toggleBGM()
    {
        bgmOn = !bgmOn;
        bgm.volume = bgmOn ? savedVolume : 0;
        return bgmOn;
    }
}
