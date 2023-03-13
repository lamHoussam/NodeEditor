using UnityEngine;

[CreateAssetMenu(fileName = "Settings", menuName = "Settings/TestSettings", order = 1)]
public class TestSettings : ScriptableObject
{
    [SerializeField] private int m_value;
    public int Value => m_value;

    [SerializeField] private string m_title;
    public string Title => m_title;
}
