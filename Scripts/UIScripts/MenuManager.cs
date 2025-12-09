using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private string m_GameplaySceneName = "TestLevel"; // Nome EXATO da sua cena de jogo

    public void PlayGame()
    {
        // Carrega a fase do jogo
        SceneManager.LoadScene(m_GameplaySceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Saiu do Jogo!");
        Application.Quit();
    }
}