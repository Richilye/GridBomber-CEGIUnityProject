using System; // Necessário para Action
using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    // ... (Mantenha as variáveis Header existentes) ...
    [SerializeField] private float m_FuseTime = 3f;
    [SerializeField] private int m_Damage = 1;
    [SerializeField] private Explosion m_ExplosionPrefab;
    [SerializeField] private LayerMask m_LevelLayer;

    private int m_ExplosionRange;
    private Collider2D m_Collider;
    private bool m_HasExploded = false;

    // --- NOVO: Notificação de explosão ---
    private Action m_OnExplodeCallback;

    private void Awake()
    {
        m_Collider = GetComponent<Collider2D>();
        m_Collider.isTrigger = true;
    }

    // --- SETUP ATUALIZADO ---
    public void Setup(int damage, int range, Action onExplode)
    {
        m_Damage = damage;
        m_ExplosionRange = range;
        m_OnExplodeCallback = onExplode; // Guarda quem avisar

        StartCoroutine(ExplodeRoutine());
    }

    // ... (Mantenha OnTriggerExit2D e ForceExplode iguais) ...
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

        // AVISA O PLACER QUE LIBEROU UM SLOT
        m_OnExplodeCallback?.Invoke();

        Instantiate(m_ExplosionPrefab, transform.position, Quaternion.identity);

        StartCoroutine(CreateExplosions(Vector2.up));
        StartCoroutine(CreateExplosions(Vector2.down));
        StartCoroutine(CreateExplosions(Vector2.left));
        StartCoroutine(CreateExplosions(Vector2.right));

        GetComponent<SpriteRenderer>().enabled = false;
        m_Collider.enabled = false;
        Destroy(gameObject, 0.3f);
    }

    // ... (Mantenha o CreateExplosions igual) ...
    private IEnumerator CreateExplosions(Vector2 direction)
    {
        // (Copie o conteúdo do seu CreateExplosions anterior aqui, ele não mudou)
        // ...
        for (int i = 1; i <= m_ExplosionRange; i++)
        {
            Vector3 position = transform.position + (Vector3)(direction * i);
            Collider2D hit = Physics2D.OverlapBox(position, Vector2.one * 0.5f, 0, m_LevelLayer);

            if (hit != null)
            {
                DestructibleBlock block = hit.GetComponent<DestructibleBlock>();
                if (block != null)
                {
                    block.DestroyBlock();
                    Instantiate(m_ExplosionPrefab, position, Quaternion.identity);
                }
                Bomb otherBomb = hit.GetComponent<Bomb>();
                if (otherBomb != null) otherBomb.ForceExplode();
                break;
            }
            Instantiate(m_ExplosionPrefab, position, Quaternion.identity);
            yield return new WaitForSeconds(0.05f);
        }
    }
}