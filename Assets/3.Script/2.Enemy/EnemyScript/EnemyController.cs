using UnityEngine;
using System;

[RequireComponent(typeof(EnemyStat))]
[RequireComponent(typeof(FollowTarget))]
[RequireComponent(typeof(EnemyAction))]
[RequireComponent(typeof(EnemySprite))]

public class EnemyController : MonoBehaviour, IDamaged
{
    [Header("Enemy Type / State Reference")]
    public EnemyType Type;
    public EnemyState CurrentState;
    public EnemyAttackType AttackType;

    public event Action<float, Vector3> OnDamaged;
    public static event Action<float, Vector3> OnAnyCharacterDamaged;

    [Header("Attack Setting")]
    public float AttackRangeOnMalee;
    [SerializeField] GameObject bullet;

    // Enemy Self Reference
    private EnemyStat _enemyStat;
    private FollowTarget _targetTracker;
    private EnemyAction _enemyAction;
    private EnemySprite _enemySprite;

    // Player Reference
    private GameObject _player;


    Vector3 _textPosition;
    // Data values;
    [SerializeField] private float _distBWPlayer;
    [SerializeField] private bool _isChasing;
    [SerializeField] private bool _isAttacking;
    [SerializeField] private bool _isDamaged;
    [SerializeField] private float _recognizeTimer = 0f;
    [SerializeField] private float _recognizeDuration = 3f;
    [SerializeField] private float _detectionRadius = 10f;

    private EnemyGroup _myGroup; // Reference to its group
    [SerializeField] private LayerMask _playerLayer;

    public void AssignToGroup(EnemyGroup group) => _myGroup = group;

    
    void Start()
    {
        InitEnemy();
    }

    public void InitEnemy()
    {
        _enemyStat = GetComponent<EnemyStat>();
        _enemyAction = GetComponent<EnemyAction>();
        _targetTracker = GetComponent<FollowTarget>();
        _enemySprite = GetComponent<EnemySprite>();

        _enemyAction.CallStart();
        _enemyAction.SetCurrentAttackType(AttackType);
        
        // Fetch the player actively right here
        _player = GameObject.FindGameObjectWithTag("Player");

        if (_player != null)
        {
            // Now that we have a player, we can safely grab its scale or stats!
            float scaler = _enemyStat.stat.playerScale; 
            transform.localScale = new Vector3(scaler, scaler, 1f);
            
            _enemyAction.SetupAttackType(scaler); 
            _targetTracker.SetTargetTransform(_player.transform);
            _textPosition = transform.position + new Vector3(0, scaler + 1, 0);
        }
        else
        {
            Debug.LogWarning($"[EnemyController] No Player found on scene restart for {gameObject.name}");
        }

        _enemySprite.CallStart();
        _enemyAction.SetupIdle();

        _targetTracker.enabled = false;
        _targetTracker.SetFollowerSpeed(_enemyStat.stat.moveSpeed);
    }

    void Update()
    {
        FindDistanceBWPlayer();

        ActByState();
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
        // Player is close → recognize first, then chase after timer
        Collider[] hits = Physics.OverlapSphere(transform.position, _detectionRadius, _playerLayer);
        if (hits.Length > 0)
        {
            CurrentState = EnemyState.Recognized;
            return;
        }

        _enemyAction.TickIdle();


        _enemySprite.SetIdelSprite();
        transform.rotation = Quaternion.identity;
    }

    void OnRecognize()
    {
        _targetTracker.enabled = false;
        RotateTowardPlayer();

        // Player left range → go back to Idle first, skip chase check
        Collider[] hits = Physics.OverlapSphere(transform.position, _detectionRadius, _playerLayer);
        if (hits.Length == 0)
        {
            CurrentState = EnemyState.Idle;
            _recognizeTimer = 0f;
            return; // ← early return, don't run timer below
        }

        // Timer → chase
        _recognizeTimer += Time.deltaTime;
        if (_recognizeTimer >= _recognizeDuration)
        {
            _recognizeTimer = 0f;
            AlertMySystem();
        }
    }

    void OnChase()
    {
        if (!_isChasing)
        {
            _targetTracker.enabled = true;
            _isChasing = true;
        }

        if (_distBWPlayer <= _targetTracker.stoppingDistance)
        {
            CurrentState = EnemyState.Attack;
        }

        _enemySprite.SetAttackSprite();
    }

    void OnAttack()
    {
        RotateTowardPlayer();

        _enemyAction.TickAttack(_player,bullet);

        if (_distBWPlayer > _targetTracker.stoppingDistance + 6f)
        {
            CurrentState = EnemyState.Chase;
        }
        
        if (_isChasing)
        {
            _targetTracker.enabled = false;
            _isChasing = false;
        }
    }

    void OnDead()
    {
        _targetTracker.enabled = false;

        int pointGiven = Type switch
        {
            EnemyType.Epic    => 6,
            EnemyType.Normal  => 2,
            EnemyType.Stocker => 1,
            _                 => 0
        };

        GameManager.Instance.EarnPoints(pointGiven);
        AlertMySystem();
        Destroy(gameObject);
    }

    void FindDistanceBWPlayer()
    {
        _distBWPlayer = (_player.transform.position - transform.position).magnitude;
    }


    void AlertMySystem()
    {
        CurrentState = EnemyState.Chase;
        
        //Debug.Log("Alerting");
        // If I belong to a group, tell everyone else!
        if (_myGroup != null)
        {
            _myGroup.AlertGroup();
        }
    }

    void RotateTowardPlayer()
    {
        Vector3 dir = (_player.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
    

    void IDamaged.TakeDamage(float damageAmount)
    {
        _enemyStat.stat.currentHealth -= damageAmount;

        if (_enemyStat.stat.currentHealth <= 0)
        {
            CurrentState = EnemyState.Dead;
        }
        else
        {
            // Trigger alert right here when hit!
            AlertMySystem();
        }

        Vector3 dynamicTextPos = transform.position + new Vector3(0, _enemyStat.stat.playerScale + 0.2f, 0);

        OnDamaged?.Invoke(damageAmount, dynamicTextPos);
        OnAnyCharacterDamaged?.Invoke(damageAmount, dynamicTextPos); // Broadcast globally!
    } 

}
