#region

using UnityEngine;

#endregion

namespace Assessment_2_Scripts.Player
{
    public class CharacterManager : MonoBehaviour
    {
        private HealthComponent m_HealthComp;

        private void Awake()
        {
            m_HealthComp = GetComponent<HealthComponent>();
            Debug.Assert(m_HealthComp != null, "Health Component not found on Player Character");
        }

        public void Init()
        {
            m_HealthComp.OnDamaged += Handle_OnDamaged;
            m_HealthComp.OnDeath += Handle_OnDeath;
        }

        private void Handle_OnDamaged(float currentHealth, float maxHealth, float damage)
        {
            Debug.Log($"Player Damaged: {damage} Current Health: {currentHealth}/{maxHealth}");
        }

        private void Handle_OnDeath(MonoBehaviour instigator)
        {
            Debug.Log($"Player Death: {instigator.name}");
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            m_HealthComp.OnDamaged -= Handle_OnDamaged;
            m_HealthComp.OnDeath -= Handle_OnDeath;
        }
    }
}