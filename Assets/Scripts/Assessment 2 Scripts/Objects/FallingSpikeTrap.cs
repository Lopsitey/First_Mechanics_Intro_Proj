#region

using System.Collections;
using Assessment_2_Scripts.Managers;
using Assessment_2_Scripts.Utilities;
using UnityEngine;

#endregion

namespace Assessment_2_Scripts.Objects
{
    public class FallingSpikeTrap : SpikeTrap
    {
        [Header("Spike Settings")] [SerializeField]
        private float m_ReactionTime = 0.5f; //the delay before the spike falls

        [SerializeField] private float m_FallGravity = 1;
        [SerializeField] private float m_DestroyDelay = 0.25f;
        [SerializeField] private AudioClip m_ShakeClip;
        [SerializeField] private AudioClip m_HitClip;

        [Header("Miscellaneous")] [SerializeField]
        private LayerMask m_GroundLayer;

        private enum TrapState
        {
            Armed,
            Falling,
            Fell
        }

        private TrapState m_CurrentState;

        // ReSharper disable once InconsistentNaming
        private Rigidbody2D m_RB;

        private GameObject m_Player;

        private void Awake()
        {
            m_RB = GetComponent<Rigidbody2D>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            //Can only fall once
            if (m_CurrentState == TrapState.Armed && other.CompareTag("PlayerCol"))
            {
                m_Player = other.transform.parent.gameObject;
                StartCoroutine(AlarmAndFall());
            }
        }

        protected override void OnCollisionEnter2D(Collision2D other)
        {
            if (m_CurrentState == TrapState.Falling)
            {
                if (other.gameObject == m_Player)
                {
                    base.OnCollisionEnter2D(other);

                    //disables collision so the spike can fall through the player slightly
                    Physics2D.IgnoreCollision(other.collider, other.otherCollider, true);
                    //Makes the spike hit look a bit better by disabling collision and delaying death
                    Destroy(gameObject, m_DestroyDelay);
                }

                if (GameHelpers.IsLayerInMask(other.gameObject.layer, m_GroundLayer))
                {
                    if (GameHelpers.ALMOST_ZERO(m_RB.linearVelocity, 1f))
                    {
                        Destroy(gameObject); //dies if it hits the ground and is still
                    }
                }

                AudioManager.Instance.PlaySFX(m_HitClip, 0.4f);
                m_CurrentState = TrapState.Fell; //prevents checking collisions multiple times
            }
        }

        /// <summary>
        /// Plays a sound before falling
        /// </summary>
        /// <returns></returns>
        private IEnumerator AlarmAndFall()
        {
            //plays audio when disturbed
            AudioManager.Instance.PlaySFX(m_ShakeClip, 0.1f); //very loud clip made quiet
            // Visual shake logic here...
            yield return new WaitForSeconds(m_ReactionTime);

            //Enables physics and gravity to fall
            m_RB.bodyType = RigidbodyType2D.Dynamic;
            m_RB.gravityScale = m_FallGravity;
            m_RB.mass = m_Player.GetComponent<Rigidbody2D>().mass * 100; //ensures the spike has unstoppable mass
            m_RB.freezeRotation = true; //makes the spike fall straight
            m_CurrentState = TrapState.Falling;
        }
    }
}