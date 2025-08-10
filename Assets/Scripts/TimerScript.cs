using UnityEngine;
using UnityEngine.Rendering;

public class TimerScript : MonoBehaviour
{
    public TMPro.TextMeshProUGUI timerText;
    private float timer;
    private float timerMax;

    private WinState winState;

    bool rewinding;

    public float getTime() { return timer; }
    public void startRewinding() { rewinding = true; }
    public void stopRewinding() { rewinding = false; }


    public float currentTime() { return timer; }

    public void rewindTimer(float t_newTime) 
    { 
        timer = t_newTime;
    }

    public void win()
    {
        timerText.text = "Game Won";
    }

    public void lose()
    {
        timerText.text = "Game Lost";
    }

    public void start()
    {
        timerText.text = timerText.text = "Timer: " + timer.ToString("0.00");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = timerMax;
    }

    // Update is called once per frame
    void Update()
    {
        if (!rewinding)
        {
            if (timer < 0 || timerText.text == "Game Won" || timerText.text == "Game Lost")
            {
                return;
            }
            timer -= Time.deltaTime;
        }
        timerText.text = "Timer: " + timer.ToString("0.00");
    }

    public void assignTime(float t_max)
    {
        timerMax = t_max;
        timer = timerMax;
    }
}
