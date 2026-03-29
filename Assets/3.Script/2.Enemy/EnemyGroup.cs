using UnityEngine;
using System.Collections.Generic;

public class EnemyGroup : MonoBehaviour
{
    public List<EnemyController> members = new List<EnemyController>();

    public void AlertGroup()
    {
        foreach (var enemy in members)
        {
            if (enemy != null && enemy.CurrentState == EnemyState.Idle)
            {
                enemy.CurrentState = EnemyState.Chase;
            }
        }
    }
}
