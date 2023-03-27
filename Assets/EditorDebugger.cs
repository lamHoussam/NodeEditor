using NodeEditorFramework;
using TMPro;
using UnityEngine;

public class EditorDebugger : MonoBehaviour
{
    [SerializeField] private NodeCanvas m_NodeCanvas;
    public NodeCanvas Cnv => m_NodeCanvas;

    private void Awake()
    {
        m_NodeCanvas.LoadCanvasParameterState();
    }

    public void Display()
    {
        for(int i = 0; i < m_NodeCanvas.ParametersCount; i++)
        {
            NodeEditorParameter param = m_NodeCanvas.GetParameter(i);
            transform.GetChild(i).GetComponent<TextMeshProUGUI>().text = "Name : " + param.Name + ", Value : " + param.Value.BoolValue;
        }
    }

    private void Update()
    {
        Display();
    }
}
