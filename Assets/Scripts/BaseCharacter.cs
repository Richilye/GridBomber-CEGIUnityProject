using UnityEngine;

public abstract class BaseCharacter : MonoBehaviour
{
    // Dados básicos
    [field: SerializeField] public string Name { get; protected set; }
    [field: SerializeField] public int MaxHealth { get; protected set; } = 3; // Valor padrão para Bomberman
    public int Health { get; protected set; }
    public bool IsAlive { get; protected set; }

    [SerializeField] protected float m_Speed = 5f;
    [SerializeField] protected Animator m_Animator;

    [SerializeField] protected BaseWeapon[] m_Weapons; 
    protected BaseWeapon m_CurrentWeapon;
    
    protected Vector2 m_MovementDirection;

    public void Initialize()
    {
        IsAlive = true;
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
        // Lógica de ataque (colocar bomba) será sobrescrita no Player
    }

    protected virtual bool CanAttack()
    {
        return true;
    }

    public virtual void TakeDamage(int damage)
    {
        if (!IsAlive) return;

        Health -= damage;
        if (m_Animator) m_Animator.SetTrigger("Hit");
        
        Debug.Log($"{gameObject.name} tomou {damage} de dano. Vida: {Health}");

        if (Health <= 0)
        {
            Die();
        }
    }

    protected void Die()
    {
        if (!IsAlive) return;
        IsAlive = false;
        DieLogic();
        if (m_Animator) m_Animator.SetBool("Dead", true);
        
        // Destroi o objeto após 2 segundos (dá tempo da animação tocar)
        Destroy(gameObject, 2f); 
    }
    protected abstract void DieLogic();
}