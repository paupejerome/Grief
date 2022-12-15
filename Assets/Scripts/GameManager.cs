using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager gameManagerInstance;
    [HideInInspector]
    public bool isPaused { get; private set; }
    [SerializeField]
    GameObject pauseMenu;
    [SerializeField]
    GameObject _mainMenu;
    [SerializeField]
    GameObject _camera;
    [SerializeField]
    public GameObject _player;
    [SerializeField]
    public GameObject _taskBox;

    public bool canPause = true;

    private void Awake()
    {
        if(gameManagerInstance == null)
        {
            gameManagerInstance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public static GameManager GetInstance()
    {
        return gameManagerInstance;
    }

    public void StartGame()
    {
        LevelManager.instance.StartGame();
        HideMainMenu();
        HideMainMenuCamera();
        EnableTaskBar("Go to work");
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            Pause();
        }
    }

    public void Pause()
    {
        if (!canPause)
            return;

        if (!EventManager.playingEvent)
        {
            _player.GetComponent<CharacterMovement>().EnableMovement(pauseMenu.activeInHierarchy);

            if (MinigameManager.GetInstance() != null)
                MinigameManager.GetInstance().Pause();
        }
        pauseMenu.SetActive(!pauseMenu.activeInHierarchy);
        isPaused = pauseMenu.activeInHierarchy;
    }

    public void QuitGame()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
    #endif
        Application.Quit();
    }

    public void HideMainMenu()
    {
        _mainMenu.SetActive(false);
    }
    public void HideMainMenuCamera()
    {
        _camera.SetActive(false);
    }
    public void UnhideMainMenuCamera()
    {
        _camera.SetActive(true);
    }

    public void EnableTaskBar(string text)
    {
        StartCoroutine(EnableTaskBarWithDelay(1, text));
    }

    public void HideTaskbar()
    {
        _taskBox.SetActive(false);
	}

    IEnumerator EnableTaskBarWithDelay(float delay, string text)
    {
        yield return new WaitForSeconds(delay);
        _taskBox.SetActive(true);
        _taskBox.GetComponent<TaskIndicator>().ChangeText(text);
    }
}
