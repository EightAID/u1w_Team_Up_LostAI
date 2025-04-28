using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance { get; private set; }

    private void Awake()
    {
        // �V���O���g���̃Z�b�g
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // �C�ӂ̃V�[���𖼑O�Ń��[�h
    public void LoadSceneName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // �V�[�����C���f�b�N�X�Ń��[�h
    public void LoadSceneIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    // ���݂̃V�[���������[�h
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ���̃V�[�������[�h�i�r���h���j
    public void LoadNextScene()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = (currentIndex + 1) % SceneManager.sceneCountInBuildSettings;
        SceneManager.LoadScene(nextIndex);
    }
}
