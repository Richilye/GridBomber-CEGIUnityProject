using System;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    // Eventos estáticos que o Player e Inimigos podem chamar
    public static Action OnPlayerDied;
    public static Action<int, int> OnPlayerHealthChanged; // Atual Vida, Max Vida
    public static Action<int, int> OnPlayerAmmoChanged;   // Bombas Atuais, Max Bombas

    [Header("Configurações da Partida")]
    [SerializeField] private float m_maxGameplayTimeSec = 180; // 3 minutos é padrão Bomberman
    [SerializeField] private Player m_player;
    [SerializeField] private UIManager m_uiManager;

    private bool m_isGameRunning;
    private float m_gameplayTimer;

    private void Awake()
    {
        // Inscrevendo as funções nos eventos
        OnPlayerDied += HandleOnPlayerDied;

        // Conecta os eventos de UI se o UIManager existir
        if (m_uiManager != null)
        {
            OnPlayerHealthChanged += m_uiManager.UpdateHealthBar;
            OnPlayerAmmoChanged += m_uiManager.UpdateAmmoCount;
        }
    }

    private void Start()
    {
        // Se esqueceu de arrastar o Player no Inspector, tenta achar sozinho
        if (m_player == null)
            m_player = FindFirstObjectByType<Player>();

        if (m_uiManager != null)
            m_uiManager.Initialize();

        if (m_player != null)
        {
            m_player.Initialize(); // Inicia vida e inputs do Player
            m_isGameRunning = true;
        }
        else
        {
            Debug.LogError("Player não encontrado na cena! Arraste o Player para o campo 'Player' no GameplayManager.");
        }

        m_gameplayTimer = Time.time;
    }

    private void Update()
    {
        if (!m_isGameRunning) return;

        // Verifica o tempo limite da fase
        if (Time.time - m_gameplayTimer > m_maxGameplayTimeSec)
        {
            Debug.Log("Tempo Esgotado!");
            EndGameplay();
        }
    }

    private void HandleOnPlayerDied()
    {
        Debug.Log("Player Morreu!");
        EndGameplay();
    }

    private void EndGameplay()
    {
        if (!m_isGameRunning) return;
        m_isGameRunning = false;

        // Se tiver UI, mostra Game Over
        if (m_uiManager != null)
            m_uiManager.ShowGameOverPanel();
        else
            Debug.Log("Fim de Jogo (Sem UI configurada)");
    }

    // Importante remover os eventos ao destruir o objeto para não dar erro de memória
    private void OnDestroy()
    {
        OnPlayerDied -= HandleOnPlayerDied;
        if (m_uiManager != null)
        {
            OnPlayerHealthChanged -= m_uiManager.UpdateHealthBar;
            OnPlayerAmmoChanged -= m_uiManager.UpdateAmmoCount;
        }
    }
}