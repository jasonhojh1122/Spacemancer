using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SkillController : MonoBehaviour
{
    [SerializeField] SkillGameUIController skillGameUIController;
    private void Update()
    {
        if (Input.GetButtonDown("Skill"))
        {
            skillGameUIController.gameObject.SetActive(true);
        }
        skillGameUIController.gameObject.SetActive(false);
    }
}
