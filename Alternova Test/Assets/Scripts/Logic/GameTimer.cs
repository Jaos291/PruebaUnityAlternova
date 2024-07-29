using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    public Text timerText; 
    private float elapsedTime;
    private bool paused;

    void Start()
    {
        paused = true; 
        elapsedTime = 0f;
        UpdateTimerText();
    }

    void Update()
    {
        if (!paused)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerText();
        }
    }

    public void SetPaused(bool isPaused)
    {
        paused = isPaused;
    }

    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60F);
        int seconds = Mathf.FloorToInt(elapsedTime % 60F);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
