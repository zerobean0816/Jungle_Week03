using UnityEngine;
using UnityEngine.Events;

public enum GameState { Playing, PuckPaused, GameOver, Win }

public class GameManager : MonoBehaviour
{
    [Header("References")]
    public GameObject player; // 플레이어 컨트롤러 참조
    //public EnemySpawner enemySpawner; // 적 스포너 참조
    

    public GameState currentState { get; private set; } = GameState.Playing;


    [Header("Event")]
    public UnityEvent onGameOver;
    public UnityEvent onWin;
    public UnityEvent onPuckPauseEnter;
    public UnityEvent onPuckPauseExit;


    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Is this line here?
        }
        else
        {
            Destroy(gameObject); // This prevents duplicates
        }
    }

    private void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogError("Player GameObject with tag 'Player' not found in the scene.");
            }
        }
    }

    private void Update()
    {
        switch(currentState)
        {
            case GameState.Playing:
                // 게임이 진행 중일 때의 로직
                break;
            case GameState.GameOver:
                // 게임 오버 상태일 때의 로직
                break;
            case GameState.PuckPaused:
                // 게임이 일시정지 상태일 때의 로직
                break;
            case GameState.Win:
                // 게임 승리 상태일 때의 로직
                break;

        }
    }


    public void EnterPuckPause()
    {
        if (currentState != GameState.Playing) return;

        currentState     = GameState.PuckPaused;
        Time.timeScale   = 0f;
        onPuckPauseEnter.Invoke();
    }

    /// <summary>퍽 UI 닫기 — 게임 재개</summary>
    public void ExitPuckPause()
    {
        if (currentState != GameState.PuckPaused) return;

        currentState     = GameState.Playing;
        Time.timeScale   = 1f;
        onPuckPauseExit.Invoke();
    }

    /// <summary>플레이어 사망 시 호출</summary>
    public void TriggerGameOver()
    {
        if (currentState == GameState.GameOver) return;

        currentState   = GameState.GameOver;
        Time.timeScale = 0f;  // 게임 오버 화면에서 멈춤
        onGameOver.Invoke();
        Debug.Log("Game Over");
    }

    /// <summary>문 파괴 시 호출</summary>
    public void TriggerWin()
    {
        if (currentState == GameState.Win) return;

        currentState   = GameState.Win;
        Time.timeScale = 0f;
        onWin.Invoke();
        Debug.Log("Win");
    }

    /// <summary>재시작</summary>
    public void RestartGame()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
}


