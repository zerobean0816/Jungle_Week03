using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _lifetime = 2f; // 총알 수명

    [SerializeField] private Rigidbody _rb;

    public float Damage;
    public float Speed = 55f; // 총알 속도
    public bool isPenetrating; // 관통 여부


    private void Start()
    {
        Destroy(gameObject, _lifetime); 

        _rb = GetComponent<Rigidbody>();
        _rb.linearVelocity = transform.right * Speed;
    }

    public void SetBulletScale(float scale)
    {
        transform.localScale *= scale;
    }

    public void SetBulletDamage(float damage)
    {
        Damage = damage;
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
