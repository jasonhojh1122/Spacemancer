using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Core;

public class DeathZone : MonoBehaviour
{
    void OnTriggerEnter(Collider col)
    {
        Util.Debug.Log(gameObject, col.gameObject.name + " enter death zone.");
        if (col.gameObject.tag == "Player")
        {
            SceneLoader.Instance.Reload();
            return;
        }
    }

}
