using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerState
{
    Idle,
    Crazy,
    Panic,
    Stuned,
}


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerStat))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PuckHandler))]
[RequireComponent(typeof(PlayerAction))]
[RequireComponent(typeof(PlayerStressControl))]

public class PlayerController : MonoBehaviour, IDamaged
{
    // player references
    [SerializeField] private Rigidbody _rb; // Rigidbody 컴포넌트 참조
    [SerializeField] private PlayerInput _playerInput; // PlayerMove 컴포넌트 참조
    [SerializeField] private PlayerStat _playerStat; // PlayerStat 컴포넌트 참조
    [SerializeField] private PuckHandler _puckHandler; // PuckHandler 컴포넌트 참조
    [SerializeField] private PlayerAction _playerAction; // PlayerAction 컴포넌트 참조
    [SerializeField] private PlayerStressControl _playerStressControl; // PlayerAction 컴포넌트 참조


    public event Action<float, Vector3> OnDamaged;
    public static event Action<float, Vector3> OnAnyCharacterDamaged;

    // player state
    public PlayerState CurrentState; // 현재 플레이어 상태
    private PlayerState _lastState; // 이전 플레이어 상태

    // input actions
    [SerializeField] private InputAction _moveAction; // 이동 입력 액션 참조
    [SerializeField] private InputAction _attackAction; // 공격 입력 액션 참조
    [SerializeField] private InputAction _spaceAction; // 스페이스바 입력 액션 참조

    [SerializeField] private Vector3 _mousePosition;

    // player state variables
    [SerializeField] private Vector3 _movement; // 이동 방향 벡터
    [SerializeField] private bool _isMousePressing; // 마우스 클릭 중인지
    [SerializeField] private bool _isMouseReleased; // 마우스 릴리즈 중인지


    [SerializeField] private bool _spaceisPressed; // 스페이스바 입력 여부

    private float _previousScaleValue = 1f;
    private Camera _mainCamera; // 메인 카메라 참조
    [SerializeField] private Transform _textTransfrom;

    private bool _isControlable => 
        CurrentState != PlayerState.Crazy && CurrentState != PlayerState.Panic && CurrentState != PlayerState.Stuned; // 이동 가능한 상태인지 여부


    void Start()
    {
        _rb = GetComponent<Rigidbody>(); // Rigidbody 컴포넌트 가져오기
        _playerInput = GetComponent<PlayerInput>(); // PlayerMove 컴포넌트 가져오기
        _playerStat = GetComponent<PlayerStat>(); // PlayerStat 컴포넌트 가져오기
        _puckHandler = GetComponent<PuckHandler>(); // PuckHandler 컴포넌트 가져오기
        _playerAction = GetComponent<PlayerAction>(); // PlayerAction 컴포넌트 가져오기cvbxn
        _playerStressControl = GetComponent<PlayerStressControl>();

        _moveAction = _playerInput.actions["Move"]; // PlayerMove에서 이동 입력 액션 가져오기
        _attackAction = _playerInput.actions["Attack"]; // PlayerMove에서 공격 입력 액션 가져오기
        _spaceAction = _playerInput.actions["Jump"]; // PlayerMove에서 스페이스바 입력 액션 가져오기    


        CurrentState = PlayerState.Idle; // 초기 상태 설정
        _lastState = CurrentState;

        _mainCamera = Camera.main; // GameManager에서 메인 카메라 참조 가져오기
    }


    void Update()
    {
        ReadInputs();

        _playerAction.PerformSkill(ref _spaceisPressed);

        if (GameManager.Instance.GameState == GameState.PuckPaused)
        {
            _movement = Vector3.zero;
            return;
        }
        
        _movement = _moveAction.ReadValue<Vector2>();

        if (GameManager.Instance.GameState == GameState.Playing)
        {
            // 1. Health Regen
            if (_playerStat.stat.currentHealth < _playerStat.stat.maxHealth)
            {
                _playerStat.stat.currentHealth += _playerStat.stat.hpRegen * Time.deltaTime;
            }

            // 2. Stress Regen (ONLY if the player is still in normal Idle state)
            if (CurrentState == PlayerState.Idle)
            {
                _playerStressControl.AddStress(_playerStat.stat.stressRegen * Time.deltaTime, _textTransfrom.position);
            }
        }
    }

    void FixedUpdate()
    {   
        if (CurrentState != _lastState)
        {
            _lastState = CurrentState;
        }

        switch (CurrentState)
        {
            case PlayerState.Idle:
                ActIdle();
                break;

            case PlayerState.Crazy:
                // Allow them to move, but they might be using a special puck!
                ActIdle(); 
                break;

            case PlayerState.Panic:
                // Allow them to move slowly
                ActIdle();
                break;

            case PlayerState.Stuned:
                // Stop all movement immediately
                _rb.linearVelocity = Vector3.zero;
                break;
        }

    }

    void ReadInputs()
    {
        if (_attackAction.triggered)
        {
            _isMousePressing = true;
        } // 공격 입력 여부 확인

        if (_attackAction.WasReleasedThisFrame())
        {
            _isMouseReleased = true;
        } // 공격 입력 릴리즈 여부 확인

        if (_spaceAction.triggered)
        {
            _spaceisPressed = true;
        } // 스페이스바 입력 여부 확인
    }

    void ActIdle()
    {
        if (GameManager.Instance.GameState != GameState.Playing)
        {
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            return;
        }

        _playerAction.HandleAttackInput(_isMousePressing, _isMouseReleased);
        _playerAction.AimTowardsMouse2D();

        Vector3 targetVelocity = new Vector3(_movement.x, _movement.y, 0f) * _playerStat.stat.moveSpeed;

        _rb.linearVelocity = Vector3.Lerp(_rb.linearVelocity, targetVelocity, Time.fixedDeltaTime * 20f);

        if (_isMouseReleased)
        {
            _isMousePressing = false;
            _isMouseReleased = false;
        }
    }

    void ActCrazy()
    {
        Debug.Log("[PlayerController] : Crazy State: Increased movement speed and attack power!"); // 크레이지 상태에서의 행동 예시 (이동 속도와 공격력 증가)
        
    }

    void ActPanic()
    {
        Debug.Log("[PlayerController] : Panic State: Decreased movement speed and attack power!"); // 패닉 상태에서의 행동 예시 (이동 속도와 공격력 감소)
    }


    public void TakeDamage(float damageAmount)
    {
        float finalDamage = damageAmount * _playerStat.stat.damageReceived; // 배율 적용
        _playerStat.stat.currentHealth -= finalDamage;
        _playerStressControl.AddStress(_playerStat.stat.stressPerDamage, _textTransfrom.position);

        OnDamaged?.Invoke(finalDamage,_textTransfrom.position);
        OnAnyCharacterDamaged?.Invoke(finalDamage, _textTransfrom.position); // Broadcast globally!
        
        if (_playerStat.stat.currentHealth <= 0)
            GameManager.Instance.TriggerGameOver();  // 체력이 0 이하가 되면 사망 처리
    }

    public void RecalculatePlayerState()
    {
        transform.localScale = new Vector3(
        _playerStat.stat.playerScale,
        _playerStat.stat.playerScale,
        1f
        );

        _previousScaleValue = _playerStat.stat.playerScale;
    }
}
