using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public Score scoreRef;
    public TimerScript timerRef;
    public Player playerRef;
    public LevelData currentLevel;

    private LevelData[] levels;
    private int currentLevelIndex;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Game Start");
        currentLevelIndex = 0;
        levels = GetComponentsInChildren<LevelData>();
        levelTransition();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if level is done
        if (currentLevel.isWon())
        {
            print("Level Won");
            timerRef.win();
            nextLevel();
        }
        else if (currentLevel.isOver() && !currentLevel.isWon())
        {
            print("Level Lost");
            timerRef.lose();
            playerRef.die();
        }
    }

    private void nextLevel()
    {
        if (currentLevelIndex < levels.Length)
        { 
            currentLevel = levels[currentLevelIndex];
            currentLevelIndex++;
            levelTransition();
        }
        else
        {
            print("End of Levels");
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene(0);
        }
    }

    private void levelTransition()
    {
        SpawnPosition pos = currentLevel.GetComponentInChildren<SpawnPosition>();
        playerRef.spawn(pos.getPosition());
        timerRef.assignTime(currentLevel.timeValue());
        currentLevel.start();
        timerRef.start();
    }
}
