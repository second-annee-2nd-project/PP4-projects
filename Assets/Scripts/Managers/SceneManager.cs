﻿using System.Collections;
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
        StartCoroutine(LoadSceneAfterSomeTime());
    }

    private IEnumerator LoadSceneAfterSomeTime()
    {
        yield return new WaitForSeconds(3f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextLevelID);
        nextLevelID++;
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
