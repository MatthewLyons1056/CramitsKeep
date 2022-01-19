using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainSceneManager : PlayerControlsLink
{
    public GameObject PauseMenuOBJ, askQuitOBJ, backgroundOBJ, askRestartOBJ, askMenuOBJ, controlMenuOBJ, optionsMenuOBJ, settingsMenuOBJ;
    public List<GameObject> dontDestroyOBJs = new List<GameObject>();


    public GameObject exitPanel;
    private void Update()
    {
        if(Input.GetButtonDown("Cancel") && !PlayerHealthHolder.isDead)
        {
            if(PlayerMain.canInteract)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }
    }
    public void PauseGame()
    {
        GameObject gameControllerOBJ = GameObject.FindWithTag("MainSceneController");
        AudioSource musicSource = gameControllerOBJ.GetComponentInChildren<AudioSource>();
        musicSource.volume /= 2f;
        PlayerMain.canInteract = false;
        PauseMenuOBJ.SetActive(true);
        backgroundOBJ.SetActive(true);
        foreach (GameObject UISection in PlayerMain.sectionsArray)
        {
            UISection.SetActive(true);
        }
        Time.timeScale = 0;
    }
    public void ResumeGame()
    {
        GameObject gameControllerOBJ = GameObject.FindWithTag("MainSceneController");
        AudioSource musicSource = gameControllerOBJ.GetComponentInChildren<AudioSource>();
        musicSource.volume *= 2f;
        HideMenus(askQuitOBJ);
        HideMenus(PauseMenuOBJ);
        HideMenus(askRestartOBJ);
        HideMenus(askMenuOBJ);
        HideMenus(controlMenuOBJ);
        HideMenus(settingsMenuOBJ);
        HideMenus(optionsMenuOBJ);
        backgroundOBJ.SetActive(false);
        PlayerMain.leftSideUI.SetActive(true);
        foreach (GameObject UISection in PlayerMain.sectionsArray)
        {
            UISection.SetActive(false);
        }
        Time.timeScale = 1;
        PlayerMain.canInteract = true;
    }
    public void HideMenus(GameObject targetMenu)
    {
        targetMenu.SetActive(false);
    }
    public void ShowMenus(GameObject targetMenu)
    {
        foreach (GameObject UISection in PlayerMain.sectionsArray)
        {
            UISection.SetActive(true);
        }
        PlayerMain.leftSideUI.SetActive(true);
        targetMenu.SetActive(true);
    }
    public void HideExtraUI()
    {
        PlayerMain.leftSideUI.SetActive(false);
        foreach (GameObject UISection in PlayerMain.sectionsArray)
        {
            UISection.SetActive(false);
        }
    }
    public void Restart(int sceneNumber)
    {
        StartCoroutine(loadGame());
        DestroyIndustructables(sceneNumber);
    }
    public void Quit()
    {
        Application.Quit();
    }

    public void DestroyIndustructables(int sceneNumber)
    {
        foreach(GameObject OBJ in dontDestroyOBJs)
        {
            Destroy(OBJ);
        }
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneNumber);
    }

    public void MoveToShop()
    {
        StartCoroutine(loadGame());
        SceneManager.LoadScene("Shop");
        Debug.Log("moving to shop");
    }
    public void NextLevel(string levelName)
    {
        StartCoroutine(loadGame());
        SceneManager.LoadScene(levelName); //current level 
    }
    IEnumerator loadGame()
    {
        yield return new WaitForSeconds(1f);
        /*   
           exitPanel.SetActive(true);

           SceneManager.LoadScene(1);
           Time.timeScale = 1;
           */
    }
}

