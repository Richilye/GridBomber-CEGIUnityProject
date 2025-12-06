using UnityEngine;
using TMPro; // Necessário para textos (TextMeshPro)

public class UIManager : MonoBehaviour
{
    // Singleton simples para facilitar acesso (opcional, mas ajuda)
    public static UIManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void Initialize()
    {
        // Configurações iniciais da tela
        Debug.Log("UI Iniciada");
    }

    // Funções que o GameplayManager do professor chama (para não quebrar o código dele)
    public void UpdateHealthBar(int current, int max)
    {
        Debug.Log($"Vida mudou: {current}/{max}");
    }

    public void UpdateBombCount(int current, int max)
    {
        // Bomberman não tem munição, mas podemos usar para "Bombas Restantes"
        Debug.Log($"Bombas: {current}");
    }

    public void UpdateEnemiesDiedCount(int count)
    {
        Debug.Log($"Inimigos mortos: {count}");
    }

    public void ShowGameOverPanel()
    {
        Debug.Log("GAME OVER - Tela apareceu");
    }
}