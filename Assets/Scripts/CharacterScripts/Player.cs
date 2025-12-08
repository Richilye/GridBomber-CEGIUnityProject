using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : BaseCharacter
{
    [Header("Player Stats")]
    [SerializeField] private PlayerInput m_playerInput;
    [SerializeField] private int m_StartingLives = 3;

    private static int s_CurrentLives = -1;
    private Rigidbody2D m_Rb;
    private Vector3 m_originalScale;

    protected override void Setup()
    {
        m_Rb = GetComponent<Rigidbody2D>();
        m_originalScale = transform.localScale;

        if (s_CurrentLives == -1) s_CurrentLives = m_StartingLives;
        GameplayManager.OnPlayerLivesChanged?.Invoke(s_CurrentLives);
        Health = MaxHealth;

        // Atualiza UI se necessário
        // GameplayManager.OnPlayerLivesChanged?.Invoke(s_CurrentLives); 

        InitializeWeapons();

        // Avisa a UI sobre as Vidas e a Vida(HP) iniciais
        GameplayManager.OnPlayerLivesChanged?.Invoke(s_CurrentLives);
        GameplayManager.OnPlayerHealthChanged?.Invoke(Health, MaxHealth);

    }

    // --- CORREÇÃO DO INPUT SYSTEM ---
    // Usamos OnEnable e OnDisable para ligar e desligar os controles
    // Isso garante que quando o objeto morrer, os inputs param de chamar ele.

    private void OnEnable()
    {
        if (m_playerInput != null)
        {
            // Inscreve usando nomes de funções (sem =>)
            m_playerInput.actions["Move"].performed += OnMovePerformed;
            m_playerInput.actions["Move"].canceled += OnMoveCanceled;
            m_playerInput.actions["Attack"].performed += OnAttackPerformed;
        }
    }

    private void OnDisable()
    {
        if (m_playerInput != null)
        {
            // DESINSCREVE (O segredo para não dar erro!)
            m_playerInput.actions["Move"].performed -= OnMovePerformed;
            m_playerInput.actions["Move"].canceled -= OnMoveCanceled;
            m_playerInput.actions["Attack"].performed -= OnAttackPerformed;
        }
    }

    // Funções separadas para o Input (facilitam o unsubscribe)
    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        HandleMovementInput(ctx.ReadValue<Vector2>());
    }

    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        HandleMovementInput(Vector2.zero);
    }

    private void OnAttackPerformed(InputAction.CallbackContext ctx)
    {
        Attack();
    }
    // --------------------------------

    private void InitializeWeapons()
    {
        if (m_Weapons != null && m_Weapons.Length > 0 && m_Weapons[0] != null)
        {
            m_CurrentWeapon = m_Weapons[0];
            m_CurrentWeapon.SetOwner(this);
            m_CurrentWeapon.SetActive(true);
        }
    }

    private void HandleMovementInput(Vector2 direction)
    {
        m_MovementDirection = direction;

        // Se a direção for diferente de zero, estamos nos movendo
        if (m_MovementDirection != Vector2.zero)
        {
            // Avisa o Animator que estamos andando
            m_Animator.SetBool("IsMoving", true);

            // Alimenta o Blend Tree com a direção (para ele saber se toca Cima, Baixo ou Lado)
            m_Animator.SetFloat("InputX", m_MovementDirection.x);
            m_Animator.SetFloat("InputY", m_MovementDirection.y);

            // Mantemos a variável Speed antiga por segurança se tiver transições antigas
            m_Animator.SetInteger("Speed", 1);
        }
        else
        {
            // Parado
            m_Animator.SetBool("IsMoving", false);
            m_Animator.SetInteger("Speed", 0);

            // NOTA: Não zeramos o InputX/InputY aqui!
            // Se deixarmos os valores antigos, o Blend Tree "lembra" a última direção.
        }
    }

    protected override void UpdateLogic() { }

    private void FixedUpdate()
    {
        if (CanWalk())
            m_Rb.linearVelocity = m_MovementDirection * m_Speed;
        else
            m_Rb.linearVelocity = Vector2.zero;
    }

    protected override bool CanWalk() => IsAlive;
    protected override void Walk() { }

    public override void Attack()
    {
        if (!IsAlive) return;
        base.Attack();
    }

    // --- MÉTODOS DE POWER-UP (Chamados pelo ItemPickup) ---

    public void AddMaxBombs()
    {
        // Precisamos achar a arma de bomba na lista
        foreach (var weapon in m_Weapons)
        {
            if (weapon is BombPlacer placer)
            {
                placer.IncreaseMaxBombs();
            }
        }
    }

    public void AddExplosionRange()
    {
        foreach (var weapon in m_Weapons)
        {
            if (weapon is BombPlacer placer)
            {
                placer.IncreaseRange();
            }
        }
    }

    public void AddSpeed(float amount)
    {
        m_Speed += amount;
        // Limite máximo para não ficar incontrolável (opcional)
        if (m_Speed > 10f) m_Speed = 10f;
        Debug.Log("Speed Up! Nova velocidade: " + m_Speed);
    }

    public void AddLife()
    {
        s_CurrentLives++;
        GameplayManager.OnPlayerLivesChanged?.Invoke(s_CurrentLives);
    }

    public void Heal()
    {
        if (Health < 9)
        {
            Health++;
            // Atualiza UI
            GameplayManager.OnPlayerHealthChanged?.Invoke(Health, MaxHealth);
        }
    }

    public override void TakeDamage(int damage)
    {
        // Salva a vida antes do dano para saber se mudou
        int healthBefore = Health;

        // Chama a lógica padrão do pai (reduzir vida, invencibilidade, animação, morte)
        base.TakeDamage(damage);

        // Se a vida mudou (ou seja, não estava invencível), avisa a UI
        if (Health != healthBefore)
        {
            // Mathf.Max(0, Health) garante que não mande vida negativa (-1) para a UI
            GameplayManager.OnPlayerHealthChanged?.Invoke(Mathf.Max(0, Health), MaxHealth);
        }
    }

    protected override void DieLogic()
    {
        m_Rb.linearVelocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;

        s_CurrentLives--;
        GameplayManager.OnPlayerLivesChanged?.Invoke(s_CurrentLives); // Atualiza UI ao morrer
        Debug.Log($"Perdeu vida! Restam: {s_CurrentLives}");

        if (s_CurrentLives > 0)
        {
            Invoke(nameof(RestartLevel), 2f);
        }
        else
        {
            s_CurrentLives = -1;
            GameplayManager.OnPlayerDied?.Invoke();
            Destroy(gameObject, 2f);
        }
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}