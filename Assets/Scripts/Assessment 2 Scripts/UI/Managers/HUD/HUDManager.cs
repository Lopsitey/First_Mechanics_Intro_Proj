#region

using Assessment_2_Scripts.Managers;
using Assessment_2_Scripts.Player;
using Unity.Properties;
using UnityEngine.UIElements;

#endregion

namespace Assessment_2_Scripts.UI.Managers.HUD
{
    public class HUDManager : Singleton<HUDManager>
    {
        private VisualElement m_CurrentContainer;

        public void CreateHUD(PlayerWrapper player)
        {
            //Kills old HUD is one already exists
            if (m_CurrentContainer != null)
            {
                m_CurrentContainer.RemoveFromHierarchy();
            }

            if (player) //essentially if the player exists
            {
                //main container element
                m_CurrentContainer = new VisualElement();
                m_CurrentContainer.name = "container";
                GetComponent<UIDocument>().rootVisualElement.Add(m_CurrentContainer); //adds it to the document

                #region Coordinates

                VisualElement labelcontainer = new VisualElement();
                labelcontainer.name = "labelcontainer";
                labelcontainer.AddToClassList("positionlabelcontainer");
                m_CurrentContainer.Add(labelcontainer);

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


                DataBinding xBinding = new DataBinding
                {
                    dataSource = player,
                    dataSourcePath = new PropertyPath("XPos"),
                    bindingMode = BindingMode.ToTarget
                };
                xBinding.updateTrigger = BindingUpdateTrigger.OnSourceChanged;
                xBinding.sourceToUiConverters.AddConverter((ref float value) =>
                    $"X: {value:F2}"); //F2 formats the float to 2 decimal places

                xLabel.SetBinding("text", xBinding);


                DataBinding yBinding = new DataBinding
                {
                    dataSource = player,
                    dataSourcePath = new PropertyPath("YPos"),
                    bindingMode = BindingMode.ToTarget
                };
                yBinding.updateTrigger = BindingUpdateTrigger.OnSourceChanged;
                yBinding.sourceToUiConverters.AddConverter((ref float value) =>
                    $"Y: {value:F2}");
                yLabel.SetBinding("text", yBinding);

                #endregion

                #region Healthbar

                VisualElement healthContainer = new VisualElement();
                healthContainer.name = "healthContainer";
                healthContainer.AddToClassList("healthbarcontainer");
                m_CurrentContainer.Add(healthContainer);

                VisualElement healthFill = new VisualElement();
                healthFill.name = "healthFill";
                healthFill.AddToClassList("healthfill");
                healthContainer.Add(healthFill);

                DataBinding healthBinding = new DataBinding
                {
                    dataSource = player,
                    dataSourcePath = new PropertyPath("HealthPercent"), //Takes the health var from the wrapper
                    bindingMode = BindingMode.ToTarget
                };


                xBinding.updateTrigger = BindingUpdateTrigger.OnSourceChanged;
                //Converts the health into a CSS length percentage
                healthBinding.sourceToUiConverters.AddConverter((ref float currentHealth) =>
                    new StyleLength(new Length(currentHealth, LengthUnit.Percent)));

                //Binds to the width property of the style
                healthFill.SetBinding("style.width", healthBinding);

                #endregion
            }
        }

        public void UpdateHUD(PlayerWrapper player)
        {
        }
    }
}