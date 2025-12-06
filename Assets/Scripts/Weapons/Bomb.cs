using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private float m_FuseTime = 3f; // Tempo até explodir
    [SerializeField] private int m_Damage = 100; // Dano (instakill geralmente)

    // Área de explosão (quantos blocos para cada lado)
    private int m_ExplosionRange;

    public void Setup(int damage, int range)
    {
        m_Damage = damage;
        m_ExplosionRange = range;

        // Começa a contagem regressiva assim que nasce
        StartCoroutine(ExplodeRoutine());
    }

    private IEnumerator ExplodeRoutine()
    {
        // Espera o tempo do pavio
        yield return new WaitForSeconds(m_FuseTime);

        Explode();
    }

    private void Explode()
    {
        Debug.Log("BOOM! Explosão range: " + m_ExplosionRange);

        // Aqui futuramente vamos criar os objetos de "Fogo" nas direções
        // Por enquanto, apenas destruímos a bomba
        Destroy(gameObject);
    }
}