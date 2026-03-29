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
            Vector3 spawnPos = transform.position + (Random.insideUnitSphere * radius);
            spawnPos.z = 0; // Keep it 2D if necessary

            GameObject go = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            EnemyController controller = go.GetComponent<EnemyController>();

            if (mode == SpawnMode.Aggressive)
            {
                controller.CurrentState = EnemyState.Chase;
            }
            else
            {
                controller.CurrentState = EnemyState.Idle;
                // Assign this enemy to the group
                controller.AssignToGroup(group); 
            }
        }
    }
}
