using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
public class MainMenu : MonoBehaviour
{
    [SerializeField] PlayableDirector director;
    [SerializeField] UnityEngine.EventSystems.EventSystem eventSystem;
    [SerializeField] Button continueButton;
    [SerializeField] Button newGameButton;
    [SerializeField] Button exitButton;
    [SerializeField] int stabilizer1BuildId;
    [SerializeField] int stabilizer2BuildId;

    private void Start()
    {
        if (Saving.GameSaveManager.Instance.IsNewGame)
        {
            continueButton.gameObject.SetActive(false);
            Navigation navigation = newGameButton.navigation;
            navigation.selectOnUp = exitButton;
            navigation.selectOnDown = exitButton;
            newGameButton.navigation = navigation;

            navigation = exitButton.navigation;
            navigation.selectOnUp = newGameButton;
            navigation.selectOnDown = newGameButton;
            exitButton.navigation = navigation;

            eventSystem.SetSelectedGameObject(newGameButton.gameObject);
        }
        else
        {
            eventSystem.SetSelectedGameObject(continueButton.gameObject);
        }
        director.Play();
    }

    public void ContinueGame()
    {
        var highestScene = Saving.GameSaveManager.Instance.GameSave.highestScene;
        var phase = Saving.GameSaveManager.Instance.GameSave.phase;
        if ( (highestScene == stabilizer1BuildId && phase == 1) ||
                (highestScene == stabilizer2BuildId && phase == 2) )
        {
            SceneLoader.Instance.Load("CenterLab");
        }
        SceneLoader.Instance.Load(Saving.GameSaveManager.Instance.GameSave.highestScene);
    }

    public void NewGame()
    {
        Saving.GameSaveManager.Instance.NewGame();
        SceneLoader.Instance.Load("StartAnimation");
    }
}