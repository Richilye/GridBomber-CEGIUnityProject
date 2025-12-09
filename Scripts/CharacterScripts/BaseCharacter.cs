using System.Collections;
using UnityEngine;

public abstract class BaseCharacter : MonoBehaviour
{
    [field: SerializeField] public string Name { get; protected set; }
    [field: SerializeField] public int MaxHealth { get; protected set; } = 3;

    [SerializeField] protected AudioClip m_DeathSound;

    // Configurações de Dano e Invencibilidade
    [SerializeField] protected float m_InvincibilityDuration = 2f;
    [SerializeField] protected float m_Speed = 5f;
    [SerializeField] protected Animator m_Animator;
    [SerializeField] protected BaseWeapon[] m_Weapons;

    public int Health { get; protected set; }
    public bool IsAlive { get; protected set; }

    // Controle interno de invencibilidade
    protected bool m_IsInvincible = false;
    protected SpriteRenderer m_SpriteRenderer;

    protected Vector2 m_MovementDirection;
    protected BaseWeapon m_CurrentWeapon;

    public void Initialize()
    {
        IsAlive = true;
        m_SpriteRenderer = GetComponent<SpriteRenderer>(); // Pega o sprite para fazer piscar
        Setup();
    }

    protected abstract void Setup();

    protected virtual void Update()
    {
        if (!IsAlive) return;
        UpdateLogic();
    }
    protected abstract void UpdateLogic();
    protected abstract bool CanWalk();
    protected abstract void Walk();

    public virtual void Attack()
    {
        if (m_CurrentWeapon == null) return;
        if (CanAttack())
        {
            m_CurrentWeapon.Use();
        }
    }

    protected virtual bool CanAttack()
    {
        if (m_CurrentWeapon != null && !m_CurrentWeapon.CanUse())
            return false;
        return true;
    }

    // --- NOVA LÓGICA DE DANO ---
    public virtual void TakeDamage(int damage)
    {
        if (!IsAlive) return;
        if (m_IsInvincible) return; // Se está invencível, ignora o dano!

        Health -= damage;

        if (m_Animator) m_Animator.SetTrigger("Hit");
        Debug.Log($"{gameObject.name} tomou {damage} de dano. Vida: {Health}");

        if (Health <= 0)
        {
            Die();
        }
        else
        {
            // Se sobreviveu, fica invencível por um tempo
            StartCoroutine(HandleInvincibility());
        }
    }

    private IEnumerator HandleInvincibility()
    {
        m_IsInvincible = true;

        // Efeito visual de piscar (opcional, mas recomendado)
        float timer = 0;
        while (timer < m_InvincibilityDuration)
        {
            if (m_SpriteRenderer) m_SpriteRenderer.enabled = !m_SpriteRenderer.enabled; // Pisca
            yield return new WaitForSeconds(0.1f);
            timer += 0.1f;
        }

        if (m_SpriteRenderer) m_SpriteRenderer.enabled = true; // Garante que fica visível no final
        m_IsInvincible = false;
    }

    protected virtual void Die()
    {
        if (!IsAlive) return;
        //tocar som de morrer
        if (m_DeathSound) GameplayManager.Instance.PlaySFX(m_DeathSound);
        IsAlive = false;
        DieLogic();
        if (m_Animator) m_Animator.SetBool("Dead", true);

        // Removemos o Destroy imediato aqui para dar chance ao Player de usar vidas extras
        // O Destroy será chamado manualmente nos filhos se necessário
    }

    protected abstract void DieLogic();
}