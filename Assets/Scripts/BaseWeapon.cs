using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
    [Header("Configurações Base")]
    [SerializeField] protected int m_Damage = 10;
    [SerializeField] protected float m_AttackCooldown = 0.5f;

    // Referências
    protected BaseCharacter m_Owner;
    protected float m_LastAttackTime = -999f; // Começa permitindo ataque imediato

    public virtual void SetOwner(BaseCharacter owner)
    {
        m_Owner = owner;
    }

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }

    public virtual bool CanUse()
    {
        if (m_Owner == null) return false;

        // Verifica se passou tempo suficiente desde o último ataque
        if (Time.time - m_LastAttackTime < m_AttackCooldown) return false;

        return true;
    }

    public abstract void Use();
}