#region

using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

#endregion

namespace Assessment_2_Scripts.Objects
{
    public class LightFlicker : MonoBehaviour
    {
        [FormerlySerializedAs("light2D")] public Light2D m_Light2D; //does this need to be of type SpotLight?

        [FormerlySerializedAs("m_minIntensity")] [Header("Flicker Settings")] [SerializeField]
        private float m_MinIntensity = 1.5f;

        [FormerlySerializedAs("m_maxIntensity")] [SerializeField]
        private float m_MaxIntensity = 2.3f;

        [FormerlySerializedAs("m_radiusJitter")] [SerializeField]
        private float m_RadiusJitter = 0.25f;

        [FormerlySerializedAs("m_flickerRate")] [SerializeField]
        private float m_FlickerRate = 0.3f;

        [FormerlySerializedAs("m_colourShift")] [SerializeField]
        private float m_ColourShift = 0.1f;

        private float m_BaseOuterRadius = 20.0f;
        private Color m_BaseColour = Color.orangeRed;

        void Awake()
        {
            if (!m_Light2D) m_Light2D = GetComponent<Light2D>();
            m_BaseColour = m_Light2D.color;
            m_BaseOuterRadius = m_Light2D.pointLightOuterRadius;
        }

        void FixedUpdate()
        {
            float t = Time.time * m_FlickerRate;

            m_Light2D.intensity = Mathf.Lerp(m_MinIntensity, m_MaxIntensity, Mathf.PerlinNoise(t, 0.0f));
            m_Light2D.pointLightOuterRadius =
                m_BaseOuterRadius + (Mathf.PerlinNoise(0.0f, t * 0.7f) - 0.5f) * m_RadiusJitter;

            var cooler = m_BaseColour * new Color(1.0f, 0.95f, 0.85f, 1f);
            var warmer = m_BaseColour * new Color(1.1f, 1.0f, 0.9f, 1f);
            m_Light2D.color = Color.Lerp(cooler, warmer,
                Mathf.PerlinNoise(t, 0.0f) * m_ColourShift + (1.0f - m_ColourShift));
        }
    }
}