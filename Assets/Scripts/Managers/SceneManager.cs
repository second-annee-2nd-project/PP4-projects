using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{

    private static SceneManager instance;
    public static SceneManager Instance => instance;

    private int nextLevelID;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        nextLevelID = 1;
    }

    public void LoadNextScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextLevelID);
        nextLevelID++;
    }
    public void LoadFirstPlayScene()
    {
        LoadNextScene();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
