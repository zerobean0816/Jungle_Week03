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
        stat.baseHealth = 45f;
        stat.baseMoveSpeed = 18f;
        stat.basePlayerScale = 2f;
        stat.baseDamage = 8f;
        stat.baseAttackSpeed = 20f;
;
        stat.ResetByChange();
    }

    void SetStatTypeEpic()
    {
        stat.baseHealth = 190f;
        stat.baseMoveSpeed = 11f;
        stat.basePlayerScale = 4.2f;
        stat.baseDamage = 13f;
        stat.baseAttackSpeed = 15f;

        stat.ResetByChange();
    }

    void SetStatTypeStocker()
    {
        stat.baseHealth = 25f;
        stat.baseMoveSpeed = 28f;
        stat.basePlayerScale = 1.4f;
        stat.baseDamage = 5f;
        stat.baseAttackSpeed = 25f;

        stat.ResetByChange();
    }
}
