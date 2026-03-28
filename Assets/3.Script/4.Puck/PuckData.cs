using UnityEngine;

[CreateAssetMenu(fileName = "New Puck", menuName = "Puck System/Puck Data")]
public class PuckData : ScriptableObject
{
    public string puckName;
    [TextArea]
    public string description;
    public Sprite icon;

    [Range(1, 10)]
    public int size = 1; // ← 추가: 차지하는 슬롯 수

    public StatModifier[] modifiers;
}
