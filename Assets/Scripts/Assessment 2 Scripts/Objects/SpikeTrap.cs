#region

using Assessment_2_Scripts.Player;
using UnityEngine;

#endregion

namespace Assessment_2_Scripts.Objects
{
    public class SpikeTrap : MonoBehaviour
    {
        [SerializeField] private float m_DamageAmount = 50f;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.parent.TryGetComponent<HealthComponent>(
                    out var healthComponent)) //check if the parent object has a health component
            {
                healthComponent.ApplyDamage(m_DamageAmount, this);
            }
        }
    }
}