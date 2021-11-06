using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(SplitableObject))]
public class DeathZone : MonoBehaviour
{
    [SerializeField] Dimension.Color activeColor; // None if no restriction
    void OnTriggerEnter(Collider col)
    {
        if (activeColor != Dimension.Color.NONE && GetComponent<SplitableObject>().ObjectColor != activeColor)
            return;

        var player = col.GetComponent<PlayerController>();
        if (player != null)
        {
            Death(player);
        }
    }
    protected void Death(PlayerController player)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }
}
