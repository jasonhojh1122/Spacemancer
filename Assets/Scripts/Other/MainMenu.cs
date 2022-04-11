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
    [SerializeField] Button unlockButton;
    [SerializeField] int stabilizer1BuildId;
    [SerializeField] int stabilizer2BuildId;

    bool loading;

    private void Awake()
    {
        loading = false;
    }

    private void Start()
    {
        if (Saving.GameSaveManager.Instance.IsNewGame)
        {
            continueButton.gameObject.SetActive(false);

            Navigation navigation = newGameButton.navigation;
            navigation.selectOnUp = unlockButton;
            navigation.selectOnDown = exitButton;
            newGameButton.navigation = navigation;

            navigation = unlockButton.navigation;
            navigation.selectOnUp = exitButton;
            navigation.selectOnDown = newGameButton;
            unlockButton.navigation = navigation;

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
        if (loading)
            return;
        loading = true;
        var highestScene = Saving.GameSaveManager.Instance.GameSave.highestScene;
        var phase = Saving.GameSaveManager.Instance.GameSave.phase;
        if ( (highestScene == stabilizer1BuildId && phase == 1) ||
                (highestScene == stabilizer2BuildId && phase == 2) )
        {
            SceneLoader.Instance.Load("CenterLab");
        }
        else
        {
            SceneLoader.Instance.Load(Saving.GameSaveManager.Instance.GameSave.highestScene);
        }
    }

    public void NewGame()
    {
        if (loading)
            return;
        loading = true;
        Saving.GameSaveManager.Instance.NewGame();
        SceneLoader.Instance.Load("StartAnimation");
    }

    public void UnlockAll()
    {
        Saving.GameSaveManager.Instance.GameSave.highestScene = stabilizer2BuildId;
        Saving.GameSaveManager.Instance.GameSave.phase = 2;
        Saving.GameSaveManager.Instance.Save();
        SceneLoader.Instance.Load("CenterLab");
    }

}