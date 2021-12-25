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
        var so = col.GetComponent<SplittableObject>();
        if (so != null)
        {
            World.Instance.DeactivateObject(so);
        }
        else
        {
            GameObject.Destroy(col.gameObject);
        }
    }
    protected void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }
}
