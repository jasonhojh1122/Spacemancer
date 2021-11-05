using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SplitableObject))]
public class VictoryZone : MonoBehaviour
{

    [SerializeField] AudioSource victoryAudio;
    [SerializeField] Dimension.Color activeColor; // None if no restriction
    void OnTriggerEnter(Collider col)
    {
        if (activeColor != Dimension.Color.NONE && GetComponent<SplitableObject>().ObjectColor != activeColor)
            return;

        var player = col.GetComponent<PlayerController>();
        if (player != null)
        {
            Victory(player);
        }
    }
    protected void Victory(PlayerController player)
    {
        player.controlEnabled = false;
        victoryAudio.Play();
    }
}
