using UnityEngine;
using UnityEngine.AI;

public class FollowTarget : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    public Transform targetTransform;
    public Vector3 targetPosition;
    public float stoppingDistance = 2f;

    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        
        if (navMeshAgent == null)
        {
            Debug.LogError("[FollowTarget] : NavMeshAgent component not found on this GameObject!");
        }
    }

    void Update()
    {
        if (navMeshAgent == null || !navMeshAgent.isOnNavMesh)
        return;

        if (targetTransform != null)
            navMeshAgent.SetDestination(targetTransform.position);
        else if (targetPosition != Vector3.zero)
            navMeshAgent.SetDestination(targetPosition);

        // ✅ 이동 방향으로 회전
        if (navMeshAgent.velocity.sqrMagnitude > 0.01f)
        {
            Vector2 dir = new Vector2(navMeshAgent.velocity.x, navMeshAgent.velocity.y);
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    
    public void SetStoppingDistance(float distance)
    {
        navMeshAgent.stoppingDistance = distance;
        stoppingDistance = distance;
    }

    // Public method to set a new target position
    public void SetTarget(Vector3 newTarget)
    {
        targetPosition = newTarget;
        targetTransform = null;
        if (navMeshAgent != null && navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.SetDestination(newTarget);
        }
    }

    // Public method to set a target transform to follow
    public void SetTargetTransform(Transform target)
    {
        targetTransform = target;
        targetPosition = Vector3.zero;
    }

    // Check if the agent has reached its destination
    public bool HasReachedDestination()
    {
        if (navMeshAgent == null || !navMeshAgent.hasPath)
            return false;

        return !navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f;
    }

    public void SetFollowerSpeed(float moveSpeed)
    {
        navMeshAgent.speed = moveSpeed;
    }
}

