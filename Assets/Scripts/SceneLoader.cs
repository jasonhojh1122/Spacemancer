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
            Debug.LogError("Multiple instance of DefaultMaterialColor created");
        _instance = this;
    }

    public void Load(string name)
    {
        sceneTransition.FadeIn();
        StartCoroutine(LoadScene(name));
    }

    public void Load()
    {
        sceneTransition.FadeIn();
        string name = SceneManager.GetSceneByBuildIndex(SceneManager.GetActiveScene().buildIndex + 1).name;
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