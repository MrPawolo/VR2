using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameHandler : MonoBehaviour
{
    public BallController player;
    public Transform levelParent;
    public GameObject[] levelsPrefabs;
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



    public UnityEvent onModeChange;
    [Header("player Lose Life")]
    public UnityEvent onPlayerLoseLife;
    public UnityEvent onPlayerDead;


    [Header("WinLevel")]
    public UnityEvent onWinLevel;
    void Start()
    {
        SetUp(true);
        
        printer.position = new Vector3(printer.position.x, up.position.y, printer.position.z);
    }
    void SetUp(bool isFirst)
    {
        if (isFirst)
        {
            foreach (GameObject go in levelsPrefabs)
            {
                go.SetActive(false);
            }
        }
        actLives = startLives;
        level = 0;
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
    }
    private void OnDisable()
    {
        StaticGameController.Instance.onSpikesPlayerEnter -= OnEnterSpikes;
        StaticGameController.Instance.onLevelFinished -= OnFinishLevel;
        StaticGameController.Instance.onStateChange -= OnModeChange;
    }
    void OnModeChange() => onModeChange?.Invoke();
    void OnEnterSpikes()
    {
        actLives--;
        player.SpawnAfterDeath();
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
    void OnFinishLevel()
    {
        onWinLevel?.Invoke();
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

        //move print plane down
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

            player.CanMove = true;
        }
        levelLoadingCorutine = null;
    }
    
}
