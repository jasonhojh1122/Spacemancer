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
    /// Loads the next scene.
    /// </summary>
    public void Load()
    {
        sceneTransition.FadeIn();
        string name = SceneManager.GetSceneByBuildIndex(SceneManager.GetActiveScene().buildIndex + 1).name;
        StartCoroutine(LoadScene(name));
    }

    public void Reload()
    {
        sceneTransition.FadeIn();
        string name = SceneManager.GetActiveScene().name;
        StartCoroutine(LoadScene(name));
    }

    System.Collections.IEnumerator LoadScene(string name)
    {
        float t = 0.0f;
        async = SceneManager.LoadSceneAsync(name);
        async.allowSceneActivation = false;
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