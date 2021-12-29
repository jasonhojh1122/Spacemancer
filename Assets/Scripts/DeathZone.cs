using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Core;

public class DeathZone : MonoBehaviour
{
    void OnTriggerEnter(Collider col)
    {
        Debug.Log("Death Zone" + col.gameObject.name);
        if (col.gameObject.tag == "Player")
        {
            ReloadLevel();
            return;
        }
    }
    protected void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }
}
