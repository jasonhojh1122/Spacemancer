using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Core;

public class SkillGameUIController : MonoBehaviour
{
    [SerializeField] public UnityEngine.UI.Button RedButton;
    [SerializeField] public UnityEngine.UI.Button GreenButton;
    [SerializeField] public UnityEngine.UI.Button BlueButton;
    // Start is called before the first frame update
    Dimension.Color skillColor = Dimension.Color.NONE;
    private void Awake()
    {
    }
}
