using System.Collections.Generic;
using UnityEngine;

public class FloatingTextSpawner : MonoBehaviour
{
    // Static instance so any script can call it easily without GetComponent
    public static FloatingTextSpawner Instance;

    [Header("Pool Settings")]
    [SerializeField] private GameObject textPrefab; // Drag your FloatingText prefab here
    [SerializeField] private int _poolSize = 5; // How many to create at the start

    private int _poolIndex = 0;

    private List<GameObject> _pooledTexts = new List<GameObject>();

    void Awake()
    {
        // 1. Singleton setup so we can use TextPoolSpawner.Instance
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 2. Pre-warm the pool by creating initial objects
        for (int i = 0; i < _poolSize; i++)
        {
            CreateNewPoolObject();
        }

    }

    void OnEnable()
    {
        PlayerController.OnAnyCharacterDamaged += HandleDamageText;
        EnemyController.OnAnyCharacterDamaged += HandleDamageText;
    }

    void OnDisable()
    {
        PlayerController.OnAnyCharacterDamaged -= HandleDamageText;
        EnemyController.OnAnyCharacterDamaged -= HandleDamageText;       
    }

    private void HandleDamageText(float damage, Vector3 position)
    {
        Color orangeRed = new Color(1.0f, 0.27f, 0.0f);
        
        // Call your looping pool function here!
        PoolText( "- " +damage.ToString("F0"), orangeRed, position);
        Debug.Log(position);
    }


    // Helper method to instantiate, disable, and add to list
    private GameObject CreateNewPoolObject()
    {
        // Instantiate the root parent, not the text child
        GameObject root = new GameObject("FloatingText_Root");
        
        // Instantiate text prefab as child of root
        GameObject textObj = Instantiate(textPrefab, root.transform);
        textObj.transform.localPosition = Vector3.zero;

        root.SetActive(false);
        _pooledTexts.Add(root);

        return root;
    }

    // 3. The main method called by your Player or Enemy scripts
    public void PoolText(string message, Color color, Vector3 _position)
    {
       GameObject root = _pooledTexts[_poolIndex];
        if (root == null) return;

        if (root.activeInHierarchy)
            root.SetActive(false);

        // Set position BEFORE enabling so OnEnable captures the correct position
        root.transform.position = _position + new Vector3(0, 1.5f, 0);

        FloatingText floatingText = root.GetComponentInChildren<FloatingText>();
        if (floatingText != null)
            floatingText.SetTextData(message, color);

        root.SetActive(true); // ← OnEnable fires here, position is already correct

        _poolIndex = (_poolIndex + 1) % _poolSize;

    }
}