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
                // Toca o som no local do item, independente do objeto morrer
                if (m_PickupSound != null)
                    AudioSource.PlayClipAtPoint(m_PickupSound, transform.position);

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
                player.AddSpeed(1f); // Aumenta velocidade em 1
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