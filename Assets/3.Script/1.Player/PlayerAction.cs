
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerAction : MonoBehaviour
{
    private Camera _mainCamera;


    [Header("Gun Settings")]
    [SerializeField] private float _maxAimRadius = 5f; // 마우스 조준 최대 반경
    [SerializeField] private GameObject _bulletPrefab; // 발사할 총알 프리팹 참조


    [Header("Aim Visuals")]
    [SerializeField] private ProceduralCone _coneVisualizer; // Drag the child object here
    [SerializeField] private float _currentRange = 5f; // Add range if you want it variable
    [SerializeField] private Transform _meshTransfrom;

    [SerializeField] private Stat _playerStat;

    [SerializeField] private bool _reloadIsComplete;
     private Coroutine _reloadCoroutine;

    private float _reloadTimer = 0f;
    private float _reloadDuration = 1f;

    const float MAXSPREADANGLE = 50f;
    [SerializeField] private float currentSpread;

    void Start()
    {
        _mainCamera = Camera.main; // 메인 카메라 자동 참조
        _playerStat = GameManager.Instance.Player.GetComponent<PlayerStat>().stat;
        _reloadIsComplete = true;
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

        currentSpread = MAXSPREADANGLE *(1f - _playerStat.accuracy/ 100f);
        currentSpread = Mathf.Clamp(currentSpread, 0f, MAXSPREADANGLE); // Keep it safe


        // 4. THE TETHER: Clamp the distance so the player doesn't have to move the mouse "dramatically"
        if (direction.magnitude > _maxAimRadius)
        {
            direction = direction.normalized * _maxAimRadius;
        }


        // Draw Cone based on Spread
         _coneVisualizer.DrawCone(currentSpread * 2f, _currentRange); 

        // 5. Apply Rotation (Rotate around Z axis for 2D)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _meshTransfrom.rotation = Quaternion.Euler(0f, 0f, angle);

    }

    public void HandleAttackInput(bool isPressing, bool isReleased)
    {
        if (isPressing && _reloadIsComplete)
        {
            FireBullet();
            StartReload();
        }
    }

    private void StartReload()
    {
        if (_reloadCoroutine != null)
            StopCoroutine(_reloadCoroutine);

        _reloadCoroutine = StartCoroutine(ReloadRoutine());
    }

    public float GetReloadProgress()
    {
        if (_reloadIsComplete) return 1f;
        return _reloadTimer / _reloadDuration;
    }

    private IEnumerator ReloadRoutine()
    {
        _reloadDuration = 1f / _playerStat.attackSpeed;
        _reloadTimer = 0f;
        _reloadIsComplete = false;

        while (_reloadTimer < _reloadDuration)
        {
            _reloadTimer += Time.deltaTime;
            yield return null; // ✅ WaitForSeconds 대신 매 프레임 업데이트
        }

        _reloadIsComplete = true;
    }

    private void FireBullet()
    { // Maximum possible spread in degrees at 0 accuracy
        float randomOffset = (Random.Range(-currentSpread, currentSpread)
                        + Random.Range(-currentSpread, currentSpread)) / 2f;

        Quaternion fireRotation = _meshTransfrom.rotation * Quaternion.Euler(0, 0, randomOffset);
        
        float offsetDistance = 2.0f; 
        Vector3 firePoint = _meshTransfrom.position + (_meshTransfrom.right * offsetDistance);

        Instantiate(_bulletPrefab, firePoint, fireRotation);

        _bulletPrefab.GetComponent<Bullet>().SetBulletDamage(_playerStat.damage);
        _bulletPrefab.GetComponent<Bullet>().SetBulletScale (_playerStat.bulletScale);
    }
}
