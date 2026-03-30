using UnityEngine;
using TMPro;
using System.Collections;

public class FloatingText : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float floatDistance = 1.0f;
    [SerializeField] private float duration = 1.0f;

    private Transform _root;
    private TextMeshPro _textElement;

    void Awake()
    {
        _textElement = GetComponent<TextMeshPro>();
        _root = transform.parent != null ? transform.parent : transform; // ← fallback to self if no parent

        if (_textElement == null)
            Debug.LogError("[FloatingText] No TextMeshPro component found on " + gameObject.name);
        if (transform.parent == null)
            Debug.LogWarning("[FloatingText] No parent found, moving self instead");
    }

    void OnEnable()
    {
        if (_textElement == null) return; // ← guard, don't start coroutine if broken
        StopAllCoroutines();
        StartCoroutine(AnimateAndReset());
    }

    public void SetTextData(string message, Color color)
    {
        if (_textElement == null) _textElement = GetComponent<TextMeshPro>();
        _textElement.text = message;
        _textElement.color = color;
    }

    private IEnumerator AnimateAndReset()
    {
        float elapsed = 0f;
        Color startColor = _textElement.color;
        Vector3 startWorldPos = _root.position;

        yield return new WaitForSeconds(0.5f);
        
        // 1. Float up over full duration
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            _root.position = startWorldPos + new Vector3(0, t * floatDistance, 0);
            _textElement.color = new Color(startColor.r, startColor.g, startColor.b, 1f - t);

            yield return null;
        }


        _textElement.color = startColor;
        _root.gameObject.SetActive(false);
    }
}