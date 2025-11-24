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
        public void CreateHUD(TransformWrapper playerTransform)
        {
            if (playerTransform) //essentially if the player exists
            {
                # region Coordinates UI Setup

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
        }
    }
}