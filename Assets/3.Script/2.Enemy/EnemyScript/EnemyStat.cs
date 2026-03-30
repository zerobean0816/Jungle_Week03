using UnityEngine;

public class EnemyStat : MonoBehaviour
{
    public Stat stat;

    private EnemyController _controller;

    void Start()
    {
        _controller = GetComponent<EnemyController>();

        switch (_controller.Type)
        {
            case EnemyType.Normal:
            SetStatTypeNormal();
            break;

            case EnemyType.Epic:
            SetStatTypeEpic();
            break;

            case EnemyType.Stocker:
            SetStatTypeStocker();
            break;
        }
    }

    void SetStatTypeNormal()
    {
        stat.baseHealth = 50f;
        stat.baseMoveSpeed = 15f;
        stat.basePlayerScale = 1.8f;
        stat.baseDamage = 8f;
        stat.baseAttackSpeed = 20f;
        stat.basePlayerScale = 1.8f;
        stat.ResetByChange();
    }

    void SetStatTypeEpic()
    {
        stat.baseHealth = 200f;
        stat.baseMoveSpeed = 10f;
        stat.basePlayerScale = 3f;
        stat.baseDamage = 12f;
        stat.baseAttackSpeed = 15f;
        stat.basePlayerScale = 2.2f;
        stat.ResetByChange();
    }

    void SetStatTypeStocker()
    {
        stat.baseHealth = 30f;
        stat.baseMoveSpeed = 20f;
        stat.basePlayerScale = 1.4f;
        stat.baseDamage = 4f;
        stat.baseAttackSpeed = 30f;
        stat.basePlayerScale = 1.5f;
        stat.ResetByChange();
    }
}
