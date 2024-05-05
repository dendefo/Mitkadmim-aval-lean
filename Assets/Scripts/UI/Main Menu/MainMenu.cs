using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject Main_Menu;
    [SerializeField] GameObject SettingsMenu;
    [SerializeField] GameObject LoadingScreen;
    [SerializeField] GameObject SceneReadyScreen;
    [SerializeField] string SceneToLoad;
    [SerializeField] Image LoadingBar;
    public void StartGame()
    {
        Main_Menu.SetActive(false);
        LoadingScreen.SetActive(true);
        var loading = SceneManager.LoadSceneAsync(SceneToLoad);
        StartCoroutine(LoadScene(loading));
        loading.allowSceneActivation = false;
    }

    private IEnumerator LoadScene(AsyncOperation loading)
    {
        while (loading.progress < 0.9f)
        {
            LoadingBar.fillAmount = loading.progress;
            yield return new WaitForEndOfFrame();
        }
        SceneReady();
        while (!Input.anyKey)
        {
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("Activated");
        loading.allowSceneActivation = true;
        yield return loading;
    }
    public void SceneReady()
    {
        LoadingScreen.SetActive(false);
        SceneReadyScreen.SetActive(true);
    }
    public void Exit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    public void OpenSettings()
    {
        Main_Menu.SetActive(false);
        SettingsMenu.SetActive(true);
    }
    public void CloseSettings()
    {
        Main_Menu.SetActive(true);
        SettingsMenu.SetActive(false);
    }
    public void Continue()
    {
        ISavable.WantsToLoad = true;
        StartGame();
    }
}