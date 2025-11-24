#region

using System.ComponentModel;
using Unity.Properties;
using UnityEngine;

#endregion

namespace Assessment_2_Scripts.Player
{
    public class PlayerWrapper : MonoBehaviour, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private HealthComponent m_HealthComp;

        //allows the data source path and data source to access the transform position x and y
        //Evaluates to false if the object is destroyed
        [CreateProperty] public float XPos => this ? transform.position.x : 0f;
        [CreateProperty] public float YPos => this ? transform.position.y : 0f;

        //Makes the health bar width dynamic - works even if max health changes
        [CreateProperty]
        public float HealthPercent =>
            (m_HealthComp.GetHealth / m_HealthComp.GetMaxHealth) * 100f;

        void Awake()
        {
            m_HealthComp = GetComponent<HealthComponent>();
        }

        void OnEnable()
        {
            if (m_HealthComp != null)
            {
                // 4. Subscribe to the event
                m_HealthComp.OnDamaged += Handle_OnDamaged;
            }
        }

        void OnDisable()
        {
            if (m_HealthComp != null)
            {
                m_HealthComp.OnDamaged -= Handle_OnDamaged;
            }
        }

        private void Handle_OnDamaged(float current, float dmg, float damage)
        {
            //Updates binding on value change
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("healthPercent"));
        }
    }
}