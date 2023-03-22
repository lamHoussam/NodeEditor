using System.Collections.Generic;
using System.Data.Common;
using UnityEditor;
using UnityEngine;

namespace NodeEditorFramework
{
    [System.Serializable]
    public class NodeConnection : ScriptableObject
    {
        [SerializeField] private Node m_From, m_To;
        [SerializeField] private List<ConnectionCondition> m_Conditions;
        private bool m_isSelected;
        private Vector2 m_scrollPosition;

        #region Properties API
        public Node From => m_From;
        public Node To => m_To;
        public int ConditionsCount => m_Conditions == null ? 0 : m_Conditions.Count;
        #endregion

        #region API
        public ConnectionCondition GetCondition(int i) => m_Conditions[i];

        /// <summary>
        /// Evaluate all conditions in condition's list
        /// </summary>
        /// <returns>True if all conditions evaluate to true False else</returns>
        public bool EvaluateConditions()
        {
            if (m_Conditions == null)
                return true;

            for(int i = 0; i < m_Conditions.Count; i++)
                if (!m_Conditions[i].Evaluate()) 
                    return false;

            return true;
        }
        #endregion

#if UNITY_EDITOR

        #region Editor API

        /// <summary>
        /// Set nodes connected to this
        /// </summary>
        /// <param name="from">Starting node</param>
        /// <param name="to">Target node</param>
        public void SetNodeConnectionPoints(Node from, Node to)
        {
            m_From = from;
            m_To = to;

        }

        public void Select()
        {
            m_isSelected = true;
        }

        public void Deselect()
        {
            m_isSelected = false;
        }

        /// <summary>
        /// Draw node connection to Node editor
        /// </summary>
        public void Draw()
        {
            if (m_From == null || m_To == null)
                return;

            Color cnxColor;

            if (m_isSelected)
                cnxColor = new Color(.1f, .5f, .8f);
            else
            {
                Color col = NodeEditor.Instance.m_trueColor;
                Color trueCol = new Color(col.r, col.g, col.b);
                col = NodeEditor.Instance.m_falseColor;
                Color falseCol = new Color(col.r, col.g, col.b);

                cnxColor = EvaluateConditions() ? trueCol : falseCol;
            }

            Handles.DrawBezier(
                m_From.Center,
                m_To.Center,
                m_From.Center - Vector2.left * 50f,
                m_To.Center + Vector2.left * 50f,
                cnxColor,
                null,
                2f
            );

            if (Handles.Button((Vector3)m_To.Center - Vector3.forward * 5, Quaternion.LookRotation(Vector3.right), 12, 8, Handles.ConeHandleCap))
                Debug.Log("Houssam");

            if (Handles.Button((m_From.Center + m_To.Center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
                NodeEditor.Instance.OnClickNodeConnection(this);
        }


        /// <summary>
        /// Display conditions to Node editor
        /// </summary>
        public void DisplayConditions()
        {
            if (GUILayout.Button("Remove Node Connection"))
                NodeEditor.Instance.OnClickRemoveNodeConnection(this);

            GUILayout.Label(new GUIContent("Conditions"), NodeEditor.Instance.m_NodeLabelBold);

            GUILayout.BeginVertical();
            m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition, false, true);
            if (GUILayout.Button("New Condition"))
                NodeEditor.Instance.OnClickAddCondition(this);


            if (m_Conditions != null)
                for (int i = 0; i < m_Conditions.Count; i++)
                    m_Conditions[i].Display();

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Add condition 
        /// </summary>
        /// <param name="cndition">Condition to add</param>
        public void AddCondition(ConnectionCondition cndition)
        {
            m_Conditions ??= new List<ConnectionCondition>();
            m_Conditions.Add(cndition);


            if (!System.String.IsNullOrEmpty(AssetDatabase.GetAssetPath(this)))
            {
                AssetDatabase.AddObjectToAsset(cndition, this);
                AssetDatabase.Refresh();
            }

        }

        /// <summary>
        /// Remove confition
        /// </summary>
        /// <param name="cndition">Condition to remove</param>
        public void RemoveCondition(ConnectionCondition cndition)
        {
            if (m_Conditions == null)
                return;

            m_Conditions.Remove(cndition);
        }

        /// <summary>
        /// Called on remove node connection
        /// </summary>
        public void OnRemove()
        {
            m_Conditions?.Clear();
            m_From.RemoveConnection(this);
            DestroyImmediate(this);
        }

        #endregion
#endif

    }
}