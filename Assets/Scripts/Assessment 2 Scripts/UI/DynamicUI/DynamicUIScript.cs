#region

using System;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

#endregion

namespace Assessment_2_Scripts.UI.DynamicUI
{
    public class DynamicScript : MonoBehaviour
    {
        private VisualElement m_UIRoot;
        private ScrollView m_ScrollArea;

        private void Awake()
        {
            VisualElement rootElement = new VisualElement();
            rootElement.name = "container";

            GetComponent<UIDocument>().rootVisualElement.Add(rootElement);
            m_UIRoot = rootElement;

            Button button = new Button();
            button.text = "Press Me";
            button.name = "btn_labelspawner";
            button.AddToClassList("button");
            button.RegisterCallback<ClickEvent>(Handle_LabelSpawn);
            m_UIRoot.Add(button);
        }

        private void Handle_LabelSpawn(ClickEvent clickEvent)
        {
            //TODO could potentially use this for creating a confirmation popup widget
            Debug.Log("Handle_LabelSpawn clicked");
            if (m_ScrollArea == null)
            {
                ScrollView list = new ScrollView();
                list.name = "scr_labelcontainer";
                list.AddToClassList("scrollview");

                m_UIRoot.Add(list);
                m_ScrollArea = list;
            }

            Label label = new Label();
            label.name = "lbl_randomnumberlabel";
            label.text = Random.Range(Int32.MinValue, Int32.MaxValue).ToString();
            label.AddToClassList("label");
            m_ScrollArea.Add(label);
        }
    }
}