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
    private EnemyAttackType _currentType;
    private FollowTarget _followTarget;
    private EnemyController _enemyController;
    private EnemyStat _enemyStat;

    [Header("Attack Settings")]
    [SerializeField] private float _attackCooldown = 1.5f;
    [SerializeField] private float _meleeLingerduration = 0.3f;

    [Header("Idle Movement Settings")]
    [SerializeField] private float _idleMoveSpeed = 2f;
    [SerializeField] private float _idleRadius = 4f;
    [SerializeField] private float _minIdleWaitTime = 1f;
    [SerializeField] private float _maxIdleWaitTime = 3f;

    private float _attackTimer = 0f;
    private float _meleeLingerTimer = 0f;

    // Idle variables
    private Vector2 _anchorPosition;
    private Vector2 _idleTargetPosition;
    private float _idleWaitTimer = 0f;
    private bool _isMovingToIdlePoint = false;

    public void SetCurrentAttackType(EnemyAttackType type) => _currentType = type;

    void RangeSetting(float scale) => _followTarget.SetStoppingDistance(20f * scale);

    public void CallStart()
    {
        _followTarget = GetComponent<FollowTarget>();
        _enemyController = GetComponent<EnemyController>();
        _enemyStat = GetComponent<EnemyStat>();

        // Set the anchor point where the enemy initially spawned
        _anchorPosition = transform.position;
    }

    // ── Idle Movement ────────────────────────────────────────────────────────
    
    // Call this once in your state machine when the enemy ENTERS the Idle state
    public void SetupIdle()
    {
        _isMovingToIdlePoint = false;
        _idleWaitTimer = Random.Range(_minIdleWaitTime, _maxIdleWaitTime);
    }

    // Call this EVERY FRAME in EnemyController's OnIdle()
    public void TickIdle()
    {
        // If we are waiting, count down the timer
        if (!_isMovingToIdlePoint)
        {
            _idleWaitTimer -= Time.deltaTime;

            if (_idleWaitTimer <= 0f)
            {
                // Pick a random point inside a circle around the anchor position
                _idleTargetPosition = _anchorPosition + (Random.insideUnitCircle * _idleRadius);
                _isMovingToIdlePoint = true;
            }
        }
        else
        {
            // Move towards the target point
            transform.position = Vector2.MoveTowards(
                transform.position, 
                _idleTargetPosition, 
                _idleMoveSpeed * Time.deltaTime
            );

            // Check if we reached the target
            if (Vector2.Distance(transform.position, _idleTargetPosition) < 0.1f)
            {
                _isMovingToIdlePoint = false;
                _idleWaitTimer = Random.Range(_minIdleWaitTime, _maxIdleWaitTime);
            }
        }
    }

    // ── Setup Attack Types ───────────────────────────────────────────────────
    public void SetupAttackType(float scale)
    {
        switch (_currentType)
        {
            case EnemyAttackType.Range:   RangeSetting(scale);   break;
            case EnemyAttackType.Malee:   MaleeSetting(scale);   break;
            case EnemyAttackType.Explode: ExplodeSetting(scale); break;
        }
    }

    public void TickAttack(GameObject player, GameObject bulletPrefab)
    {
        _attackTimer -= Time.deltaTime;

        switch (_currentType)
        {
            case EnemyAttackType.Range:
                TickRangeAttack(player, bulletPrefab);
                break;
            case EnemyAttackType.Malee:
                TickMeleeAttack(player);
                break;
            case EnemyAttackType.Explode:
                TickExplodeAttack(player);
                break;
        }
    }

    // ── Range ────────────────────────────────────────────────────────────────
    void RangeSetting() => _followTarget.SetStoppingDistance(20f);

    void TickRangeAttack(GameObject player, GameObject bulletPrefab)
    {
        if (_attackTimer > 0f || bulletPrefab == null) return;

        Vector3 dir = (player.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // FIXED: Spawn the bullet further out so large enemies don't spawn it inside themselves!
        float bulletSpawnOffset = 3f * transform.localScale.x; 
        GameObject bullet = Instantiate(bulletPrefab, transform.position + (dir * bulletSpawnOffset), Quaternion.Euler(0f, 0f, angle));

        bullet.GetComponent<Bullet>().Damage = _enemyStat.stat.damage;
        _attackTimer = _attackCooldown;
    }


    // ── Melee ────────────────────────────────────────────────────────────────
    void MaleeSetting(float scale) => _followTarget.SetStoppingDistance(2f * scale);

    void TickMeleeAttack(GameObject player)
    {
        if (_attackTimer > 0f) return;

        // Accumulate linger time while we stay in range
        _meleeLingerTimer += Time.deltaTime;

        if (_meleeLingerTimer >= _meleeLingerduration)
        {
            // Deal damage
            IDamaged playerDamageable = player.GetComponent<IDamaged>();
            playerDamageable?.TakeDamage(_enemyStat.stat.damage);

            // Reset
            _meleeLingerTimer = 0f;
            _attackTimer = _attackCooldown;
        }
    }

    // ── Explode ───────────────────────────────────────────────────────────────
    void ExplodeSetting(float scale) => _followTarget.SetStoppingDistance(3f * scale);

    void TickExplodeAttack(GameObject player)
    {
        // Placeholder — trigger explosion logic here
    }

    // Reset linger when leaving attack state so timing doesn't carry over
    public void ResetMeleeTimer() => _meleeLingerTimer = 0f;
}
