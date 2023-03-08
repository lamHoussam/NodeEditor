using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace NodeEditorFramework
{
    public class NodeEditor : EditorWindow
    {
        private static NodeEditor m_Instance;
        public static NodeEditor Instance => m_Instance;

        private NodeCanvas m_LoadedNodeCanvas;
        public NodeCanvas LoadedNodeCanvas => m_LoadedNodeCanvas;

        private string m_openedCanvas = "New Canvas";
        private string m_openedCanvasPath;

        public GUIStyle m_NodeBase;
        public GUIStyle m_NodeBox;
        public GUIStyle m_NodeLabelBold;


        private Vector2 m_offset;
        private Vector2 m_drag;

        private float m_scale = 1;

        public static Texture2D ColorToTex(Color col)
        {
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(1, 1, col);
            tex.Apply();
            return tex;
        }

        public void Init()
        {
            m_NodeBase = new GUIStyle();
            m_NodeBase.normal.background = ColorToTex(new Color(0.5f, 0.5f, 0.5f));
            m_NodeBase.normal.textColor = new Color(0.7f, 0.7f, 0.7f);

            m_NodeBox = new GUIStyle(m_NodeBase);
            m_NodeBox.margin = new RectOffset(8, 8, 5, 8);
            m_NodeBox.padding = new RectOffset(8, 8, 8, 8);

            m_NodeLabelBold = new GUIStyle(m_NodeBase);
            m_NodeLabelBold.fontStyle = FontStyle.Bold;
            m_NodeLabelBold.wordWrap = false;

        }



        [MenuItem("Window/Node Editor")]
        private static void CreateEditor()
        {
            m_Instance = GetWindow<NodeEditor>();
            m_Instance.minSize = new Vector2(800, 600);

            if (Instance.LoadedNodeCanvas == null)
                Instance.CreateNewNodeCanvas();

            m_Instance.Init();

            Instance.m_scale = 1;
        }

        public void CreateNewNodeCanvas()
        {
            m_LoadedNodeCanvas = CreateInstance<NodeCanvas>();
        }

        public void LoadNodeCanvas(string path)
        {
            if (System.String.IsNullOrEmpty(path))
                return;
            Object[] objects = AssetDatabase.LoadAllAssetsAtPath(path);
            if (objects.Length == 0)
                return;
            NodeCanvas newNodeCanvas = null;

            for (int cnt = 0; cnt < objects.Length; cnt++)
            { // We only have to search for the Node Canvas itself in the mess, because it still hold references to all of it's nodes and their connections
                object obj = objects[cnt];
                if (obj.GetType() == typeof(NodeCanvas))
                    newNodeCanvas = obj as NodeCanvas;
            }
            if (newNodeCanvas == null)
                return;
            m_LoadedNodeCanvas = newNodeCanvas;

            string[] folders = path.Split(new char[] { '/' }, System.StringSplitOptions.None);
            m_openedCanvas = folders[^1];
            m_openedCanvasPath = path;

            Repaint();
            AssetDatabase.Refresh();
        }


        public void SaveNodeCanvas(string path)
        {
            if (System.String.IsNullOrEmpty(path))
                return;
            string existingPath = AssetDatabase.GetAssetPath(m_LoadedNodeCanvas);
            if (!System.String.IsNullOrEmpty(existingPath))
            {
                if (existingPath != path)
                {
                    AssetDatabase.CopyAsset(existingPath, path);
                    LoadNodeCanvas(path);
                }
                return;
            }
            AssetDatabase.CreateAsset(m_LoadedNodeCanvas, path);
            for (int nodeCnt = 0; nodeCnt < m_LoadedNodeCanvas.NodeCount; nodeCnt++)
            { // Add every node and every of it's inputs/outputs into the file. 
              // Results in a big mess but there's no other way
                Node node = m_LoadedNodeCanvas.GetNode(nodeCnt);
                AssetDatabase.AddObjectToAsset(node, m_LoadedNodeCanvas);
                for (int inCnt = 0; inCnt < node.InConnectionsCount; inCnt++)
                    AssetDatabase.AddObjectToAsset(node.GetInConnection(inCnt), node);
                for (int outCnt = 0; outCnt < node.OutConnectionsCount; outCnt++)
                    AssetDatabase.AddObjectToAsset(node.GetOutConnection(outCnt), node);
            }

            string[] folders = path.Split(new char[] { '/' }, System.StringSplitOptions.None);
            m_openedCanvas = folders[^1];
            m_openedCanvasPath = path;

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Repaint();
        }


        private void OnGUI()
        {

            DrawGrid(20, .2f, Color.gray);
            DrawGrid(100, .2f, Color.gray);

            ProcessEvents(Event.current);

            if (m_LoadedNodeCanvas)
                for (int i = 0; i < m_LoadedNodeCanvas.NodeCount; i++)
                    m_LoadedNodeCanvas.GetNode(i).Draw();

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
                    if (e.button == 1) 
                        ProcessContextMenu(e.mousePosition);
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


        private void ProcessContextMenu(Vector2 mousePosition)
        {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Add Test node"), false, () => OnClickAddNode(mousePosition, "TestNode"));
            genericMenu.ShowAsContext();
        }

        public void OnClickAddNode(Vector2 position, string type)
        {
            switch (type)
            {
                case "TestNode":
                    TestNode.Create(new Rect(position, new Vector2(200, 50)));
                    break;
                default:
                    break;
            }
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