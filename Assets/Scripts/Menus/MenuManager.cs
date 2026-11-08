
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Nivel1");
    }

    public void RestartLevel()
    {
        string level = PlayerPrefs.GetString("NivelActual", "Nivel1");
        Time.timeScale = 1f;
        SceneManager.LoadScene(level);
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuEntrada");
    }

    public void ShowDeathScreen()
    {
        PlayerPrefs.SetString("NivelActual",
            SceneManager.GetActiveScene().name);
        SceneManager.LoadScene("Muerte");
    }

    public void ShowVictoryScreen()
    {
        SceneManager.LoadScene("Final");
    }
}

