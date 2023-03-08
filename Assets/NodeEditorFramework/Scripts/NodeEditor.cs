using UnityEditor;
using UnityEngine;

namespace NodeEditorFramework
{
    public class NodeEditor : EditorWindow
    {
        private static NodeEditor m_Instance;
        public static NodeEditor Instance => m_Instance;

        private Vector2 m_offset;
        private Vector2 m_drag;

        private float m_scale = 1;


        [MenuItem("Window/Node Editor")]
        static void CreateEditor()
        {
            m_Instance = EditorWindow.GetWindow<NodeEditor>();
            m_Instance.minSize = new Vector2(800, 600);

            Instance.m_scale = 1;
        }

        private void OnGUI()
        {
            DrawGrid(20, .2f, Color.gray);
            DrawGrid(100, .2f, Color.gray);

            ProcessEvents(Event.current);

            if (GUI.changed)
                Repaint();

            // draw the nodes
            //BeginWindows();
        }

        public void ProcessEvents(Event e)
        {
            m_drag = Vector2.zero;
            switch (e.type)
            {
                case EventType.MouseDown:
                    break;

                case EventType.MouseDrag:
                    if (e.button == 0)
                        OnDrag(e.delta);
                    break;

                case EventType.ScrollWheel:
                    float scrollDelta = e.delta.y;
                    OnScroll(scrollDelta);
                    break;
            }

        }

        public void OnDrag(Vector2 delta)
        {
            m_drag = delta;
            GUI.changed = true;
        }

        public void OnScroll(float delta)
        {
            m_scale += delta * Time.deltaTime;
            GUI.changed = true;
        }

        private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
        {
            int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
            int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

            Handles.BeginGUI();
            Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);
            m_offset += m_drag * 0.5f;
            Vector3 newOffset = new Vector3(m_offset.x % gridSpacing, m_offset.y % gridSpacing, 0);

            for (int i = 0; i < widthDivs; i++)
            {
                Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
            }

            for (int j = 0; j < heightDivs; j++)
            {
                Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
            }

            Handles.color = Color.white;
            Handles.EndGUI();
        }
    }
}