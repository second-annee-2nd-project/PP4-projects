using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{

    private static SceneManager instance;
    public static SceneManager Instance => instance;

    private int nextLevelID;
    public int NextLevelID => nextLevelID;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void LoadNextScene(float timer)
    {
        StartCoroutine(LoadSceneAfterSomeTime(timer));
    }

    public IEnumerator LoadSceneAfterSomeTime(float timer)
    {
        yield return new WaitForSeconds(timer);
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextLevelID);
        nextLevelID++;
        if (nextLevelID > UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings)
        {
            nextLevelID = 0;
        }
    }
    public void LoadFirstPlayScene()
    {
        nextLevelID = 1;
        LoadNextScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
