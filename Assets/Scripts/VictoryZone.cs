using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


using Core;

[RequireComponent(typeof(Core.SplittableObject))]
public class VictoryZone : MonoBehaviour
{

    [SerializeField] AudioSource victoryAudio;
    [SerializeField] Dimension.Color activeColor; // None if no restriction
    [SerializeField] string nextSceneName;
    Core.SplittableObject splittableObject;

    private void Awake()
    {
        splittableObject = GetComponent<Core.SplittableObject>();
    }

    void OnTriggerEnter(Collider col)
    {
        if (activeColor != Dimension.Color.NONE && (splittableObject.Color != activeColor || !splittableObject.IsInCorrectDim()))
            return;

        var player = col.GetComponent<Character.PlayerController>();
        if (player != null)
        {
            Victory(player);
        }
    }

    protected void Victory(Character.PlayerController player)
    {
        victoryAudio.Play();
        if(nextSceneName != null)
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
