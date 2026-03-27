using UnityEngine;

[CreateAssetMenu(fileName = "New Puck", menuName = "Puck System/Puck Data")]
public class PuckData : ScriptableObject
{
    public string puckName; // 퍽 이름 
    [TextArea]
    public string description; // 퍽 설명 텍스트
    public Sprite icon; // 퍽 아이콘 (선택적)

    public StatModifier[] modifiers; // 퍽이 제공하는 모디파이어 배열
}
