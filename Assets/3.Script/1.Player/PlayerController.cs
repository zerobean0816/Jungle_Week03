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
public class PlayerController : MonoBehaviour, IDamaged
{
    // player references
    [SerializeField] private Rigidbody _rb; // Rigidbody 컴포넌트 참조
    [SerializeField] private PlayerInput _playerInput; // PlayerMove 컴포넌트 참조
    [SerializeField] private PlayerStat _playerStat; // PlayerStat 컴포넌트 참조


    // player state
    public PlayerState CurrentState; // 현재 플레이어 상태
    private PlayerState _lastState; // 이전 플레이어 상태

    // input actions
    [SerializeField] private InputAction _moveAction; // 이동 입력 액션 참조
    [SerializeField] private InputAction _attackAction; // 공격 입력 액션 참조
    [SerializeField] private InputAction _spaceAction; // 스페이스바 입력 액션 참조

    // player state variables
    [SerializeField] private Vector3 _movement; // 이동 방향 벡터
    [SerializeField] private bool _isAttacking; // 공격 중인지 여부
    [SerializeField] private bool _hasChangedState; // 상태 변경 여부
    [SerializeField] private bool _spaceisPressed; // 스페이스바 입력 여부
    private bool _isControlable => 
        CurrentState != PlayerState.Crazy && CurrentState != PlayerState.Panic && CurrentState != PlayerState.Stuned; // 이동 가능한 상태인지 여부

    void Start()
    {
        _rb = GetComponent<Rigidbody>(); // Rigidbody 컴포넌트 가져오기
        _playerInput = GetComponent<PlayerInput>(); // PlayerMove 컴포넌트 가져오기
        _playerStat = GetComponent<PlayerStat>(); // PlayerStat 컴포넌트 가져오기

        _moveAction = _playerInput.actions["Move"]; // PlayerMove에서 이동 입력 액션 가져오기
        _attackAction = _playerInput.actions["Attack"]; // PlayerMove에서 공격 입력 액션 가져오기
        _spaceAction = _playerInput.actions["Jump"]; // PlayerMove에서 스페이스바 입력 액션 가져오기    

        _hasChangedState = true;

        CurrentState = PlayerState.Idle; // 초기 상태 설정
        _lastState = CurrentState;
    }


    void Update()
    {
        // PlayerMove 컴포넌트에서 이동 방향과 속도를 가져와서 Rigidbody에 적용
        _movement = _moveAction.ReadValue<Vector2>(); // PlayerMove에서 이동 방향 가져오기

        if (_attackAction.triggered)
        {
            
            _isAttacking = true;
        } // 공격 입력 여부 확인

        if (_spaceAction.triggered)
        {
            
            _spaceisPressed = true;
        } // 스페이스바 입력 여부 확인

        CallSkillStateOnPress();
    }

    void FixedUpdate()
    {   
        // 상태가 변경되었는지 확인
        if (CurrentState != _lastState)
        {
            _hasChangedState = false;
            _lastState = CurrentState;
        }

        switch (CurrentState)
        {
            case PlayerState.Idle:
                ActIdle();
                break;

            case PlayerState.Crazy:
                ActCrazy();
                break;

            case PlayerState.Panic:
                ActPanic();
                break;

            case PlayerState.Stuned:
                break;
        }

        _hasChangedState = true;
    }

    void ActIdle()
    {
        CallAttackOnPress();

        Vector3 move = new Vector3(_movement.x,_movement.y,0f) * _playerStat.stat.moveSpeed * Time.fixedDeltaTime; // 이동 방향과 속도 계산
        _rb.MovePosition(transform.position + move); // Rigidbody에 이동 적용
    }

    void ActCrazy()
    {
        Debug.Log("[PlayerController] : Crazy State: Increased movement speed and attack power!"); // 크레이지 상태에서의 행동 예시 (이동 속도와 공격력 증가)
        
    }

    void ActPanic()
    {
        Debug.Log("[PlayerController] : Panic State: Decreased movement speed and attack power!"); // 패닉 상태에서의 행동 예시 (이동 속도와 공격력 감소)
    }


    void CallSkillStateOnPress()
    {
        if (_spaceisPressed && GameManager.Instance.GameState == GameState.Playing)
        {
            Debug.Log("[PlayerController] : Space Pressed! Entering Puck Pause State."); // 스페이스바 입력 시 행동 예시 (콘솔에 로그 출력)
            GameManager.Instance.EnterPuckPause(); // 퍽 일시정지 상태로 전환
        }
        else if (_spaceisPressed && GameManager.Instance.GameState == GameState.PuckPaused)
        {
            Debug.Log("[PlayerController] : Space Pressed! Exiting Puck Pause State."); // 스페이스바 입력 시 행동 예시 (콘솔에 로그 출력)
            GameManager.Instance.ExitPuckPause(); // 퍽 일시정지 상태 해제
        }

        _spaceisPressed = false; // 스페이스바 입력 초기
    }

    void CallAttackOnPress()
    {
        if (_isAttacking)
        {
            _isAttacking = false; // 공격 입력 초기화
            Debug.Log("[PlayerController] : Attack!"); // 공격 행동 예시 (콘솔에 로그 출력)
            
        }
    }

    public void TakeDamage(float damageAmount)
    {
        _playerStat.stat.currentHealth -= damageAmount; // 체력 감소

        if (_playerStat.stat.currentHealth <= 0)
        {
            GameManager.Instance.TriggerGameOver(); // 체력이 0 이하가 되면 사망 처리
        }
    }
}
