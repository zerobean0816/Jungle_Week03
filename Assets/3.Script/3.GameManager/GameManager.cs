using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

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
            Debug.Log($"[GameManager] : Game State changed to: {_gameState}");
            OnGameStateChanged?.Invoke(); // 변경 시에만 발생
        }
    }

    [Header("Event")]
    public UnityEvent OnGameOver;
    public UnityEvent OnWin;
    public UnityEvent OnPuckPauseEnter;
    public UnityEvent OnPuckPauseExit;
    public UnityEvent OnGameStateChanged;

    public UIPuckSlotContainer SlotContainer;

    [Header("Puck Pause Settings")]
    private float _puckPauseCooldown = 3f;
    private float _puckPauseCooldownTimer = 0f;
    public bool CanOpenPuckPause => _puckPauseCooldownTimer <= 0f && PuckPoints > 0;
    public float PuckPauseCooldownRemaining => _puckPauseCooldownTimer;


    [Header("Puck Datas")]
    public int PuckCounts;
    public List<PuckData> PuckDatas;
    public List<PuckData> StressPuckData;

    [Header("Puck Point Settings")]
    private int _puckPoints = 3;
    public int PuckPoints
    {
        get { return _puckPoints; }
        set
        {
            _puckPoints = Mathf.Max(0, value); // ← Max, not Min, prevents going below 0
        }
    }

    public bool CanEquipPuck(PuckData puck) => PuckPoints >= puck.PointCost;
    public void EarnPoints(int amount) => PuckPoints += amount;
    public void SpendPoints(PuckData puck) => PuckPoints -= puck.PointCost;
    public void RefundPoints(PuckData puck) => PuckPoints += puck.PointCost;


    public int Spowner = 0;
    private int MaxSpowner = 5;

    public static GameManager Instance;

    public event Action OnPuckPaused;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
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
                Debug.LogWarning("[GameManager] : Player GameObject with tag 'Player' not found in the scene.");
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

        switch (GameState)
        {
            case GameState.Playing:
                Player.GetComponent<PlayerStat>().stat.currentStress += Time.deltaTime;

                if (_puckPauseCooldownTimer > 0f)
                    _puckPauseCooldownTimer -= Time.unscaledDeltaTime;

                if (Spowner >= MaxSpowner)
                {
                     GameState = GameState.Win;
                }

                // FIXED: Use PauseGame() directly
                if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
                {
                    PauseGame(); 
                }
                break;

            case GameState.PuckPaused:
                if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
                {
                    ExitPuckPause(); // FIXED: Use the clean exit method you already made!
                }
                break;

            case GameState.Paused:
                // ADDED: Allow the player to unpause by pressing Esc again!
                if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
                {
                    UnPauseGame();
                }
                break;
            case GameState.Win:
                Time.timeScale = 0f;

                break;
            case GameState.MainMenu:
                // 메인 메뉴 상태일 때의 로직   
                break;

             default:
                 break;
        }

    }


    public void EnterPuckPause()
    {
        if (GameState != GameState.Playing) return;
        if (!CanOpenPuckPause)
        {
            Debug.Log($"[GameManager] Puck pause on cooldown: {_puckPauseCooldownTimer:F1}s remaining");
            return;
        }
        // No timer set here anymore

        PuckPoints--;
        Player.GetComponent<PlayerStat>().EnterPuckPause();
        OnPuckPaused?.Invoke();

        GameState = GameState.PuckPaused;
        Time.timeScale = 0f;
        OnPuckPauseEnter.Invoke();
    }

    /// <summary>퍽 UI 닫기 — 게임 재개</summary>
    public void ExitPuckPause()
    {
        if (GameState != GameState.PuckPaused) return;

        _puckPauseCooldownTimer = _puckPauseCooldown; // ← moved here from EnterPuckPause

        GameState = GameState.Playing;
        Player.GetComponent<PlayerStat>().ConfirmModifiers();

        Time.timeScale = 1f;
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
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
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

    public void UnPauseGame()
    {
        if (GameState != GameState.Paused) return;

        GameState = GameState.Playing;
        Time.timeScale = 1f;
    }
}


