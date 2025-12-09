using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [SerializeField] private float m_Lifetime = 1f;

    private void Start()
    {
        Destroy(gameObject, m_Lifetime);
    }
}