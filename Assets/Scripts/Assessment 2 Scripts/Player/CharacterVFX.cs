#region

using Assessment_1_Scripts.Player;
using UnityEngine;

#endregion

namespace Assessment_2_Scripts.Player
{
    public class CharacterVFX : MonoBehaviour
    {
        [Header("Damage VFX")] [SerializeField]
        private GameObject m_DamageVFX; //The VFX to be played

        [SerializeField] private Vector3 m_DamageOffset = new Vector3(0, 0.5f, 0); //location on the player's body
        [SerializeField] private float m_DamageAnimationLength = 0.5f;

        [Header("Aiming & Dash VFX")] [SerializeField]
        private GameObject m_ChargingDashVFX;

        [SerializeField] private Vector3 m_ChargingDashOffset = new Vector3(0, 0.5f, 0);

        [SerializeField] private GameObject m_DashVFX;
        [SerializeField] private Vector3 m_DashOffset = new Vector3(0, 0.5f, 0);
        [SerializeField] private float m_DashAnimationLength = 0.5f;

        [Header("Jumping VFX")] [SerializeField]
        private GameObject m_JumpVFX;

        [SerializeField] private Vector3 m_JumpOffset = new Vector3(0, 0.5f, 0);
        [SerializeField] private float m_JumpAnimationLength = 0.5f;

        private GameObject m_AimVFX; //so it can be delayed until the do dash is called

        private HealthComponent m_HealthComp;
        private CharacterMovement m_CharacterMovement;

        private void Awake()
        {
            if (m_HealthComp == null)
                m_HealthComp = GetComponent<HealthComponent>();

            if (m_CharacterMovement == null)
                m_CharacterMovement = GetComponent<CharacterMovement>();
        }

        private void OnEnable()
        {
            if (m_HealthComp)
                m_HealthComp.OnDamaged += PlayDamageVFX;

            if (m_CharacterMovement)
            {
                m_CharacterMovement.OnJump += PlayJumpVFX;
                m_CharacterMovement.OnAim += PlayAimVFX;
                m_CharacterMovement.OnDash += PlayDashVFX;
            }
        }

        private void OnDisable()
        {
            if (m_HealthComp)
                m_HealthComp.OnDamaged -= PlayDamageVFX;
            if (m_CharacterMovement)
            {
                m_CharacterMovement.OnJump -= PlayJumpVFX;
                m_CharacterMovement.OnAim -= PlayAimVFX;
                m_CharacterMovement.OnDash -= PlayDashVFX;
            }
        }

        private void PlayDamageVFX(float current, float max, float damage)
        {
            if (m_DamageVFX)
            {
                //spawns the vfx on the player
                SpawnVFX(m_DamageVFX, m_DamageOffset, m_DamageAnimationLength);
            }
        }

        private void PlayJumpVFX()
        {
            SpawnVFX(m_JumpVFX, m_JumpOffset, m_JumpAnimationLength);
        }

        private void PlayAimVFX()
        {
            m_AimVFX = SpawnVFX(m_ChargingDashVFX, m_ChargingDashOffset);
        }

        private void PlayDashVFX()
        {
            if (m_AimVFX)
                //only stops charging when a dash has started
                Destroy(m_AimVFX);
            SpawnVFX(m_DashVFX, m_DashOffset, m_DashAnimationLength);
        }

        private GameObject SpawnVFX(GameObject prefab, Vector3 offset, float delay = 0)
        {
            if (prefab)
            {
                //ref to the spawned obj
                GameObject instance = Instantiate(prefab, transform.position + offset, Quaternion.identity);

                //If they are follow vfx, get them to follow the player
                if (instance.TryGetComponent<VFXFollow>(out var follower))
                {
                    follower.Setup(transform, offset); //passes the players transform to followW
                }
                else
                {
                    //Automatically destroys the object after a set time
                    Destroy(instance, delay);
                }

                return instance; //For any objects that aren't destroyed immediately
            }

            return null;
        }
    }
}