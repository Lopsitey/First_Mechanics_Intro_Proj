#region

using Assessment_1_Scripts.Managers;
using Assessment_1_Scripts.Player;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

#endregion

namespace Assessment_1_Scripts.UI.HUD
{
    public class HUDScript : MonoBehaviour
    {
        [SerializeField] private GameManager m_GameManager;
        private HealthComponent m_HealthComp;

        void Awake()
        {
            if (m_GameManager)
                m_GameManager.FinishInit += CreateHUD; //subscribes to the finish init event

            if (m_GameManager.TryGetPlayerHealthComp(out m_HealthComp))
                m_HealthComp.OnDeath += DestroyHUD; //subscribes to the player death event
        }

        private void CreateHUD()
        {
            # region UI Setup

            VisualElement container = new VisualElement();
            container.name = "container";
            GetComponent<UIDocument>().rootVisualElement.Add(container);

            VisualElement labelcontainer = new VisualElement();
            labelcontainer.name = "labelcontainer";
            labelcontainer.AddToClassList("positionlabelcontainer");
            container.Add(labelcontainer);

            Label xLabel = new Label();
            xLabel.name = "xlabel";
            xLabel.text = "X: ____";
            xLabel.AddToClassList("positionlabel");
            labelcontainer.Add(xLabel);

            Label yLabel = new Label();
            yLabel.name = "ylabel";
            yLabel.text = "Y: ____";
            yLabel.AddToClassList("positionlabel");
            labelcontainer.Add(yLabel);

            # endregion

            #region Binding Setup

            TransformWrapper playerTransform =
                GameObject.FindGameObjectWithTag("Player").GetComponent<TransformWrapper>();

            DataBinding xBinding = new DataBinding
            {
                dataSource = playerTransform,
                dataSourcePath = new PropertyPath("xPos"),
                bindingMode = BindingMode.ToTarget
            };
            xBinding.updateTrigger = BindingUpdateTrigger.OnSourceChanged;
            xBinding.sourceToUiConverters.AddConverter((ref float value) =>
                $"X: {value:F2}"); //F2 formats the float to 2 decimal places

            xLabel.SetBinding("text", xBinding);


            DataBinding yBinding = new DataBinding
            {
                dataSource = playerTransform,
                dataSourcePath = new PropertyPath("yPos"),
                bindingMode = BindingMode.ToTarget
            };
            yBinding.updateTrigger = BindingUpdateTrigger.OnSourceChanged;
            yBinding.sourceToUiConverters.AddConverter((ref float value) =>
                $"Y: {value:F2}");
            yLabel.SetBinding("text", yBinding);

            #endregion
        }

        private void DestroyHUD(MonoBehaviour instigator)
        {
            //destroys the HUD when the player dies
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (m_GameManager && m_HealthComp)
            {
                m_GameManager.FinishInit -= CreateHUD;
                m_HealthComp.GetComponent<HealthComponent>().OnDeath -= DestroyHUD;
            }
        }
    }
}