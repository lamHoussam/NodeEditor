
using Newtonsoft.Json.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace NodeEditorFramework
{
    public class TestNode : Node
    {

        private int m_value;
        public static TestNode Create(Rect rect)
        {
            TestNode node = CreateInstance<TestNode>();

            node.name = "Camera Logic";
            node.m_Rect = rect;

            //NodeInput.Create(node, "myValue", typeof(int));

            node.Init();
            return node;
        }

        public override void Draw()
        {
            base.Draw();
            Rect boxRect = new Rect(Position, Size);

            GUI.Box(boxRect, name, m_isSelected ? NodeEditor.Instance.m_SelectedNodeBase : NodeEditor.Instance.m_NodeBase);
            //Rect rect = new Rect(Position + Vector2.up * (Height - 5), Size);
            //GUI.Box(rect, "Settings", NodeEditor.Instance.DefaultNodeStyle);

        }
    }
}