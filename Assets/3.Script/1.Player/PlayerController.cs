using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerState
{
    Idel,
    Move,
    Carzy,
    Happy
}


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerStat))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour, IDamaged
{
    // player references
    private Rigidbody _rb; // Rigidbody 컴포넌트 참조
    private PlayerInput _playerInput; // PlayerMove 컴포넌트 참조
    private PlayerStat _playerStat; // PlayerStat 컴포넌트 참조
    private InputAction _moveAction; // 이동 입력 액션 참조
    private InputAction _attackAction; // 공격 입력 액션 참조

    private Vector3 _movement; // 이동 방향 벡터
    private bool _isAttacking; // 공격 중인지 여부

    void Start()
    {
        _rb = GetComponent<Rigidbody>(); // Rigidbody 컴포넌트 가져오기
        _playerInput = GetComponent<PlayerInput>(); // PlayerMove 컴포넌트 가져오기
        _playerStat = GetComponent<PlayerStat>(); // PlayerStat 컴포넌트 가져오기

        _moveAction = _playerInput.actions["Move"]; // PlayerMove에서 이동 입력 액션 가져오기
        _attackAction = _playerInput.actions["Attack"]; // PlayerMove에서 공격 입력 액션 가져오기
    }


    void Update()
    {
        // PlayerMove 컴포넌트에서 이동 방향과 속도를 가져와서 Rigidbody에 적용
        _movement = _moveAction.ReadValue<Vector2>(); // PlayerMove에서 이동 방향 가져오기
        _isAttacking = _attackAction.triggered; // 공격 입력 여부 확인
    }

    void FixedUpdate()
    {
        // Rigidbody에 이동 방향과 속도를 적용하여 이동
        Vector3 move = new Vector3(_movement.x,_movement.y,0f) * _playerStat.stat.moveSpeed * Time.fixedDeltaTime; // 이동 방향과 속도 계산
        //Debug.Log($"Move Vector: {move}"); // 이동 벡터 디버그 출력
        _rb.MovePosition(transform.position + move); // Rigidbody에 이동 적용
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
