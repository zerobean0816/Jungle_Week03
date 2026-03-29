using Unity.VisualScripting;
using UnityEngine;


[RequireComponent(typeof(EnemyStat))]
[RequireComponent(typeof(FollowTarget))]
[RequireComponent(typeof(EnemyAction))]
public class EnemyController : MonoBehaviour, IDamaged
{
    [Header("Enemy Type / State Reference")]
    public EnemyType Type;
    public EnemyState CurrentState;
    public EnemyAttackType AttackType;

    
    // Enemy Self Reference
    private EnemyStat _enemyStat;
    private FollowTarget _targetTracker;
    private EnemyAction _enemyAction;

    // Player Reference
    private GameObject _player;


    // Data values;
    [SerializeField] private float _distBWPlayer;
    [SerializeField] private bool _isChasing;
    [SerializeField] private bool _isAttacking;
    [SerializeField] private bool _isDamaged;

    private EnemyGroup _myGroup; // Reference to its group
    [SerializeField] private LayerMask _playerLayer;

    public void AssignToGroup(EnemyGroup group) => _myGroup = group;

    
    void Start()
    {
        _enemyStat = GetComponent<EnemyStat>();
        _enemyAction = GetComponent<EnemyAction>();
        _targetTracker = GetComponent<FollowTarget>();

        _enemyAction.SetCurrentAttackType(AttackType);

        _player = GameManager.Instance.Player;

        _targetTracker.enabled = false;
        _targetTracker.SetTargetTransform(_player.transform);
        _targetTracker.SetFollowerSpeed(_enemyStat.stat.moveSpeed);
    }

    void IDamaged.TakeDamage(float damageAmount)
    {
        _enemyStat.stat.currentHealth -= damageAmount;

        if (_enemyStat.stat.currentHealth <= 0)
        {
            CurrentState = EnemyState.Dead;
        }
    } 


    void Update()
    {
        FindDistanceBWPlayer();

        ActByState();

        transform.rotation = Quaternion.identity;
    }

    void ActByState()
    {
        switch (CurrentState)
        {
            case EnemyState.Idle:
                OnIdle();
                break;
            case EnemyState.Recognized:
                OnRecognize();
                break;
            case EnemyState.Chase:
                OnChase();
                break;
            case EnemyState.Attack:
                OnAttack();
                break;
            case EnemyState.Dead:
                OnDead();
                break;
        }
    }

    void OnIdle()
    {
        // 1. Check if damaged (You already have this)
        if (_enemyStat.stat.currentHealth < _enemyStat.stat.maxHealth)
        {
            AlertMySystem();
        }

        // 2. Check Raycast for Player
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 10f, _playerLayer);
        if (hit.collider != null)
        {
            AlertMySystem();
        }
    }

    void OnRecognize()
    {
        
    }

    void OnChase()
    {
        if (!_isChasing)
        {
            _targetTracker.enabled = true;
            _isChasing = true;
        }
    }

    void OnAttack()
    {
        
    }

    void OnDead()
    {
        _targetTracker.enabled = false;
        Destroy(gameObject);
    }

    void FindDistanceBWPlayer()
    {
        _distBWPlayer = (_player.transform.position - transform.position).magnitude;
    }


    void AlertMySystem()
    {
        CurrentState = EnemyState.Chase;
        
        // If I belong to a group, tell everyone else!
        if (_myGroup != null)
        {
            _myGroup.AlertGroup();
        }
    }
}
