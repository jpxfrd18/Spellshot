using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }
    public bool nextLevel { get; private set; } = false;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadLevel(int levelNum)
    {
        Destroy(MusicPlayer.Instance.gameObject);
        StartCoroutine(LoadSceneRoutine("Level" + levelNum.ToString()));
    }

    public void LoadMainMenu()
    {
        Destroy(MusicPlayer.Instance.gameObject);
        Destroy(RespawnManager.Instance.gameObject);
        Destroy(SpawnPoint.Instance.gameObject);
        nextLevel = false;
        StartCoroutine(LoadSceneRoutine("Main Menu"));
    }

    public void NextLevel()
    {
        Destroy(MusicPlayer.Instance.gameObject);
        Destroy(RespawnManager.Instance.gameObject);
        Destroy(SpawnPoint.Instance.gameObject);
        nextLevel = true;
        StartCoroutine(LoadSceneRoutine("Main Menu"));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        yield return null;
        SceneManager.LoadScene(sceneName);
    }
}
