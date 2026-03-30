using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public enum SpawnMode { Aggressive, NeutralPack }
    
    public SpawnMode mode;
    public GameObject enemyPrefab;
    public int count = 5;
    public float radius = 10f;

    void Start()
    {
        // Create a group container if we are in NeutralPack mode
        EnemyGroup group = null;
        if (mode == SpawnMode.NeutralPack)
        {
            GameObject groupObj = new GameObject("EnemyGroup_Instance");
            group = groupObj.AddComponent<EnemyGroup>();
        }

        for (int i = 0; i < count; i++)
        {
            // Calculate 2D position within a circle instead of a sphere
            Vector2 randomCircle = Random.insideUnitCircle * radius;
            Vector3 spawnPos = transform.position + new Vector3(randomCircle.x, randomCircle.y, 0f);

            GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

            // CACHE COMPONENTS: This stops Unity from lagging when spawning many enemies
            EnemyController controller = enemy.GetComponent<EnemyController>();
            EnemyStat enemyStat = enemy.GetComponent<EnemyStat>();

            // 1. Assign random types FIRST so it's ready before state triggers
            ApplyRandomType(controller);


            // 3. Apply behaviors based on spawn mode
            if (mode == SpawnMode.Aggressive)
            {
                controller.CurrentState = EnemyState.Chase;
            }
            else
            {
                controller.CurrentState = EnemyState.Idle;
                
                // Assign this enemy to the group
                if (group != null)
                {
                    controller.AssignToGroup(group); 
                    group.RegisterMember(controller);
                }
            }
        }
    }

    void ApplyRandomType(EnemyController controller)
    {
        int random = Random.Range(0, 10); // Generates 0 to 9

        // Always check the highest/rarest values first!
        if (random >= 9) // 10% chance (Rolls a 9)
        {
            controller.Type = EnemyType.Epic;
            controller.AttackType = EnemyAttackType.Malee;
        }
        else if (random >= 6) // 30% chance (Rolls a 6, 7, 8)
        {
            controller.Type = EnemyType.Normal;
            controller.AttackType = EnemyAttackType.Range;
        }
        else // 60% chance (Rolls 0, 1, 2, 3, 4, 5)
        {
            controller.Type = EnemyType.Normal;
            controller.AttackType = EnemyAttackType.Malee;
        }
    }
}
