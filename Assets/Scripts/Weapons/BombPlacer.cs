using UnityEngine;

// Herda de BaseWeapon para funcionar com o sistema do Player
public class BombPlacer : BaseWeapon
{
    [Header("Configurações da Bomba")]
    [SerializeField] private Bomb m_BombPrefab; // Referência ao prefab que criamos
    [SerializeField] private int m_BombRange = 1; // Alcance inicial

    // Variáveis internas para lógica de grid
    private Grid m_Grid;

    private void Start()
    {
        // Acha o Grid na cena automaticamente para alinhar a bomba
        m_Grid = FindFirstObjectByType<Grid>();
    }

    public override void Use()
    {
        // Verifica cooldown (herdado do BaseWeapon)
        if (!CanUse()) return;

        PlaceBomb();
    }

    private void PlaceBomb()
    {
        // 1. Onde colocar? Na posição do Player
        Vector3 playerPos = m_Owner.transform.position;

        // 2. Alinhamento com a Grade (Snap to Grid)
        // Isso converte a posição livre (ex: 1.54, 2.33) para indices da grid (1, 2)
        Vector3Int cellPosition = m_Grid.WorldToCell(playerPos);

        // Converte de volta para o centro daquela célula no mundo
        Vector3 spawnPos = m_Grid.GetCellCenterWorld(cellPosition);

        // 3. Cria a bomba
        Bomb newBomb = Instantiate(m_BombPrefab, spawnPos, Quaternion.identity);
        newBomb.Setup(m_Damage, m_BombRange);

        // Atualiza tempo do último ataque para o cooldown funcionar
        m_LastAttackTime = Time.time;

        // Toca som (se tivermos Audio Manager futuramente)
        Debug.Log("Bomba plantada em: " + cellPosition);
    }
}