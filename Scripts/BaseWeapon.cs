using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
    [Header("Configurações Base")]
    [SerializeField] protected int m_Damage = 10;
    [SerializeField] protected float m_AttackCooldown = 0.5f;

    protected BaseCharacter m_Owner;
    protected float m_LastAttackTime = -999f;

    public virtual void SetOwner(BaseCharacter owner)
    {
        m_Owner = owner;
    }

    public void SetActive(bool value)
    {
        // A versão do professor exigia um objeto m_Root aqui.
        // A nossa versão apenas ativa/desativa o componente/gameobject inteiro.
        gameObject.SetActive(value);
    }

    public virtual bool CanUse()
    {
        // TRAVA 1: Tem dono?
        if (m_Owner == null)
        {
            Debug.LogError($"A arma {name} não tem Dono! O Player não chamou SetOwner.");
            return false;
        }

        // TRAVA 2: Cooldown (Tempo entre bombas)
        if (Time.time - m_LastAttackTime < m_AttackCooldown)
        {
            return false; // Ainda está recarregando
        }

        return true;
    }

    public abstract void Use();
}