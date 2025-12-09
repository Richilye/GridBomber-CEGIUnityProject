using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private int m_Damage = 1;
    [SerializeField] private float m_Lifetime = 0.5f;

    private void Start()
    {
        Destroy(gameObject, m_Lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Dano em Personagens
        BaseCharacter character = other.GetComponent<BaseCharacter>();
        if (character != null)
        {
            character.TakeDamage(m_Damage);
        }

        // Explodir outras Bombas 
        Bomb bomb = other.GetComponent<Bomb>();
        if (bomb != null)
        {
            bomb.ForceExplode();
        }
    }
}