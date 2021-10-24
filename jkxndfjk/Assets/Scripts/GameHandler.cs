using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class GameHandler : MonoBehaviour
{
    public TMP_Text timeText;
    public TMP_Text livesText;
    public TMP_Text levelText;

    public bool timeIsRunning = false;
    float timer;
    bool gameIsFinished;
    public bool autoStart = false;
    public bool debugEnter = false;
    public int debugLevel = 0;

    public BallController player;
    public Transform levelParent;
    public GameObject[] levelsPrefabs;
    public GameObject testLevel;
    public int level = 0;

    public int startLives = 5;
    public int actLives;

    [Header("Printer")]
    public Transform printer;
    public Transform down;
    public Transform up;
    public float speed = 2f;

    int activeLevelIndex = 0;
    IEnumerator levelLoadingCorutine;

    public UnityEvent onPrinterGoUp;
    public UnityEvent onPrinterGoDown;

    public UnityEvent onModeChange;
    [Header("player Lose Life")]
    public UnityEvent onPlayerLoseLife;
    public UnityEvent onPlayerDead;


    [Header("WinLevel")]
    public UnityEvent onStartLevel;
    public UnityEvent onWinLevel;

    public UnityEvent onPlayerWinGame;
    void Start()
    {
        printer.position = new Vector3(printer.position.x, up.position.y, printer.position.z);
        foreach (GameObject go in levelsPrefabs)
        {
            go.SetActive(false);
        }
        if (autoStart)
        {
            StartGame();
        }
    }
    public void StartGame()
    {
        SetUp(true);
    }
    void UpdateLives()
    {
        livesText.text = actLives.ToString();
    }
    void UpdateLevel()
    {
        levelText.text = level.ToString();
    }
    void SetUp(bool isFirst)
    {
        testLevel.SetActive(false);
        if (isFirst)
        {
            foreach (GameObject go in levelsPrefabs)
            {
                go.SetActive(false);
            }
        }
        if (!debugEnter)
        {
            
            level = 0;
            
        }
        else
        {
            level = debugLevel;
        }

        actLives = startLives;
        timer = 0;
        gameIsFinished = false;
        timeIsRunning = false;  
        UpdateLives();
        ReloadOrNextLevelLoad(true);
    }
    private void OnEnable()
    {
        StaticGameController.Instance.onSpikesPlayerEnter -= OnEnterSpikes;
        StaticGameController.Instance.onSpikesPlayerEnter += OnEnterSpikes;
        StaticGameController.Instance.onLevelFinished -= OnFinishLevel;
        StaticGameController.Instance.onLevelFinished += OnFinishLevel;
        StaticGameController.Instance.onStateChange -= OnModeChange;
        StaticGameController.Instance.onStateChange += OnModeChange;
        StaticGameController.Instance.onLevelStart -= OnStartLevel;
        StaticGameController.Instance.onLevelStart += OnStartLevel;
    }
    private void Update()
    {
        if (timeIsRunning && !gameIsFinished)
        {
            timer += Time.deltaTime;
            float minutes = Mathf.FloorToInt(timer / 60);
            float seconds = Mathf.FloorToInt(timer % 60);
            float microseconds = Mathf.FloorToInt((timer % 1) * 1000);

            timeText.text = minutes.ToString() + ":" + seconds.ToString() + ":" + microseconds.ToString();
        }
    }
    private void OnDisable()
    {
        StaticGameController.Instance.onSpikesPlayerEnter -= OnEnterSpikes;
        StaticGameController.Instance.onLevelFinished -= OnFinishLevel;
        StaticGameController.Instance.onStateChange -= OnModeChange;
        StaticGameController.Instance.onLevelStart -= OnStartLevel;
    }
    void OnModeChange() => onModeChange?.Invoke();
    void OnEnterSpikes()
    {
        actLives--;
        player.SpawnAfterDeath();
        UpdateLives();
        if (actLives > 0)
        {
            onPlayerLoseLife?.Invoke();
            player.gameObject.SetActive(false);
            this.CallWithDelay(() =>ReloadOrNextLevelLoad(true), 1);
        }
        else
        {
            onPlayerDead?.Invoke();
            player.gameObject.SetActive(false);
            this.CallWithDelay(() => SetUp(false), 1);
        }
    }
    void OnStartLevel()
    {
        timeIsRunning = true;
        onStartLevel?.Invoke();
    }
    void OnFinishLevel()
    {
        timeIsRunning = false;
        onWinLevel?.Invoke();
        //StaticGameController.Instance.OnLevelFinishedCall();
        ReloadOrNextLevelLoad(false);
    }
    [ContextMenu("Level")]
    public void LoadLevelTest()
    {
        ReloadOrNextLevelLoad(true);
    }
    public void ReloadOrNextLevelLoad(bool loadSpecificLevel)
    {
        if (levelLoadingCorutine != null) return;
        if (!loadSpecificLevel)
        {
            level++;
        }
        levelLoadingCorutine = UnloadLoadLevel();
        StartCoroutine(levelLoadingCorutine);
    }

    void OnFinishGame()
    {
        gameIsFinished = true;
        onPlayerWinGame?.Invoke();

        testLevel.SetActive(true);
        player.MoveTo(GameObject.FindWithTag("Respawn").transform.position); //can Return false if every obiect with this tag is disabled or 
        player.gameObject.SetActive(true);
        onPrinterGoUp?.Invoke();
        //move print plane up
        printer.position = new Vector3(printer.position.x, up.position.y, printer.position.z);


        player.CanMove = true;



        //StaticGameController.Instance.OnLevelFinishedCall();
        Debug.Log("End of levels :3 ");
    }

    
    void ForceStopLevelLoadCorutine()
    {
        if (levelLoadingCorutine != null) return;
        StopCoroutine(levelLoadingCorutine);
        levelLoadingCorutine = null;
    }
    IEnumerator UnloadLoadLevel()
    {
        player.CanMove = false;
        player.ResetPlayer();
        player.gameObject.SetActive(false);
        UpdateLevel();
        

        //move print plane down
        onPrinterGoDown?.Invoke();
        float dist = up.position.y - down.position.y;
        float t = 0;
        while (t < 1)
        {

            float height = Mathf.Lerp(0, dist, 1 - t);
            printer.position = new Vector3(printer.position.x, down.position.y + height, printer.position.z);
            t += Time.deltaTime * speed;
            yield return null;
        }
        printer.position = new Vector3(printer.position.x, down.position.y, printer.position.z);


        //unload old prefab
        foreach (GameObject go in levelsPrefabs)
        {
            go.SetActive(false);
        }

        //load new prefab
        if (levelsPrefabs.Length < (level + 1)) { OnFinishGame(); ForceStopLevelLoadCorutine();}
        else
        {
            levelsPrefabs[level].SetActive(true);
            activeLevelIndex = level;

            player.MoveTo(GameObject.FindWithTag("Respawn").transform.position); //can Return false if every obiect with this tag is disabled or 
            player.gameObject.SetActive(true);


            onPrinterGoUp?.Invoke();
            //move print plane up
            t = 0;
            while (t < 1)
            {

                float height = Mathf.Lerp(0, dist, t);
                printer.position = new Vector3(printer.position.x, down.position.y + height, printer.position.z);
                t += Time.deltaTime * speed;
                yield return null;
            }
            printer.position = new Vector3(printer.position.x, down.position.y + dist, printer.position.z);

        }
        player.CanMove = true;
        levelLoadingCorutine = null;
        StaticGameController.Instance.OnLevelStart();
        
    }
    
}
