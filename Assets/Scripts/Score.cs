using UnityEngine;

public class Score : MonoBehaviour
{
    public int deadEnemies;
    public TimerScript timerRef;
    public TMPro.TextMeshProUGUI scoreText;

    private int score;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        deadEnemies = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (deadEnemies > 0)
        {
            //print("Dead: " + deadEnemies.ToString());
        }
    }

    public void incrementScore(int t_amount)
    {
        deadEnemies++;
        score += (int)timerRef.getTime() * t_amount;
        scoreText.text = "Score: " + score.ToString();
    }
}
