using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintPageController : MonoBehaviour
{
    UnityEngine.UI.Button button;
    [SerializeField] UnityEngine.UI.Button hintButton;
    Canvas currentCanvas;
    MenuController_Paused pauseMenu;
    bool turnOnOnce = true;
    bool hintPageEnable = true;
    // Start is called before the first frame update
    void Start()
    {
        currentCanvas = GetComponentInChildren<Canvas>();
        InputManager.Instance.pause = true;
        pauseMenu = FindObjectOfType<MenuController_Paused>();
    }
    // Update is called once per frame
    void Update()
    {
        if(currentCanvas.enabled && turnOnOnce && hintPageEnable){
            turnOnOnce = false;
		    button = currentCanvas.GetComponentInChildren<UnityEngine.UI.Button>();
		    Debug.Log(button.name);
		    button.Select();
        }
        else{
            turnOnOnce = true;
        }
        if(pauseMenu.isPaused){
            ExitHintPage();
        }
    }
    public void EnableHintPage(){
        hintPageEnable = true;
        if(button == null){
            Debug.LogError("HintButton is null");
            hintPageEnable = false; // reset to previous state
            return;
        }
        InputManager.Instance.pause = true;
        hintButton.onClick.Invoke();
    }
    public void ExitHintPage(){
        hintPageEnable = false;
        //hintButton.gameObject.SetActive(true);
        if(!pauseMenu.isPaused)
            InputManager.Instance.pause = false;   
        else{
            for(int i = 0;i<transform.childCount;i++){
                transform.GetChild(i).gameObject.SetActive(false);
            }
            hintButton.gameObject.SetActive(true);            
        }            
    }
}
