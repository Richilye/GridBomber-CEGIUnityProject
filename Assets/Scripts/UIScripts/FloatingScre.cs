using UnityEngine;
using TMPro;

public class FloatingScore : MonoBehaviour
{
    [SerializeField] private float m_MoveSpeed = 2f;
    [SerializeField] private float m_FadeTime = 1f;

    private TextMeshPro m_Text;
    private Color m_StartColor;
    private float m_Timer;

    private void Awake()
    {
        m_Text = GetComponent<TextMeshPro>();
        m_StartColor = m_Text.color;
    }

    public void Setup(int score)
    {
        m_Text.text = score.ToString();
        // Destrói automaticamente após o tempo
        Destroy(gameObject, m_FadeTime);
    }

    private void Update()
    {
        // 1. Sobe
        transform.Translate(Vector3.up * m_MoveSpeed * Time.deltaTime);

        // 2. Desaparece (Fade Out)
        m_Timer += Time.deltaTime;
        float alpha = Mathf.Lerp(1f, 0f, m_Timer / m_FadeTime);
        m_Text.color = new Color(m_StartColor.r, m_StartColor.g, m_StartColor.b, alpha);
    }
}