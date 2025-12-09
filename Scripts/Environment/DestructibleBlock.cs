using UnityEngine;

public class DestructibleBlock : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private GameObject m_DestroyVFX;

    [Header("Drops")]
    [SerializeField] private ItemPickup[] m_PossibleDrops; 
    [SerializeField][Range(0, 100)] private float m_DropChance = 30f; 

    public void DestroyBlock()
    {
        
        if (m_DestroyVFX != null)
        {
            Instantiate(m_DestroyVFX, transform.position, Quaternion.identity);
        }

        
        TrySpawnItem();

        
        Destroy(gameObject);
    }

    private void TrySpawnItem()
    {
        if (Random.Range(0f, 100f) <= m_DropChance)
        {
            if (m_PossibleDrops.Length > 0)
            {
                int randomIndex = Random.Range(0, m_PossibleDrops.Length);
                Instantiate(m_PossibleDrops[randomIndex], transform.position, Quaternion.identity);
            }
        }
    }
}