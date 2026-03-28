using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAction : MonoBehaviour
{
    private Camera _mainCamera;

    [Header("Gun Settings")]
    [SerializeField] private float _maxAimRadius = 5f; // 마우스 조준 최대 반경
    [SerializeField] private GameObject _bulletPrefab; // 발사할 총알 프리팹 참조
    [SerializeField] private GameObject _coneVisual;
    [SerializeField] private Transform _firePoint;

    [Header("Aim Visuals")]
    [SerializeField] private ProceduralCone _coneVisualizer; // Drag the child object here
    [SerializeField] private float _currentRange = 5f; // Add range if you want it variable


    [SerializeField] private Stat _playerStat;

    void Start()
    {
        _mainCamera = Camera.main; // 메인 카메라 자동 참조
        _playerStat = GameManager.Instance.Player.GetComponent<PlayerStat>().stat;
    }

    // Player Puck System Staty / End commend.
    public void PerformSkill(ref bool _spaceisPressed)
    {
        if (_spaceisPressed && GameManager.Instance.GameState == GameState.Playing)
        {
            //Debug.Log("[PlayerController] : Space Pressed! Entering Puck Pause State."); // 스페이스바 입력 시 행동 예시 (콘솔에 로그 출력)
            GameManager.Instance.EnterPuckPause(); // 퍽 일시정지 상태로 전환
        }
        else if (_spaceisPressed && GameManager.Instance.GameState == GameState.PuckPaused)
        {
            //Debug.Log("[PlayerController] : Space Pressed! Exiting Puck Pause State."); // 스페이스바 입력 시 행동 예시 (콘솔에 로그 출력)
            GameManager.Instance.ExitPuckPause(); // 퍽 일시정지 상태 해제
        }

        _spaceisPressed = false; // 스페이스바 입력 초기
    }

    // Player Aiming / Firing system
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


        // Draw Cone based on Spread
         _coneVisualizer.DrawCone(_playerStat.accuracy * 2f, _currentRange); 

        // 5. Apply Rotation (Rotate around Z axis for 2D)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Optional: Visualize the tethered aim point in the Scene view
        Debug.DrawRay(transform.position, direction * 10f, Color.green);
    }

    public void HandleAttackInput(bool isPressing, bool isReleased)
    {
        if (isPressing)
        {
           FireBullet();
        }
        

        // 시각적으로 부채꼴 그리기 (디버그용)
        DrawVisualAim();
    }

    private void FireBullet()
    {
        // 현재 조준 각도 내에서 랜덤한 오차 적용
        float randomOffset = Random.Range(-_playerStat.accuracy, _playerStat.accuracy);
        Quaternion fireRotation = transform.rotation * Quaternion.Euler(0, 0, randomOffset);
        

        Instantiate(_bulletPrefab, _firePoint.position, fireRotation);
    }

    private void DrawVisualAim()
    {
        // 부채꼴의 양 끝 선을 Scene 뷰에 그림
        Vector3 leftBound = Quaternion.Euler(0, 0, _playerStat.accuracy) * transform.right;
        Vector3 rightBound = Quaternion.Euler(0, 0, -_playerStat.accuracy) * transform.right;

        Debug.DrawRay(transform.position, leftBound * _maxAimRadius, Color.red);
        Debug.DrawRay(transform.position, rightBound * _maxAimRadius, Color.red);
    }
}
