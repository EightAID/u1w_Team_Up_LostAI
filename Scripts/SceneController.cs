using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance { get; private set; }

    private void Awake()
    {
        // シングルトンのセット
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // 任意のシーンを名前でロード
    public void LoadSceneName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // シーンをインデックスでロード
    public void LoadSceneIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    // 現在のシーンをリロード
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // 次のシーンをロード（ビルド順）
    public void LoadNextScene()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = (currentIndex + 1) % SceneManager.sceneCountInBuildSettings;
        SceneManager.LoadScene(nextIndex);
    }
}
