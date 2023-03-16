using UnityEngine;

namespace NodeEditorFramework
{
    public class TestNode : Node
    {

        private int m_value;

#if UNITY_EDITOR
        public static TestNode Create(Rect rect)
        {
            TestNode node = CreateInstance<TestNode>();

            node.name = "Camera Logic";
            node.m_Rect = rect;
            node.m_InitialRect = rect;

            node.Init();
            return node;
        }

        public override void Draw(float scale = 1)
        {
            base.Draw(scale);
            Rect boxRect = new Rect(Position, Size);

            GUI.Box(boxRect, name, m_isSelected ? NodeEditor.Instance.m_SelectedNodeBase : NodeEditor.Instance.m_NodeBase);

        }
#endif
    }
}