using System;

using UnityEditor;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;

namespace NodeEditorFramework
{
    public class NodeEditor : EditorWindow
    {
        private static NodeEditor m_Instance;
        public static NodeEditor Instance => m_Instance;

        private NodeCanvas m_LoadedNodeCanvas;
        public NodeCanvas LoadedNodeCanvas => m_LoadedNodeCanvas;

        public const string m_editorPath = "Assets/NodeCanvases/";
        private string m_openedCanvas = "New Canvas";
        private string m_openedCanvasPath;

        public GUIStyle m_NodeBase;
        public GUIStyle m_SelectedNodeBase;
        public GUIStyle m_NodeBox;
        public GUIStyle m_NodeLabelBold;
        public GUIStyle m_EvaluatedNodeResult;
        public GUIStyle m_KnobStyle;

        public static GUIStyle m_NodeButton;

        private float m_sideWindowWidth = 400;
        public Rect SideWindowRect => new Rect(position.width - m_sideWindowWidth, 0, m_sideWindowWidth, position.height);
        public Rect ParameterWindowRect => new Rect(0, 0, m_sideWindowWidth, position.height);

        //private NodeConnectionPoint m_SelectedInConnectionPoint, m_SelectedOutConnectionPoint;
        private Node m_SelectedNodeForConnection;

        private NodeConnection m_SelectedNodeConnection; 

        private Vector2 m_offset;
        private Vector2 m_drag;

        private float m_scale = 1;

        private bool m_isInitialised = false;

        public static Texture2D ColorToTex(Color col)
        {
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(1, 1, col);
            tex.Apply();
            return tex;
        }

        public void Init()
        {
            m_NodeBase = new GUIStyle(GUI.skin.box);
            m_NodeBase.normal.background = ColorToTex(new Color(0.5f, 0.5f, 0.5f));
            m_NodeBase.normal.textColor = new Color(0.7f, 0.7f, 0.7f);
            m_NodeBase.alignment = TextAnchor.MiddleCenter;


            m_SelectedNodeBase = new GUIStyle(GUI.skin.box);
            m_SelectedNodeBase.normal.background = ColorToTex(new Color(1, 1, 1f));
            m_SelectedNodeBase.normal.textColor = new Color(1, 1, 1);
            m_SelectedNodeBase.alignment = TextAnchor.MiddleCenter;

            m_EvaluatedNodeResult = new GUIStyle(GUI.skin.box);
            m_EvaluatedNodeResult.normal.background = ColorToTex(new Color(0, .5f, .8f));
            m_EvaluatedNodeResult.normal.textColor = new Color(.8f, .7f, 1);
            m_EvaluatedNodeResult.alignment = TextAnchor.MiddleCenter;

            m_NodeBox = new GUIStyle(m_NodeBase);
            m_NodeBox.margin = new RectOffset(8, 8, 5, 8);
            m_NodeBox.padding = new RectOffset(8, 8, 8, 8);

            m_NodeLabelBold = new GUIStyle(m_NodeBase);
            m_NodeLabelBold.fontStyle = FontStyle.Bold;
            m_NodeLabelBold.wordWrap = false;

            m_KnobStyle = new GUIStyle();
            m_KnobStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
            m_KnobStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
            m_KnobStyle.border = new RectOffset(4, 4, 12, 12);


            m_NodeButton = new GUIStyle();
            m_NodeButton.normal.textColor = new Color(0.3f, 0.3f, 0.3f);

            m_isInitialised = true;

            
            if(m_LoadedNodeCanvas.Entry == null)
                OnClickAddNode(new Vector2(Instance.position.width / 2, Instance.position.height / 2), "EntryNode");
        }




        [MenuItem("Window/Node Editor")]
        private static void CreateEditor()
        {
            m_Instance = GetWindow<NodeEditor>();
            m_Instance.minSize = new Vector2(800, 600);

            if (Instance.LoadedNodeCanvas == null)
            {
                Instance.CreateNewNodeCanvas();
            }

            //m_Instance.Init();
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
            UnityEngine.Object[] objects = AssetDatabase.LoadAllAssetsAtPath(path);
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

                //AssetDatabase.AddObjectToAsset(node.InConnection, node);
                //AssetDatabase.AddObjectToAsset(node.OutConnection, node);

            }

            string[] folders = path.Split(new char[] { '/' }, System.StringSplitOptions.None);
            m_openedCanvas = folders[^1];
            m_openedCanvasPath = path;

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Repaint();
        }

        public void DrawSideWindow()
        {
            GUILayout.Label(new GUIContent("Node Editor (" + m_openedCanvas + ")", "The currently opened canvas in the Node Editor"), m_NodeLabelBold);
            //GUILayout.Label(new GUIContent("Do note that changes will be saved automatically!", "All changes are automatically saved to the currently opened canvas (see above) if it's present in the Project view."), m_NodeBase);
            if (GUILayout.Button(new GUIContent("Save Canvas", "Saves the canvas as a new Canvas Asset File in the Assets Folder")))
            {
                SaveNodeCanvas(EditorUtility.SaveFilePanelInProject("Save Node Canvas", "Node Canvas", "asset", "Saving to a file is only needed once.", m_editorPath + "Saves/"));
            }
            if (GUILayout.Button(new GUIContent("Load Canvas", "Loads the canvas from a Canvas Asset File in the Assets Folder")))
            {
                string path = EditorUtility.OpenFilePanel("Load Node Canvas", m_editorPath + "Saves/", "asset");
                if (!path.Contains(Application.dataPath))
                {
                    if (path != String.Empty)
                        ShowNotification(new GUIContent("You should select an asset inside your project folder!"));
                    return;
                }
                path = path.Replace(Application.dataPath, "Assets");
                LoadNodeCanvas(path);
            }
            if (GUILayout.Button(new GUIContent("New Canvas", "Creates a new Canvas (remember to save the previous one to a referenced Canvas Asset File at least once before! Else it'll be lost!)")))
            {
                CreateNewNodeCanvas();
            }

            if (GUILayout.Button(new GUIContent("Evaluate")))
            {
                //CreateNewNodeCanvas();
                Node node = LoadedNodeCanvas.Evaluate();
                if (node != null)
                {
                    //Debug.Log("Found node : " + node.ToString());
                    try
                    {
                        StateNode sNode = (StateNode)node;
                        if (sNode)
                        {
                            Debug.Log(sNode.Settings.Value);
                        }
                    }
                    catch (Exception)
                    {

                    }
                    Debug.Log(node.GetType());
                    node.SetEvaluationResult();
                }
                //GUILayout.TextField(node.ToString());
            }


            if (m_SelectedNodeConnection)
                m_SelectedNodeConnection.DisplayConditions();
        }

