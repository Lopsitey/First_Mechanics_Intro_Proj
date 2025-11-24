#region

using UnityEngine;

#endregion

namespace Assessment_2_Scripts.Player
{
    public class HealthComponent : MonoBehaviour
    {
        // Delegate type for health change event
        public delegate void
            OnDamagedHandler(float currentHealth, float maxHealth, float damage);

        public event
            OnDamagedHandler OnDamaged; // Event triggered when damaged

        public delegate void
            OnDeathHandler(MonoBehaviour instigator);

        public event
            OnDeathHandler OnDeath;

        [SerializeField] private float m_MaxHealth = 100f;
        private float m_CurrentHealth;
        public float GetHealth => m_CurrentHealth; //essentially a get function disguised as a variable
        public float GetMaxHealth => m_MaxHealth;

        private void Start()
        {
            m_CurrentHealth = m_MaxHealth;
        }

        public void ApplyDamage(float damage, MonoBehaviour instigator)
        {
            float change = Mathf.Clamp(damage, 0f, m_CurrentHealth);
            m_CurrentHealth -= change;

            OnDamaged?.Invoke(m_CurrentHealth, m_MaxHealth, damage);
            if (m_CurrentHealth <= 0)
                OnDeath?.Invoke(instigator);
        }
    }
}