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

        [SerializeField] public HealthComponent m_HealthComp;

        //allows the data source path and data source to access the transform position x and y
        [CreateProperty] public float XPos => transform.position.x;

        [CreateProperty] public float YPos => transform.position.y;

        //Makes the health bar width dynamic - works even if max health changes
        [CreateProperty]
        public float HealthPercent =>
            (m_HealthComp.GetHealth / m_HealthComp.GetMaxHealth) * 100f;

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