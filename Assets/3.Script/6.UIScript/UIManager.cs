using UnityEngine;
using UnityEngine.SceneManagement;

public enum UIType
{
    None,
    MainMenu,
    InGame,
    InPuckPause,
    PauseMenu,
    GameOver,
    GameClear
}

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public UIType CurrentUI = UIType.None;

    [SerializeField] private GameObject _mainMenuUI;
    [SerializeField] private GameObject _gameUI;
    [SerializeField] private GameObject _puckPauseUI;
    [SerializeField] private GameObject _pauseUI;
    [SerializeField] private GameObject _gameOverUI;
    [SerializeField] private GameObject _gameClearUI;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        DismissAllUI();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged.AddListener(ChangeUIState);
        }


        // 초기 UI 상태 설정
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            CurrentUI = UIType.MainMenu;
        }
        else
        {
            CurrentUI = UIType.None; // 초기 상태는 None으로 설정
        }

        ChangeUIState(); // 초기 UI 상태 설정
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged.RemoveListener(ChangeUIState);
        }
    }

    public void ChangeUIState()
    {
        if (GameManager.Instance == null)
            return;

        switch (GameManager.Instance.GameState)
        {
            case GameState.Playing:
                CurrentUI = UIType.InGame;
                OnPlayState();

                    break;
                
                case GameState.PuckPaused:
                    CurrentUI = UIType.InPuckPause;
                    OnPuckPauseState();

                    break;

                case GameState.Paused:
                    CurrentUI = UIType.PauseMenu;
                    OnPauseState();
                    break;

                case GameState.GameOver:
                    CurrentUI = UIType.GameOver;
                    OnGameOverUI();
                    break;

                case GameState.Win:
                    CurrentUI = UIType.GameClear; // 승리도 게임 클리어 UI로 처리
                    OnGameClearUI();
                    break;

                case GameState.MainMenu:
                    CurrentUI = UIType.MainMenu;
                    OnMainMenuState();
                    break;

                default:
                    CurrentUI = UIType.None;
                    break;
            }
    }

    void OnPlayState()
    {
        ShowGameUI();
        DismissMainMenuUI();
        DismissPuckPauseUI();
        DismissPauseUI();   
    }

    void OnPuckPauseState()
    {
        ShowPuckPauseUI();
        ShowGameUI();
        DismissMainMenuUI();
        DismissPauseUI();
    }

    void OnPauseState()
    {
        ShowPauseUI();
    }

    void OnMainMenuState()
    {
        ShowMainMenuUI();
        DismissGameUI();
        DismissPuckPauseUI();
        DismissPauseUI();
    }

   void OnGameOverUI()
    {
        DismissAllUI();
        ShowGameOverUI();
    }

    void OnGameClearUI()
    {
        DismissAllUI();
        ShowGameClearUI();
    }

    void DismissAllUI()
    {
        DismissMainMenuUI();
        DismissGameUI();
        DismissPuckPauseUI();
        DismissPauseUI();
        DismissGameOverUI();
        DismissGameClearUI();
    }

     void ShowPuckPauseUI()
    {
        _puckPauseUI.SetActive(true);
    }

     void DismissPuckPauseUI()
    {
        _puckPauseUI.SetActive(false);
    }

     void ShowGameUI()
    {
        _gameUI.SetActive(true);
    }

     void DismissGameUI()
    {
        _gameUI.SetActive(false);
    }

     void ShowMainMenuUI()
    {
        _mainMenuUI.SetActive(true);
    }

     void DismissMainMenuUI()
    {
        _mainMenuUI.SetActive(false);
    }

     void ShowPauseUI()
    {
        _pauseUI.SetActive(true);
    }

     void DismissPauseUI()
    {
        _pauseUI.SetActive(false);
    }

    void ShowGameOverUI()
    {
        _gameOverUI.SetActive(true);
    }

    void DismissGameOverUI()
    {
        _gameOverUI.SetActive(false);
    }

    void ShowGameClearUI()
    {
        _gameClearUI.SetActive(true);
    }  

    void DismissGameClearUI()
    {
        _gameClearUI.SetActive(false);
    }
}
