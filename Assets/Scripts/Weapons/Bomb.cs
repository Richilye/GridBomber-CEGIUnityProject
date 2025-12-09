using System;
using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("Visual da Explosão (Prefabs)")]
    [SerializeField] private Explosion m_ExplosionCenter;

    [Header("Meios (Tubos)")]
    [SerializeField] private Explosion m_MiddleHorizontal; // Deitado
    [SerializeField] private Explosion m_MiddleVertical;   // Em pé

    [Header("Pontas (Ends)")]
    [SerializeField] private Explosion m_EndUp;
    [SerializeField] private Explosion m_EndDown;
    [SerializeField] private Explosion m_EndLeft;
    [SerializeField] private Explosion m_EndRight;

    [Header("Configurações")]
    [SerializeField] private float m_FuseTime = 3f;
    [SerializeField] private int m_Damage = 1;
    [SerializeField] private LayerMask m_LevelLayer;
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
        if (GameplayManager.Instance) GameplayManager.Instance.PlaySFX(m_ExplosionSound);

        // Centro sempre igual
        Instantiate(m_ExplosionCenter, transform.position, Quaternion.identity);

        // Espalha
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

            Collider2D hit = Physics2D.OverlapBox(position, Vector2.one * 0.5f, 0, m_LevelLayer);

            // Se bateu em algo (Parede ou Bloco)
            if (hit != null)
            {
                DestructibleBlock block = hit.GetComponent<DestructibleBlock>();
                if (block != null)
                {
                    block.DestroyBlock();
                    // Na parede quebrável, usamos a PONTA correta para aquela direção
                    Instantiate(GetEndPrefab(direction), position, Quaternion.identity);
                }
                Bomb otherBomb = hit.GetComponent<Bomb>();
                if (otherBomb != null) otherBomb.ForceExplode();
                break;
            }

            // Se for o último bloco do range
            if (i == m_ExplosionRange)
            {
                Instantiate(GetEndPrefab(direction), position, Quaternion.identity);
            }
            else
            {
                // Se for meio do caminho
                Instantiate(GetMiddlePrefab(direction), position, Quaternion.identity);
            }

            yield return new WaitForSeconds(0.05f);
        }
    }

    // --- LÓGICA PARA ESCOLHER O PREFAB CERTO ---
    private Explosion GetEndPrefab(Vector2 direction)
    {
        if (direction == Vector2.up) return m_EndUp;
        if (direction == Vector2.down) return m_EndDown;
        if (direction == Vector2.left) return m_EndLeft;
        if (direction == Vector2.right) return m_EndRight;
        return m_ExplosionCenter; // Fallback
    }

    private Explosion GetMiddlePrefab(Vector2 direction)
    {
        // Se for vertical (Y != 0), usa o tubo em pé
        if (direction.y != 0) return m_MiddleVertical;
        // Se for horizontal, usa o tubo deitado
        return m_MiddleHorizontal;
    }
}