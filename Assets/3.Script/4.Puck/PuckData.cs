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

    public bool _isNegative;

    public StatModifier[] modifiers;

    public enum PuckRarity { Normal, Rare,Epic}

    // Add inside PuckData ScriptableObject
    public PuckRarity rarity;

    public int PointCost
    {
        get
        {
            switch (rarity)
            {
                case PuckRarity.Epic: return 20;
                case PuckRarity.Rare: return 5; // Added a cost for Rare!
                default: return 2; // Normal
            }
        }
    }
}
