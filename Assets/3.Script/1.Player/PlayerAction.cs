using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAction : MonoBehaviour
{
    private Camera _mainCamera;

    [SerializeField] private float _maxAimRadius = 5f; // 마우스 조준 최대 반경

    void Start()
    {
        _mainCamera = Camera.main; // 메인 카메라 자동 참조
    }

    public void AimTowardsMouse2D()
    {
        // GetMouse Position in local space
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();

        float distanceFromCamera = Mathf.Abs(_mainCamera.transform.position.z - transform.position.z);

        Vector3 mouseWorldPos = _mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, distanceFromCamera));

        // 3. Create the Direction Vector (X and Y only)
        Vector2 direction = (Vector2)mouseWorldPos - (Vector2)transform.position;

        // 4. THE TETHER: Clamp the distance so the player doesn't have to move the mouse "dramatically"
        if (direction.magnitude > _maxAimRadius)
        {
            direction = direction.normalized * _maxAimRadius;
        }

        // 5. Apply Rotation (Rotate around Z axis for 2D)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Optional: Visualize the tethered aim point in the Scene view
        Debug.DrawRay(transform.position, direction, Color.green);
    }

    public void PerformSkill(ref bool _spaceisPressed)
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

    public void CallAttackOnPress(bool _isAttacking)
    {
        if (_isAttacking)
        {
            _isAttacking = false; // 공격 입력 초기화
            Debug.Log("[PlayerController] : Attack!"); // 공격 행동 예시 (콘솔에 로그 출력)
            
        }
    }
}
