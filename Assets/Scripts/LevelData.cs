using UnityEngine;

public class LevelData : MonoBehaviour
{
    public int enemyCount; // -1 == none
    public int itemCount; // -1 == none
    private int activeEnemyCount;
    private int activeItemCount;
    public float time;
    public int enemyHealth;
    private WinState winState;

    public Game gameRef;
    public Score scoreRef;

    public bool isOver()
    {
        if (winState != WinState.ACTIVE)
        {
            return true;
        }
        return false;
    }

    public bool isWon()
    {
        if (winState == WinState.ENDWIN)
        {
            return true;
        }
        return false;
    }

    public float timeValue() { return time; }

    private void countEntities()
    {
        Enemy[] enemies = GetComponentsInChildren<Enemy>();
        Item[] items = GetComponentsInChildren<Item>();

        enemyCount = enemies.Length;
        itemCount = items.Length;

        if (enemyCount == 0)
        {
            enemyCount = -1;
        }
        if (itemCount == 0)
        {
            itemCount = -1;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scoreRef = gameRef.scoreRef;
        winState = WinState.ACTIVE;
        countEntities();
        activeEnemyCount = enemyCount;
        activeItemCount = itemCount;
    }

    // Update is called once per frame
    void Update()
    {
        gameStateCheck();
    }

    public void decrementEnemy(int t_amount)
    {
        activeEnemyCount -= t_amount;
    }

    public void decrementItem(int t_amount)
    {
        activeItemCount -= t_amount;
    }

    public void start()
    {
        winState = WinState.ACTIVE;
    }

    private void gameStateCheck()
    {
        if (activeEnemyCount == 0)
        {
            if (activeItemCount == -1)
            {
                winState = WinState.ENDWIN;
                return;
            }
        }
        if (activeItemCount == 0)
        {
            winState = WinState.ENDWIN;
            return;
        }

        if (gameRef.timerRef.currentTime() <= 0)
        {
            if (winState == WinState.ACTIVE)
            {
                winState = WinState.ENDLOSE;
            }
            return;
        }
    }
}

enum WinState { ACTIVE, ENDWIN, ENDLOSE}
