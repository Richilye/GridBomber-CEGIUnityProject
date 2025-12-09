using UnityEngine;

// Herda de BaseCharacter, assim como o Player
public class BaseEnemy : BaseCharacter
{
    protected override void Setup()
    {
        // Configuração inicial do inimigo (faremos depois)
    }

    protected override void UpdateLogic()
    {
        // IA do inimigo (faremos depois)
    }

    protected override bool CanWalk()
    {
        return true;
    }

    protected override void Walk()
    {
        // Movimento (faremos depois)
    }

    protected override void DieLogic()
    {
        // Som de morte, pontuação, etc.
        Destroy(gameObject);
    }
}