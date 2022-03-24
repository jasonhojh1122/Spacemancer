using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Core;

public class DeathZone : MonoBehaviour
{
    public static bool pause = false;
    
    void OnTriggerEnter(Collider col)
    {
        if (!pause && col.gameObject.tag == "Player")
        {
            SceneLoader.Instance.Reload();
            return;
        }
    }

}
