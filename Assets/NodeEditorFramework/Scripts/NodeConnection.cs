using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NodeEditorFramework
{
    [System.Serializable]
    public class NodeConnection : ScriptableObject
    {
        [SerializeField] private Node m_From, m_To;
        public Node From => m_From;
        public Node To => m_To;

        [SerializeField] private List<ConnectionCondition> m_Conditions;
        public int ConditionsCount => m_Conditions == null ? 0 : m_Conditions.Count;
        public ConnectionCondition GetCondition(int i) => m_Conditions[i];



        public void SetNodeConnectionPoints(Node from, Node to)
        {
            m_From = from;
            m_To = to;

            if (!System.String.IsNullOrEmpty(AssetDatabase.GetAssetPath(NodeEditor.Instance.LoadedNodeCanvas)))
            {
                AssetDatabase.AddObjectToAsset(this, NodeEditor.Instance.LoadedNodeCanvas);

                if (m_Conditions != null)
                {
                    for (int i = 0; i < m_Conditions.Count; i++)
                        AssetDatabase.AddObjectToAsset(m_Conditions[i], this);
                }

                AssetDatabase.Refresh();
            }

        }


        public void Draw()
        {
            if (m_From == null || m_To == null)
                return;

            Handles.DrawBezier(
                m_From.Center,
                m_To.Center,
                m_From.Center + Vector2.left * 50f,
                m_To.Center - Vector2.left * 50f,
                EvaluateConditions() ? new Color(0, .5f, .8f) : Color.white,
                null,
                2f
            );

            if (Handles.Button(m_To.Center, Quaternion.identity, 4, 8, Handles.CircleHandleCap))
                Debug.Log("Houssam");

            if (Handles.Button((m_From.Center + m_To.Center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
                NodeEditor.Instance.OnClickNodeConnection(this);
        }


        public void DisplayConditions()
        {
            if (GUILayout.Button("Remove Node Connection"))
                NodeEditor.Instance.OnClickRemoveNodeConnection(this);

            GUILayout.Label(new GUIContent("Condition"), NodeEditor.Instance.m_NodeLabelBold);
            if (GUILayout.Button("New Condition"))
                NodeEditor.Instance.OnClickAddCondition(this);


            if (m_Conditions != null)
                for (int i = 0; i < m_Conditions.Count; i++)
                    m_Conditions[i].Display();
        }

        public void AddCondition(ConnectionCondition cndition)
        {
            m_Conditions ??= new List<ConnectionCondition>();
            m_Conditions.Add(cndition);
        }

        public void RemoveCondition(ConnectionCondition cndition)
        {
            if (m_Conditions == null)
                return;

            m_Conditions.Remove(cndition);
        }

        public bool EvaluateConditions()
        {
            if (m_Conditions == null)
                return true;

            for(int i = 0; i < m_Conditions.Count; i++)
                if (!m_Conditions[i].Evaluate()) 
                    return false;

            return true;
        }

        public void OnRemove()
        {
            m_Conditions?.Clear();
            m_From.RemoveConnection(this);
        }       

    }
}