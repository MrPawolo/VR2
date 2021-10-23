using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    public GameObject[] levelsPrefabs;
    public int level;

    public int startLives = 5;
    public int actLives;

    void Start()
    {
        
    }
    void SetUp()
    {
        actLives = startLives;
        level = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadNextLevelCall()
    {

    }

    IEnumerator UnloadLoadLevel()
    {
        yield return null;
    }

}
