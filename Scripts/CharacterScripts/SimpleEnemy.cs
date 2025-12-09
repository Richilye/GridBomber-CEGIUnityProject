using UnityEngine;

public class SimpleEnemy : BaseCharacter
{
    [Header("IA Settings")]
    [SerializeField] private LayerMask m_ObstacleLayer;
    [SerializeField] private float m_ChangeDirectionTime = 3f; // Muda a cada 3 segundos

    private Vector2 m_CurrentDirection;
    private Rigidbody2D m_Rb;
    private float m_Timer;

    private void Start()
    {
        Initialize();
    }

    protected override void Setup()
    {
        m_Rb = GetComponent<Rigidbody2D>();
        m_Speed = 2f;
        Health = 1;
        ChooseNewDirection();
    }

    protected override void UpdateLogic()
    {
        // 1. Checa obstáculo na frente
        CheckObstacles();

        // 2. Timer para mudar de direção aleatoriamente (Comportamento errático)
        m_Timer += Time.deltaTime;
        if (m_Timer > m_ChangeDirectionTime)
        {
            m_Timer = 0;
            // Tenta achar uma direção nova válida
            TryChangeDirectionRandomly();
        }
    }

private void FixedUpdate()
    {
        if (CanWalk())
        {
            m_Rb.linearVelocity = m_CurrentDirection * m_Speed;
            
            // --- CÓDIGO NOVO DE ANIMAÇÃO ---
            if (m_Animator)
            {
                // Avisa que está andando
                m_Animator.SetBool("IsMoving", true);
                
                // Passa a direção atual para o Blend Tree escolher o sprite (Cima/Baixo/Lado)
                m_Animator.SetFloat("InputX", m_CurrentDirection.x);
                m_Animator.SetFloat("InputY", m_CurrentDirection.y);
            }
            // -------------------------------
        }
        else
        {
            m_Rb.linearVelocity = Vector2.zero;
            if (m_Animator) m_Animator.SetBool("IsMoving", false);
        }
    }

    private void CheckObstacles()
    {
        float rayDist = 0.6f;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, m_CurrentDirection, rayDist, m_ObstacleLayer);

        if (hit.collider != null)
        {
            ChooseNewDirection(); // Bateu? Muda obrigatóriamente
        }
    }

    private void TryChangeDirectionRandomly()
    {
        // Sorteia uma direção
        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        Vector2 candidate = directions[Random.Range(0, directions.Length)];

        // Só muda se a nova direção NÃO tiver parede (não queremos que ele vire de cara na parede)
        RaycastHit2D hit = Physics2D.Raycast(transform.position, candidate, 0.6f, m_ObstacleLayer);

        if (hit.collider == null)
        {
            m_CurrentDirection = candidate;
        }
    }

    private void ChooseNewDirection()
    {
        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        m_CurrentDirection = directions[Random.Range(0, directions.Length)];
    }

    protected override bool CanWalk() => IsAlive;
    protected override void Walk() { }

    protected override void DieLogic()
    {
        m_Rb.linearVelocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
        GameplayManager.OnEnemyDied?.Invoke(transform.position);
        Destroy(gameObject, 1f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            BaseCharacter player = other.GetComponent<BaseCharacter>();
            if (player != null && player.IsAlive)
            {
                player.TakeDamage(1);
            }
        }
    }
}