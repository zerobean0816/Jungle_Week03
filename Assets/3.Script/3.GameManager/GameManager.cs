using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public enum GameState { Playing, PuckPaused, Paused, GameOver, Win , MainMenu}

public class GameManager : MonoBehaviour
{
    [Header("References")]
    public GameObject Player; // 플레이어 컨트롤러 참조
    //public EnemySpawner enemySpawner; // 적 스포너 참조

    private GameState _gameState = GameState.Playing;
    public GameState GameState
    {
        get => _gameState;
        private set
        {
            if (_gameState == value) return; // 같은 상태면 무시

            _gameState = value;
            Debug.Log($"Game State changed to: {_gameState}");
            OnGameStateChanged?.Invoke(); // 변경 시에만 발생
        }
    }

    [Header("Event")]
    public UnityEvent OnGameOver;
    public UnityEvent OnWin;
    public UnityEvent OnPuckPauseEnter;
    public UnityEvent OnPuckPauseExit;
    public UnityEvent OnGameStateChanged;

    public int PuckCounts;

    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // This prevents duplicates
        }
    }

    private void Start()
    {
        if (Player == null)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
            if (Player == null)
            {
                Debug.LogError("[GameManager] : Player GameObject with tag 'Player' not found in the scene.");
            }
        }

        PuckCounts = 0;
    }

    private void OnEnable()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            GameState = GameState.MainMenu; // 메인 메뉴에서는 메인 메뉴 상태로 시작
        }
        else
        {
            GameState = GameState.Playing; // 게임 씬에서는 플레이 상태로 시작
        }
    }

    private void Update()
    {
        switch(GameState)
        {
            case GameState.Playing:
                // 게임이 진행 중일 때의 로직

                Player.GetComponent<PlayerStat>().stat.currentStress += Time.deltaTime;
                    
                break;
            case GameState.GameOver:
                // 게임 오버 상태일 때의 로직
                break;
            case GameState.PuckPaused:
                // 게임이 일시정지 상태일 때의 로직
                break;
            case GameState.Paused:

                break;
            case GameState.Win:
                // 게임 승리 상태일 때의 로직
                break;
            case GameState.MainMenu:
                // 메인 메뉴 상태일 때의 로직   
                break;

             default:
                 Debug.LogWarning($"Unhandled GameState: {GameState}");
                 break;
        }

    }


    public void EnterPuckPause()
    {
        if (GameState != GameState.Playing) return;

        GameState     = GameState.PuckPaused;
        Time.timeScale   = .0f;
        OnPuckPauseEnter.Invoke();
    }

    /// <summary>퍽 UI 닫기 — 게임 재개</summary>
    public void ExitPuckPause()
    {
        if (GameState != GameState.PuckPaused) return;

        GameState     = GameState.Playing;
        Time.timeScale   = 1f;
        OnPuckPauseExit.Invoke();
    }

    /// <summary>플레이어 사망 시 호출</summary>
    public void TriggerGameOver()
    {
        if (GameState == GameState.GameOver) return;

        GameState   = GameState.GameOver;
        Time.timeScale = 0f;  // 게임 오버 화면에서 멈춤
        OnGameOver.Invoke();
        Debug.Log("Game Over");
    }

    /// <summary>문 파괴 시 호출</summary>
    public void TriggerWin()
    {
        if (GameState == GameState.Win) return;

        GameState   = GameState.Win;
        Time.timeScale = 0f;
        OnWin.Invoke();
        Debug.Log("Win");
    }

    /// <summary>재시작</summary>
    public void RestartGame()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }

    public void PauseGame()
    {
        if (GameState != GameState.Playing) return;

        GameState = GameState.Paused;
        Time.timeScale = 0f;
    }
}


