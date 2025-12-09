using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public enum ItemType
    {
        ExtraBomb,
        BlastRadius,
        SpeedUp,
        ExtraLife,
        Heal
    }

    [Header("Configuração do Item")]
    [SerializeField] private ItemType m_Type;
    [SerializeField] private SpriteRenderer m_SpriteRenderer;

    //Som ao pegar
    [SerializeField] private AudioClip m_PickupSound;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                if (m_PickupSound != null)
                    GameplayManager.Instance.PlaySFX(m_PickupSound);

                ApplyEffect(player);
                Destroy(gameObject);
            }
        }
    }

    private void ApplyEffect(Player player)
    {
        Debug.Log($"Pegou item: {m_Type}");

        switch (m_Type)
        {
            case ItemType.ExtraBomb:
                player.AddMaxBombs();
                break;
            case ItemType.BlastRadius:
                player.AddExplosionRange();
                break;
            case ItemType.SpeedUp:
                player.AddSpeed(0.5f);
                break;
            case ItemType.ExtraLife:
                player.AddLife();
                break;
            case ItemType.Heal:
                player.Heal();
                break;
        }
    }
}