using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Core;

[RequireComponent(typeof(Core.SplittableObject))]
public class DeathZone : MonoBehaviour
{
    [SerializeField] Dimension.Color activeColor; // None if no restriction
    void OnTriggerEnter(Collider col)
    {
        if (activeColor != Dimension.Color.NONE && GetComponent<Core.SplittableObject>().ObjectColor.Color != activeColor)
            return;

        var player = col.GetComponent<Character.PlayerController>();
        if (player != null)
        {
            Death(player);
        }
    }
    protected void Death(Character.PlayerController player)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }
}
