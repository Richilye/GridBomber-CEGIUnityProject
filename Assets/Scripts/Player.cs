using UnityEngine;
using UnityEngine.InputSystem;

public class Player : BaseCharacter
{
    [Header("Input Settings")]
    [SerializeField] private PlayerInput m_playerInput;

    // Referência para o corpo físico
    private Rigidbody2D m_Rb;

    // Variáveis internas para animação
    private Vector3 m_originalScale;

    protected override void Setup()
    {
        // Pega o componente Rigidbody2D que adicionamos na Unity
        m_Rb = GetComponent<Rigidbody2D>();
        
        // Pega a escala original para virar o personagem depois
        m_originalScale = transform.localScale;

        // Configura vida inicial (Código do professor)
        Health = MaxHealth;
        GameplayManager.OnPlayerHealthChanged?.Invoke(Health, MaxHealth); //

        // --- CONFIGURAÇÃO DOS INPUTS ---
        // O professor usava eventos (performed/canceled). 
        // Vamos manter essa estrutura, mas armazenar o valor para usar no FixedUpdate.
        
        if (m_playerInput != null)
        {
            // Ação de Mover
            m_playerInput.actions["Move"].performed += ctx => HandleMovementInput(ctx.ReadValue<Vector2>());
            m_playerInput.actions["Move"].canceled += ctx => HandleMovementInput(Vector2.zero);

            // Ação de Atacar (Colocar Bomba)
            m_playerInput.actions["Attack"].performed += ctx => Attack();
        }
    }

    private void HandleMovementInput(Vector2 direction)
    {
        m_MovementDirection = direction;

        // Lógica de Animação (Vira o sprite e seta a velocidade do Animator)
        if (m_MovementDirection == Vector2.zero)
        {
            m_Animator.SetInteger("Speed", 0);
        }
        else
        {
            // Vira o personagem horizontalmente baseado na direção X
            if (m_MovementDirection.x != 0)
            {
                float facingDirection = Mathf.Sign(m_MovementDirection.x);
                transform.localScale = new Vector3(facingDirection * Mathf.Abs(m_originalScale.x), m_originalScale.y, m_originalScale.z);
            }
            m_Animator.SetInteger("Speed", 1);
        }
    }

    // O UpdateLogic é chamado no Update do BaseCharacter.
    // Para física (Rigidbody), o ideal é mover no FixedUpdate, mas para esse projeto simples,
    // vamos controlar a velocidade aqui ou sobrescrever o UpdateLogic.
    // Vamos ignorar o UpdateLogic do pai para movimento e usar FixedUpdate para física.
    protected override void UpdateLogic()
    {
        // Deixamos vazio pois moveremos no FixedUpdate para evitar bugs de colisão
    }

    private void FixedUpdate()
    {
        if (CanWalk())
        {
            MovePhysics();
        }
        else
        {
            // Se não pode andar, para o corpo
            m_Rb.linearVelocity = Vector2.zero; 
            // OBS: Na Unity 6 "velocity" virou "linearVelocity". 
            // Se der erro, troque por "m_Rb.velocity = Vector2.zero;"
        }
    }

    private void MovePhysics()
    {
        // Mover usando Física: Alteramos a velocidade do corpo diretamente.
        // Isso impede que ele atravesse paredes e respeita a colisão.
        m_Rb.linearVelocity = m_MovementDirection * m_Speed;
        // OBS: Se der erro na Unity 6, troque por "m_Rb.velocity = ..."
    }

    protected override bool CanWalk()
    {
        return IsAlive; // Só anda se estiver vivo
    }

    protected override void Walk()
    {
        // Não usado mais, pois substituímos pelo MovePhysics no FixedUpdate
    }

    public override void Attack()
    {
        if (!IsAlive) return;

        // Aqui futuramente chamaremos o código de colocar a bomba
        // Por enquanto, deixamos o log para testar
        Debug.Log("Tentou colocar bomba!");
        
        base.Attack(); // Chama lógica base se tiver arma equipada
    }

    protected override void DieLogic()
    {
        // Para o corpo imediatamente
        m_Rb.linearVelocity = Vector2.zero;
        // OBS: Se der erro, troque por "velocity"

        // Avisa o jogo que morreu
        GameplayManager.OnPlayerDied?.Invoke(); //
        
        // Desativa colisões para não atrapalhar
        GetComponent<Collider2D>().enabled = false;
    }

    // Implementação obrigatória da classe abstrata, mas bombas não usam "CanAttack" da mesma forma que armas
    protected override bool CanAttack()
    {
        return true; 
    }
}
