using System;
using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("Visual da Explosão")]
    [SerializeField] private Explosion m_ExplosionCenter;
    [SerializeField] private Explosion m_ExplosionMiddle; // O "tubo"
    [SerializeField] private Explosion m_ExplosionEnd;    // A "ponta"

    // ... (Variáveis existentes: FuseTime, Damage, LayerMask, etc) ...
    [SerializeField] private float m_FuseTime = 3f;
    [SerializeField] private int m_Damage = 1;
    [SerializeField] private LayerMask m_LevelLayer;
    [Header("Audio")]
    [SerializeField] private AudioClip m_ExplosionSound;

    private int m_ExplosionRange;
    private Collider2D m_Collider;
    private bool m_HasExploded = false;
    private Action m_OnExplodeCallback; 

    private void Awake()
    {
        m_Collider = GetComponent<Collider2D>();
        m_Collider.isTrigger = true;
    }

    public void Setup(int damage, int range, Action onExplode)
    {
        m_Damage = damage;
        m_ExplosionRange = range;
        m_OnExplodeCallback = onExplode;
        StartCoroutine(ExplodeRoutine());
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) m_Collider.isTrigger = false;
    }

    public void ForceExplode()
    {
        if (m_HasExploded) return;
        StopAllCoroutines();
        Explode();
    }

    private IEnumerator ExplodeRoutine()
    {
        yield return new WaitForSeconds(m_FuseTime);
        Explode();
    }

    private void Explode()
    {
        if (m_HasExploded) return;
        m_HasExploded = true;

        m_OnExplodeCallback?.Invoke();
        if(GameplayManager.Instance) GameplayManager.Instance.PlaySFX(m_ExplosionSound);

        // 1. Instancia o CENTRO (Sem rotação)
        Instantiate(m_ExplosionCenter, transform.position, Quaternion.identity);

        // 2. Espalha para as direções
        StartCoroutine(CreateExplosions(Vector2.up));
        StartCoroutine(CreateExplosions(Vector2.down));
        StartCoroutine(CreateExplosions(Vector2.left));
        StartCoroutine(CreateExplosions(Vector2.right));

        GetComponent<SpriteRenderer>().enabled = false;
        m_Collider.enabled = false;
        Destroy(gameObject, 0.3f);
    }

    private IEnumerator CreateExplosions(Vector2 direction)
    {
        for (int i = 1; i <= m_ExplosionRange; i++)
        {
            Vector3 position = transform.position + (Vector3)(direction * i);

            // Calcula a rotação baseada na direção
            // Atan2 dá o ângulo em radianos, convertemos para graus e criamos a rotação Z
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);

            Collider2D hit = Physics2D.OverlapBox(position, Vector2.one * 0.5f, 0, m_LevelLayer);

            if (hit != null)
            {
                DestructibleBlock block = hit.GetComponent<DestructibleBlock>();
                if (block != null)
                {
                    block.DestroyBlock();
                    // Na parede, sempre usamos a PONTA ou o CENTRO, pois a explosão morre ali
                    Instantiate(m_ExplosionEnd, position, rotation); 
                }
                Bomb otherBomb = hit.GetComponent<Bomb>();
                if (otherBomb != null) otherBomb.ForceExplode();
                break;
            }

            // Lógica: É o último bloco do range?
            if (i == m_ExplosionRange)
            {
                // Sim: Usa a PONTA
                Instantiate(m_ExplosionEnd, position, rotation);
            }
            else
            {
                // Não: Usa o MEIO
                Instantiate(m_ExplosionMiddle, position, rotation);
            }

            yield return new WaitForSeconds(0.05f);
        }
    }
}