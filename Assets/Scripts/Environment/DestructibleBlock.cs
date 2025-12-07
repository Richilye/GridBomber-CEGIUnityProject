using UnityEngine;

public class DestructibleBlock : MonoBehaviour
{
    [SerializeField] private float m_DestroyDelay = 0.1f;

    [Header("Drops")]
    [SerializeField] private ItemPickup[] m_PossibleDrops; // Lista de prefabs de itens
    [SerializeField][Range(0, 100)] private float m_DropChance = 30f; // 30% de chance

    public void DestroyBlock()
    {
        TrySpawnItem();
        Destroy(gameObject, m_DestroyDelay);
    }

    private void TrySpawnItem()
    {
        // Sorteia se vai cair algo (0 a 100)
        if (Random.Range(0f, 100f) <= m_DropChance)
        {
            if (m_PossibleDrops.Length > 0)
            {
                // Escolhe um item aleatório da lista
                int randomIndex = Random.Range(0, m_PossibleDrops.Length);
                Instantiate(m_PossibleDrops[randomIndex], transform.position, Quaternion.identity);
            }
        }
    }
}