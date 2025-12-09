using UnityEngine;

public class BombPlacer : BaseWeapon
{
    [Header("Configurações da Bomba")]
    [SerializeField] private Bomb m_BombPrefab;
    [SerializeField] private int m_BombRange = 1;
    [SerializeField] private int m_MaxBombs = 1; // Começa podendo por 1 bomba
    [Header("Audio")]
    [SerializeField] private AudioClip m_PlaceBombSound; 

    private int m_ActiveBombs = 0; // Quantas tem no chão agora
    private Grid m_Grid;

    private void Start()
    {
        m_Grid = FindFirstObjectByType<Grid>();

        // Avisa a UI inicial
        GameplayManager.OnPlayerRangeChanged?.Invoke(m_BombRange);
        GameplayManager.OnPlayerBombCountChanged?.Invoke(m_MaxBombs - m_ActiveBombs, m_MaxBombs);
    }

    public override bool CanUse()
    {
        // 1. Verifica Cooldown (Tempo mínimo entre cliques, coloque 0.1 no Inspector)
        if (!base.CanUse()) return false;

        // 2. Verifica Quantidade
        if (m_ActiveBombs >= m_MaxBombs)
        {
            Debug.Log("Limite de bombas atingido!");
            return false;
        }

        return true;
    }

    public override void Use()
    {
        if (CanUse()) PlaceBomb();
    }

    // --- MÉTODOS PARA OS ITENS CHAMAREM ---
    public void IncreaseRange()
    {
        m_BombRange++;
        GameplayManager.OnPlayerRangeChanged?.Invoke(m_BombRange);
    }

    public void IncreaseMaxBombs()
    {
        m_MaxBombs++;
        GameplayManager.OnPlayerBombCountChanged?.Invoke(m_MaxBombs - m_ActiveBombs, m_MaxBombs);
    }
    // -------------------------------------

    private void PlaceBomb()
    {
        if (m_Grid == null) return;

        Vector3 playerPos = m_Owner.transform.position;
        Vector3Int cellPosition = m_Grid.WorldToCell(playerPos);
        Vector3 spawnPos = m_Grid.GetCellCenterWorld(cellPosition);
        spawnPos.z = 0;

        // 3. VERIFICA SE JÁ TEM BOMBA NO LOCAL
        // Cria um circulo pequeno (0.4f) e vê se bate em alguma coisa que seja Bomba
        Collider2D hit = Physics2D.OverlapCircle(spawnPos, 0.4f);
        if (hit != null && hit.GetComponent<Bomb>() != null)
        {
            // Já tem bomba aqui! Cancela.
            return;
        }
        //tocar som de colocar bomba
        if (m_PlaceBombSound) GameplayManager.Instance.PlaySFX(m_PlaceBombSound);
        Bomb newBomb = Instantiate(m_BombPrefab, spawnPos, Quaternion.identity);

        // AUMENTA CONTADOR
        m_ActiveBombs++;
        GameplayManager.OnPlayerBombCountChanged?.Invoke(m_MaxBombs - m_ActiveBombs, m_MaxBombs);

        // Passa a função que diminui o contador quando explodir
        newBomb.Setup(m_Damage, m_BombRange, () =>
        {
            m_ActiveBombs--;
            GameplayManager.OnPlayerBombCountChanged?.Invoke(m_MaxBombs - m_ActiveBombs, m_MaxBombs);
        });

        m_LastAttackTime = Time.time;
    }
}