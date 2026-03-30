
using UnityEngine;

public class Puck_RandomGenerator : MonoBehaviour
{
    [SerializeField] GameObject PuckUIContainer; // Parent container for the generated puck UI elements

    public int puckCount = 5; // Number of pucks to generate

    [Header("Reference")]
    [SerializeField] private GameObject cardPerfab; // Prefab for the puck card

    void Start()
    {
        if (cardPerfab == null)
        {
            Debug.LogError("Card prefab is not assigned in the inspector.");
        }
    }

    void OnEnable()
    {
        SpawnRandomPuck();
    }

    void OnDisable()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            // Destroy the GameObject attached to the child transform
            Destroy(transform.GetChild(i).gameObject);
        }
    }


    public void SpawnRandomPuck()
    {
        for (int i = 0; i < puckCount; i++)
        {
            int randomIndex = Random.Range(0, GameManager.Instance.PuckDatas.Count);
            PuckData randomPuckData = GameManager.Instance.PuckDatas[randomIndex];

            GameObject newPuckCard = Instantiate(cardPerfab, Vector3.zero, Quaternion.identity);
            newPuckCard.transform.SetParent(this.transform, false);

            UIPuckItem puckItem = newPuckCard.GetComponent<UIPuckItem>();
            puckItem.Canvas = PuckUIContainer;
            puckItem.Init(randomPuckData); // ← pass data directly, set and init in one step
        }
    }
}