        public void DrawParametersWindow()
        {
            if (!m_LoadedNodeCanvas)
                return;

            Rect rect = new Rect(Vector2.up * 100, new Vector2(m_sideWindowWidth, 100));
            m_LoadedNodeCanvas.DisplayParameters(rect);
        }

        public void OnClickNodeConnection(NodeConnection connection)
        {
            m_SelectedNodeConnection = connection;
        }

        public void OnClickAddCondition(NodeConnection connection)
        {
            if(m_LoadedNodeCanvas.ParametersCount == 0 || connection == null) 
                return;

            NodeEditorParameter param = m_LoadedNodeCanvas.GetFirst();
            ConnectionCondition condition = new ConnectionCondition(param, default);

            connection.AddCondition(condition);
        }


        private void OnGUI()
        {
            if (!m_isInitialised)
                Init();

            DrawGrid(20, .2f, Color.gray);
            DrawGrid(100, .2f, Color.gray);


            if (m_LoadedNodeCanvas)
            {
                m_LoadedNodeCanvas.ProcessNodeEvents(Event.current);
                //m_LoadedNodeCanvas.Proces
            }

            ProcessEvents(Event.current);

            if (m_LoadedNodeCanvas)
            {
                for (int i = 0; i < m_LoadedNodeCanvas.NodeCount; i++)
                    m_LoadedNodeCanvas.GetNode(i).Draw();

                for(int i = 0; i < m_LoadedNodeCanvas.NodeConnectionsCount; i++)
                    m_LoadedNodeCanvas.GetNodeConnection(i).Draw();
            }

            m_sideWindowWidth = Math.Min(600, Math.Max(200, (int)(position.width / 5)));
            GUILayout.BeginArea(SideWindowRect, m_NodeBox);
            DrawSideWindow();

            GUILayout.EndArea();

            GUILayout.BeginArea(ParameterWindowRect, m_NodeBox);
            DrawParametersWindow();
            GUILayout.EndArea();

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

            if (m_LoadedNodeCanvas)
                for (int i = 0; i < m_LoadedNodeCanvas.NodeCount; i++)
                    m_LoadedNodeCanvas.GetNode(i).OnDrag(delta);

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
            genericMenu.AddItem(new GUIContent("Add State Node"), false, () => OnClickAddNode(mousePosition, "StateNode"));
            genericMenu.AddItem(new GUIContent("Add new Parameter"), false, () => OnClickAddParameter());
            genericMenu.ShowAsContext();
        }

        public void OnClickAddParameter()
        {
            if (!m_LoadedNodeCanvas)
                return;
            m_LoadedNodeCanvas.AddParameter(new NodeEditorParameter(ParameterType.Bool, (object)false, "Parameter"));
        }

        public void OnClickRemoveNode(Node node)
        {
            m_LoadedNodeCanvas.RemoveNode(node);
            node.OnRemove();
        }

        public void OnClickRemoveNodeConnection(NodeConnection connection)
        {
            m_LoadedNodeCanvas.RemoveNodeConnection(connection);
            connection.OnRemove();
        }

        public void OnClickRemoveCondition(ConnectionCondition condition) {
            if (!m_SelectedNodeConnection)
                return;

            m_SelectedNodeConnection.RemoveCondition(condition);
        }

        public void OnClickAddNode(Vector2 position, string type)
        {
            switch (type)
            {
                case "TestNode":
                    TestNode.Create(new Rect(position, new Vector2(200, 50)));
                    break;

                case "StateNode":
                    StateNode.Create(new Rect(position, new Vector2(200, 50)));
                    break;
                case "EntryNode":
                    EntryNode.Create(new Rect(position, new Vector2(200, 50)));
                    break;
                default:
                    break;
            }
        }

        public void OnClickFirstNodeForConnection(Node fromNode)
        {
            m_SelectedNodeForConnection = fromNode;
        }

        public void CreateConnection(Node toNode)
        {
            //if (m_LoadedLogic)
            //    m_LoadedLogic.AddConnection(new Connection(m_SelectedInPoint, m_SelectedOutPoint));
            if (toNode.GetType() == typeof(EntryNode) || toNode == m_SelectedNodeForConnection)
                return;

            if (m_LoadedNodeCanvas && m_SelectedNodeForConnection)
            {
                NodeConnection connection = CreateInstance<NodeConnection>();
                connection.SetNodeConnectionPoints(m_SelectedNodeForConnection, toNode);

                m_SelectedNodeForConnection.AddConnection(connection);
                m_LoadedNodeCanvas.AddNodeConnection(connection);

                ClearConnectionSelection();
            }
        }

        public void ClearConnectionSelection()
        {
            m_SelectedNodeForConnection = null;
            //m_SelectedOutConnectionPoint = null;

            m_SelectedNodeConnection = null;
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