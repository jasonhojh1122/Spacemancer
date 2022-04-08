using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] UI.CanvasGroupFader sceneTransition;

    static SceneLoader _instance;
    AsyncOperation async = null;

    public static SceneLoader Instance
    {
        get => _instance;
    }

    private void Awake()
    {
        if (_instance != null)
            Debug.LogError("Multiple instance of SceneLoader created");
        _instance = this;
    }

    /// <summary>
    /// Loads the scene with the given name.
    /// </summary>
    /// <param name="name"> The name of the scene to be loaded. </param>
    public void Load(string name)
    {
        sceneTransition.FadeIn();
        StartCoroutine(LoadScene(name));
    }

    /// <summary>
    /// Loads the scene with the given build id.
    /// </summary>
    /// <param name="id"> The build id of the scene to be loaded.</param>
    public void Load(int id)
    {
        sceneTransition.FadeIn();
        StartCoroutine(LoadScene(id));
    }

    /// <summary>
    /// Loads the next scene.
    /// </summary>
    public void Load()
    {
        sceneTransition.FadeIn();
        Debug.Log(SceneManager.GetActiveScene().buildIndex);
        StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex + 1));
    }

    /// <summary>
    /// Reloads the current scene.
    /// </summary>
    public void Reload()
    {
        sceneTransition.FadeIn();
        string name = SceneManager.GetActiveScene().name;
        StartCoroutine(LoadScene(name));
    }

    /// <summary>
    /// Exits the game.
    /// </summary>
    public void Exit()
    {
        Application.Quit();
    }

    System.Collections.IEnumerator LoadScene(int id)
    {
        async = SceneManager.LoadSceneAsync(id);
        async.allowSceneActivation = false;
        yield return StartCoroutine(Anim());
        Saving.GameSaveManager.Instance.Save();
    }

    System.Collections.IEnumerator LoadScene(string name)
    {
        async = SceneManager.LoadSceneAsync(name);
        async.allowSceneActivation = false;
        yield return StartCoroutine(Anim());
        Saving.GameSaveManager.Instance.Save();
    }

    System.Collections.IEnumerator Anim()
    {
        float t = 0.0f;
        while (!async.isDone)
        {
            t += Time.deltaTime;
            if (async.progress >= 0.9f)
            {
                if (t < sceneTransition.Duration)
                    yield return new WaitForSeconds(sceneTransition.Duration - t);
                async.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}