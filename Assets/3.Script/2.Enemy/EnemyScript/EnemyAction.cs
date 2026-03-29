using UnityEngine;


public interface EnemyBehavior
{
    void OnIdle();
    void OnRecognize();
    void OnChase();
    void OnAttack();
    void OnDead();
}

public class EnemyAction : MonoBehaviour
{
    private EnemyAttackType currentType;

    public void SetCurrentAttackType(EnemyAttackType type)
    {
        currentType = type;
    }


    void Start()
    {
        switch (currentType)
        {
            case EnemyAttackType.Range:

                break;
            case EnemyAttackType.Malee:

                break;

            case EnemyAttackType.Explode:

                break;
        }
    }
}
