using UnityEngine;

public class EnemySprite : MonoBehaviour
{
    SpriteRenderer _spriteRenderer;
    [SerializeField] Sprite _idelSprite;
    [SerializeField] Sprite _attackSprite;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetIdelSprite()
    {
        _spriteRenderer.color = Color.orange;
    }

    public void SetAttackSprite()
    {
        _spriteRenderer.color = Color.red;
    }
}
