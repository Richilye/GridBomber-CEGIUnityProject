using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
    // Apenas para o código compilar por enquanto
    public void SetOwner(BaseCharacter owner) { }
    public void SetActive(bool value) { gameObject.SetActive(value); }
    public virtual bool CanUse() { return true; }
    public abstract void Use();
}