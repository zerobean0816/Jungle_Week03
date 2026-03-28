using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed = 55f; // 총알 속도
    [SerializeField] private float _lifetime = 2f; // 총알 수명

    [SerializeField] private Rigidbody _rb;

    public float Damage;
    public bool isPenetrating; // 관통 여부

    private PlayerStat playerStat; // 플레이어 스탯 참조


    private void Start()
    {
        Destroy(gameObject, _lifetime); 

        // 플레이어 스탯 참조 (GameManager를 통한 캐싱 권장)
        var player = GameManager.Instance.Player;
        if (player != null)
        {
            PlayerStat playerStat = player.GetComponent<PlayerStat>();
            Damage = playerStat.stat.damage; 
            transform.localScale *= playerStat.stat.bulletScale;
        }

        _rb = GetComponent<Rigidbody>();
        _rb.linearVelocity = transform.right * _speed;
    }


    private void OnTriggerEnter(Collider collision)
    {
        if (collision.GetComponent<IDamaged>() != null)
        {
            collision.GetComponent<IDamaged>().TakeDamage(Damage); // 충돌한 객체가 IDamaged 인터페이스를 구현했다면 데미지 적용
        }

        if (isPenetrating || collision.gameObject.CompareTag("Bullet"))
        {
            return;
        }
        Destroy(gameObject); // 총알 제거
    }
}
