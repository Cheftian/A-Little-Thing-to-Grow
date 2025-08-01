using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuNavigator : MonoBehaviour
{
    public void PlayGame()
    {
        SceneLoader.sceneToLoad = "Game";
        SceneManager.LoadScene("LoadingScene");
    }

    public void OpenSettings()
    {
        SceneManager.LoadScene("Settings");
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OpenCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}