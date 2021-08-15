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
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            nextLevelID = 1;
        }
    }

    public void LoadNextScene()
    {
        Debug.Log("essaye de load la scene ID : "+nextLevelID);
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextLevelID);
        nextLevelID++;
        Debug.Log("new id : "+nextLevelID);
        Debug.Log("nombre de scènes : "+UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings);
        if (nextLevelID > UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings)
        {
            nextLevelID = 0;
        }
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
