using UnityEngine;
using UnityEngine.InputSystem;

public class Player : BaseCharacter
{
    [Header("Input Settings")]
    [SerializeField] private PlayerInput m_playerInput;

    private Rigidbody2D m_Rb;
    private Vector3 m_originalScale;

    protected override void Setup()
    {
        m_Rb = GetComponent<Rigidbody2D>();
        m_originalScale = transform.localScale;

        Health = MaxHealth;
        // Se tiver UI Manager no futuro, descomente a linha abaixo
        // GameplayManager.OnPlayerHealthChanged?.Invoke(Health, MaxHealth);

        InitializeWeapons(); // <--- O PULO DO GATO ESTÁ AQUI

        if (m_playerInput != null)
        {
            m_playerInput.actions["Move"].performed += ctx => HandleMovementInput(ctx.ReadValue<Vector2>());
            m_playerInput.actions["Move"].canceled += ctx => HandleMovementInput(Vector2.zero);
            m_playerInput.actions["Attack"].performed += ctx => Attack();
        }
    }

    // Função nova para equipar a primeira arma (o BombPlacer)
    private void InitializeWeapons()
    {
        if (m_Weapons != null && m_Weapons.Length > 0)
        {
            // Pega a primeira arma da lista (o BombPlacer que você arrastou)
            m_CurrentWeapon = m_Weapons[0];
            m_CurrentWeapon.SetActive(true);
        }
    }

    private void HandleMovementInput(Vector2 direction)
    {
        m_MovementDirection = direction;

        if (m_MovementDirection == Vector2.zero)
        {
            if (m_Animator) m_Animator.SetInteger("Speed", 0);
        }
        else
        {
            if (m_MovementDirection.x != 0)
            {
                float facingDirection = Mathf.Sign(m_MovementDirection.x);
                transform.localScale = new Vector3(facingDirection * Mathf.Abs(m_originalScale.x), m_originalScale.y, m_originalScale.z);
            }
            if (m_Animator) m_Animator.SetInteger("Speed", 1);
        }
    }

    protected override void UpdateLogic() { }

    private void FixedUpdate()
    {
        if (CanWalk())
        {
            // Usando velocity direto para Unity 2022/2023. 
            // Se estiver na Unity 6 Preview e der erro, mude para linearVelocity
            m_Rb.linearVelocity = m_MovementDirection * m_Speed;
        }
        else
        {
            m_Rb.linearVelocity = Vector2.zero;
        }
    }

    protected override bool CanWalk()
    {
        return IsAlive;
    }

    protected override void Walk() { }

    public override void Attack()
    {
        if (!IsAlive) return;

        Debug.Log("Tentou colocar bomba!");

        // Agora isso vai funcionar porque m_CurrentWeapon não é mais null!
        base.Attack();
    }

    protected override void DieLogic()
    {
        m_Rb.linearVelocity = Vector2.zero;
        GameplayManager.OnPlayerDied?.Invoke();
        GetComponent<Collider2D>().enabled = false;
    }
}