#region

using Assessment_2_Scripts.Player;
using UnityEngine;

#endregion

namespace Assessment_2_Scripts.Objects
{
    public class SpikeTrap : MonoBehaviour
    {
        [SerializeField] protected float m_DamageAmount = 50f;

        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.transform.TryGetComponent<HealthComponent>(
                    out var health) //checks base obj 
                || collision.transform.parent.TryGetComponent<HealthComponent>(
                    out health)) //checks parent obj
            {
                health.ApplyDamage(m_DamageAmount, this);
            }
        }
    }
}