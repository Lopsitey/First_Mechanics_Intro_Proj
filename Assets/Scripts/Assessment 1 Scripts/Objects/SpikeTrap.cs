#region

using Assessment_1_Scripts.Player;
using UnityEngine;

#endregion

namespace Assessment_1_Scripts.Objects
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
        } //TODO forum post on this and the future of combat/damage etc
    }
}