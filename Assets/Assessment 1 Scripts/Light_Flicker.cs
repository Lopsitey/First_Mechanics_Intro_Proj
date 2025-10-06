using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.Rendering.Universal;
public class Light_Flicker : MonoBehaviour
{
    public Light2D light2D;//does this need to be of type SpotLight?
    [Header("Flicker Settings")]
    [SerializeField] private float m_minIntensity = 1.5f;
    [SerializeField] private float m_maxIntensity = 2.3f;
    [SerializeField] private float m_radiusJitter = 0.25f;
    [SerializeField] private float m_flickerRate = 0.3f;
    [SerializeField] private float m_colourShift = 0.1f;
    
    private float m_baseOuterRadius = 20.0f;
    private Color baseColour = Color.orangeRed;
    
    void Awake()
    {
        if (!light2D) light2D = GetComponent<Light2D>();
        baseColour = light2D.color;
        m_baseOuterRadius = light2D.pointLightOuterRadius;
    }
    
    void FixedUpdate()
    {
        float t = Time.time * m_flickerRate;
        
        light2D.intensity = Mathf.Lerp(m_minIntensity, m_maxIntensity, Mathf.PerlinNoise(t, 0.0f));
        light2D.pointLightOuterRadius = m_baseOuterRadius + (Mathf.PerlinNoise(0.0f, t * 0.7f) - 0.5f) * m_radiusJitter;
        
        var cooler = baseColour * new Color(1.0f,0.95f,0.85f,1f);
        var warmer = baseColour * new Color(1.1f, 1.0f, 0.9f, 1f);
        light2D.color = Color.Lerp(cooler, warmer, Mathf.PerlinNoise(t, 0.0f) * m_colourShift + (1.0f - m_colourShift));
    }
}
