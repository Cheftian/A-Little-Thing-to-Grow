using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuNavigator : MonoBehaviour
{
    
    public void PlayGame()
    {
        SceneManager.LoadScene("Game");
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