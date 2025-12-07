using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Paineis")]
    [SerializeField] private GameObject m_GameOverPanel;

    [Header("HUD Texts (Arraste os Textos aqui)")]
    [SerializeField] private TextMeshProUGUI m_HealthText;
    [SerializeField] private TextMeshProUGUI m_BombText;
    [SerializeField] private TextMeshProUGUI m_LivesText;
    [SerializeField] private TextMeshProUGUI m_RangeText;
    [SerializeField] private TextMeshProUGUI m_ScoreText;
    [SerializeField] private TextMeshProUGUI m_TimerText;

    private void Awake()
    {
        Instance = this;
    }

    public void Initialize()
    {
        if (m_GameOverPanel) m_GameOverPanel.SetActive(false);
    }

    // --- FUNÇÕES DE ATUALIZAÇÃO ---

    public void UpdateTime(float timeRemaining)
    {
        if (m_TimerText)
        {
            // Formata para 00:00 (Minutos:Segundos)
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            m_TimerText.text = $"{minutes:00}:{seconds:00}";

            // Opcional: Ficar vermelho quando tiver pouco tempo
            if (timeRemaining < 30) m_TimerText.color = Color.red;
            else m_TimerText.color = Color.white;
        }
    }

    public void UpdateHealthBar(int current, int max)
    {
        // Mostra "0" se tiver 1 de vida (Vida Extra)
        // Mathf.Max(0, ...) garante que nunca mostre -1
        if (m_HealthText) m_HealthText.text = $"x {Mathf.Max(0, current - 1)}";
    }

    public void UpdateBombCount(int current, int max)
    {
        if (m_BombText) m_BombText.text = $"x {current}"; // Ex: 💣 x 1
    }

    public void UpdateLives(int lives)
    {
        if (m_LivesText) m_LivesText.text = $"x {Mathf.Max(0, lives - 1)}";
    }

    public void UpdateRange(int range)
    {
        if (m_RangeText) m_RangeText.text = $"x {range}"; // Ex: (Fogo) x 2
    }

    public void UpdateScore(int score)
    {
        // "D6" formata o numero com zeros a esquerda: 000100
        if (m_ScoreText) m_ScoreText.text = $"SCORE: {score:D6}";
    }

    public void ShowGameOverPanel()
    {
        if (m_GameOverPanel) m_GameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}