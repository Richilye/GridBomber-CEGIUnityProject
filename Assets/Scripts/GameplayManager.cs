using System;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    // --- EVENTOS ---
    public static Action OnPlayerDied;
    public static Action<Vector3> OnEnemyDied;

    public static Action<int, int> OnPlayerHealthChanged;
    public static Action<int, int> OnPlayerBombCountChanged;
    public static Action<int> OnPlayerLivesChanged;
    public static Action<int> OnPlayerRangeChanged;
    public static Action<int> OnScoreChanged;

    [Header("Configurações")]
    [SerializeField] private float m_maxGameplayTimeSec = 180;
    [SerializeField] private Player m_player;
    [SerializeField] private UIManager m_uiManager;

    [Header("Audio")]
    [SerializeField] private AudioSource m_MusicSource; // Crie um AudioSource no objeto Managers
    [SerializeField] private AudioSource m_SfxSource;
    [SerializeField] private AudioClip m_StageBGM;

    [Header("Prefabs")]
    [SerializeField] private FloatingScore m_ScorePopupPrefab; //prefab do floating text.

    private bool m_isGameRunning;
    private float m_gameplayTimer;

    // --- VARIÁVEIS DE SCORE E COMBO ---
    private int m_CurrentScore;
    private int m_ComboMultiplier = 1;
    private float m_LastKillTime = 0f;
    private const float COMBO_WINDOW = 0.5f; // Meio segundo para manter o combo

    public static GameplayManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        OnPlayerDied += HandleOnPlayerDied;
        OnEnemyDied += HandleOnEnemyDied;

        if (m_uiManager != null)
        {
            OnPlayerHealthChanged += m_uiManager.UpdateHealthBar;
            OnPlayerBombCountChanged += m_uiManager.UpdateBombCount;
            OnPlayerLivesChanged += m_uiManager.UpdateLives;
            OnPlayerRangeChanged += m_uiManager.UpdateRange;
            OnScoreChanged += m_uiManager.UpdateScore;
        }
    }

    private void Start()
    {
        if (m_player == null) m_player = FindFirstObjectByType<Player>();
        if (m_uiManager != null) m_uiManager.Initialize();

        if (m_player != null)
        {
            m_player.Initialize();
            m_isGameRunning = true;
        }

        m_gameplayTimer = Time.time;
        m_CurrentScore = 0;
        OnScoreChanged?.Invoke(m_CurrentScore);
        if (m_MusicSource && m_StageBGM)
        {
            m_MusicSource.clip = m_StageBGM;
            m_MusicSource.loop = true; // Loop infinito
            m_MusicSource.Play();
        }
        Canvas canvas = GetComponentInChildren<Canvas>();
        if (canvas.worldCamera == null)
            canvas.worldCamera = Camera.main;
    }

    private void Update()
    {
        if (!m_isGameRunning) return;

        float timeRemaining = m_maxGameplayTimeSec - (Time.time - m_gameplayTimer);

        if (timeRemaining <= 0)
        {
            Debug.Log("Tempo Esgotado!");
            if (m_uiManager != null) m_uiManager.UpdateTime(0);
            EndGameplay();
        }
        else
        {
            if (m_uiManager != null) m_uiManager.UpdateTime(timeRemaining);
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && m_SfxSource != null)
        {
            m_SfxSource.PlayOneShot(clip);
        }
    }

    private void HandleOnPlayerDied()
    {
        Debug.Log("Game Over!");
        EndGameplay();
    }

    // --- LÓGICA DE COMBO NOVA ---
    private void HandleOnEnemyDied(Vector3 enemyPosition)
    {
        if (Time.time - m_LastKillTime < COMBO_WINDOW) m_ComboMultiplier++;
        else m_ComboMultiplier = 1;

        int points = 100 * m_ComboMultiplier;
        m_CurrentScore += points;

        OnScoreChanged?.Invoke(m_CurrentScore);
        m_LastKillTime = Time.time;

        // --- NOVA LÓGICA: SPAWNAR O TEXTO ---
        if (m_ScorePopupPrefab != null)
        {
            // Cria o texto na posição do inimigo
            FloatingScore popup = Instantiate(m_ScorePopupPrefab, enemyPosition, Quaternion.identity);
            popup.Setup(points);
        }
    }

    private void EndGameplay()
    {
        if (!m_isGameRunning) return;
        m_isGameRunning = false;

        if (m_uiManager != null)
            m_uiManager.ShowGameOverPanel();
    }

    private void OnDestroy()
    {
        OnPlayerDied -= HandleOnPlayerDied;
        OnEnemyDied -= HandleOnEnemyDied;

        if (m_uiManager != null)
        {
            OnPlayerHealthChanged -= m_uiManager.UpdateHealthBar;
            OnPlayerBombCountChanged -= m_uiManager.UpdateBombCount;
            OnPlayerLivesChanged -= m_uiManager.UpdateLives;
            OnPlayerRangeChanged -= m_uiManager.UpdateRange;
            OnScoreChanged -= m_uiManager.UpdateScore;
        }
    }
}