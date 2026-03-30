using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyConstantSpawn : MonoBehaviour, IDamaged
{
    public GameObject enemyPrefab;
    public int count = 5;
    public float radius = 10f;
    
    [Header("Spawn Scaling")]
    public float baseSpawnInterval = 15f; 
    public float startTime = 5f;
    public float minimumSpawnInterval = 3f; 

    [Header("Optimization")]
    public int maxEnemies = 30; // 👈 Stop spawning if there are too many on screen!
    private int _currentEnemyCount = 0;

    public event Action<float, Vector3> OnDamaged;
    private float _hp = 200f;
    private Coroutine _spawnCoroutine;

    [SerializeField] private Slider slider;

    private void Start()
    {
        _spawnCoroutine = StartCoroutine(SpawnLoop());
        slider.value = Mathf.InverseLerp(0f, 200f, _hp);
    }

    public void TakeDamage(float damageAmount)
    {
        _hp -= damageAmount;
        slider.value = Mathf.InverseLerp(0f, 200f, _hp);

        OnDamaged?.Invoke(damageAmount, transform.position);

        if (_hp <= 0)
        {
            if (_spawnCoroutine != null)
                StopCoroutine(_spawnCoroutine);
                
            GameManager.Instance.Spowner++;
            Debug.Log("Spawner Killed" + GameManager.Instance.Spowner);
            Destroy(gameObject);
        }
    }

    private IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(startTime);

        while (true)
        {
            // 👈 Only spawn if we haven't hit the cap!
            if (_currentEnemyCount < maxEnemies)
            {
                for (int i = 0; i < count; i++)
                {
                    SpawnOne();
                }
            }
            else
            {
                Debug.Log($"[Spawner] Cap hit ({_currentEnemyCount}/{maxEnemies}). Skipping wave.");
            }

            float currentInterval = GetScaledSpawnInterval();
            yield return new WaitForSeconds(currentInterval);
        }
    }

    private float GetScaledSpawnInterval()
    {
        int positivePuckCount = 0;

        // 1. Double check GameManager isn't null
        if (GameManager.Instance != null && GameManager.Instance.PuckDatas != null)
        {
            foreach (PuckData puck in GameManager.Instance.PuckDatas)
            {
                if (puck != null && !puck._isNegative) 
                {
                    positivePuckCount++;
                }
            }
        }

        // 2. Math calculation
        float reducedTime = baseSpawnInterval - (positivePuckCount * (baseSpawnInterval * 0.05f));
        
        // 3. THE SANITY CHECK: 
        // If reducedTime somehow drops to 0 or below, or is faster than minimum, 
        // we force it to stay at minimumSpawnInterval!
        float finalInterval = Mathf.Max(reducedTime, minimumSpawnInterval);

        // 4. Emergency Backup: If minimumSpawnInterval was set to 0 by mistake in the inspector,
        // this guarantees the game won't crash from 0-second infinite spawns.
        if (finalInterval <= 0.5f) 
        {
            finalInterval = 1.0f; 
        }

        Debug.Log($"[Spawner] Spawning wave. Waiting {finalInterval:F2}s for the next one.");

        return finalInterval;
    }

    public void SpawnOne()
    {
        Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * radius;
        Vector3 spawnPos = transform.position + new Vector3(randomCircle.x, randomCircle.y, 0f);

        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        
        // 👈 Increment count
        _currentEnemyCount++;

        EnemyController controller = enemy.GetComponent<EnemyController>();
        
        // 👈 Hook into the enemy's destruction to decrement the count!
        controller.OnDamaged += (damage, pos) => {
            // We use standard checking in IDamaged or state checks, 
            // but the cleanest way is a callback right before Destroy() in EnemyController.
        };

        ApplyRandomType(controller);
        controller.InitEnemy();
        controller.CurrentState = EnemyState.Chase;
    }

    // Call this from the EnemyController when it dies!
    public void DecrementEnemyCount()
    {
        _currentEnemyCount = Mathf.Max(0, _currentEnemyCount - 1);
    }

    void ApplyRandomType(EnemyController controller)
    {
        int random = UnityEngine.Random.Range(0, 15);

        if (random >= 11) 
        {
            controller.Type = EnemyType.Normal;
            controller.AttackType = EnemyAttackType.Range;
        }
        else if (random >= 7) 
        {
            controller.Type = EnemyType.Normal;
            controller.AttackType = EnemyAttackType.Range; 
        }
        else if (random >= 3) 
        {
            controller.Type = EnemyType.Epic;
            controller.AttackType = EnemyAttackType.Malee;
        }
        else 
        {
            controller.Type = EnemyType.Normal;
            controller.AttackType = EnemyAttackType.Malee;
        }
    }
}