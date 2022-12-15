using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public enum WinCondition
{
    NONE,
    COLLECT_GOLD,
    TRAVEL_DISTANCE,
    SURVIVE
}

public class MinigameManager : MonoBehaviour
{
    private static MinigameManager instance;
    [SerializeField] TMP_Text winConditionText;
    [SerializeField] TMP_Text countdownText;
    [SerializeField] Player player;
    [SerializeField] GameObject menuPannel;
    [SerializeField] GameObject gamePannel;
    [SerializeField] GameObject winPannel;
    [SerializeField] int coinsToCollect;
    [SerializeField] float distanceToReach;
    [SerializeField] float distanceToSurvive;
    [SerializeField] InputActionAsset actionAsset;

    GameManager gameManager;

    int count = 3;

    bool canPause = true;
    bool wasMoving = false;

    public WinCondition winCondition { get; private set; }
    public bool isVictorius { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        gameManager = GameManager.GetInstance();
    }

    void Start()
    {
        isVictorius = false;

        switch (DifficultyCurve.day)
        {
            case 1:
                winCondition = WinCondition.TRAVEL_DISTANCE;
                break;

            case 2:
            case 3:
                int rand = Random.Range(0, 2);
                switch (rand)
                {
                    case 0:
                        winCondition = WinCondition.COLLECT_GOLD;
                        break;

                    case 1:
                        winCondition = WinCondition.TRAVEL_DISTANCE;
                        break;
                }
                break;

            default:
                int rand2 = Random.Range(0, 3);
                switch (rand2)
                {
                    case 0:
                        winCondition = WinCondition.COLLECT_GOLD;
                        break;

                    case 1:
                        winCondition = WinCondition.TRAVEL_DISTANCE;
                        break;

                    case 2:
                        winCondition = WinCondition.SURVIVE;
                        break;
                }
                break;
        }
    }

    void Update()
    {
        if (!isVictorius)
        {
            switch (winCondition)
            {
                case WinCondition.COLLECT_GOLD:
                    CheckGold();
                    break;

                case WinCondition.TRAVEL_DISTANCE:
                    CheckDistance();
                    break;

                case WinCondition.SURVIVE:
                    CheckSurvive();
                    break;
            }
        }
    }

    public static MinigameManager GetInstance()
    {
        return instance;
    }

    void CheckGold()
    {
        if (player.nbCoins >= coinsToCollect)
        {
            Victory();
        }
    }

    void CheckDistance()
    {
        if (player.distance >= distanceToReach)
        {
            Victory();
        }
    }

    void CheckSurvive()
    {
        if (player.distance >= distanceToSurvive)
        {
            Victory();
        }
    }

    public void EndGame()
    {
        gameManager.canPause = true;
        FindObjectOfType<SoundManager>().UnpauseMusic();
        MiniGameLoader.EndGame(isVictorius);
    }

    public void StartCountdown()
    {
        StartCoroutine(Count());
    }

    IEnumerator Count()
    {
        gameManager.canPause = false;

        menuPannel.SetActive(false);
        gamePannel.SetActive(true);

        winConditionText.enabled = true;
        countdownText.enabled = true;

        MinigameSoundManager.GetInstance().StartMusic(DifficultyCurve.day);

        switch (winCondition)
        {
            case WinCondition.NONE:
                winConditionText.text = "";
                break;

            case WinCondition.COLLECT_GOLD:
                winConditionText.text = "Get Rich Quick!";
                break;

            case WinCondition.TRAVEL_DISTANCE:
                winConditionText.text = "Travel 600 Meters!";
                break;

            case WinCondition.SURVIVE:
                winConditionText.text = "Survive 300 Meters!";
                break;
        }

        for (int i = 0; i < 4; i++)
        {
            yield return new WaitForSeconds(1f);

            while (gameManager.isPaused)
                yield return null;

            count--;

            if (count <= 0)
                countdownText.text = "GO";
            else
                countdownText.text = count.ToString();
        }

        Destroy(countdownText);
        Destroy(winConditionText);
        player.AllowMovements();
        gameManager.canPause = true;

        yield break;
    }

    public void Pause()
    {
        if (gameManager.canPause)
        {
            if (!gameManager.isPaused)
            {
                if (player.isMoving)
                {
                    player.DisableMovements();
                    wasMoving = true;
                }
                else
                {
                    wasMoving = false;
                }

                gamePannel.SetActive(!gamePannel.activeInHierarchy);
                MinigameSoundManager.GetInstance().GetComponent<AudioSource>().Pause();
            }
            else
            {
                if (wasMoving)
                {
                    player.AllowMovements();
                }

                MinigameSoundManager.GetInstance().GetComponent<AudioSource>().Play();
                gamePannel.SetActive(true);
            }
        }
    }

    void Victory()
    {
        isVictorius = true;
        gamePannel.SetActive(false);
        winPannel.SetActive(true);
        MinigameSoundManager.GetInstance().StopSongSmoothly();
        gameManager.canPause = false;
    }
}
