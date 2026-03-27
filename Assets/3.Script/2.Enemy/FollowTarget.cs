using UnityEngine;
using UnityEngine.AI;

public class FollowTarget : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    public Transform targetTransform;
    public Vector3 targetPosition;
    public float stoppingDistance = 0.5f;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        // navMeshAgent.updateRotation = false;
        
        if (navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent component not found on this GameObject!");
        }
    }

    void Update()
    {
        if (navMeshAgent == null || !navMeshAgent.isOnNavMesh)
            return;

        // If there's a target transform, follow it
        if (targetTransform != null)
        {
            navMeshAgent.SetDestination(targetTransform.position);
        }
        // Otherwise, follow the set target position
        else if (targetPosition != Vector3.zero)
        {
            navMeshAgent.SetDestination(targetPosition);
        }

        transform.rotation = Quaternion.Euler(0,0,0);
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
}

