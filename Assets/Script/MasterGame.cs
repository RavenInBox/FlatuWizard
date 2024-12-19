using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MasterGame : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] Image timerImage;
    [SerializeField] float countdownTime = 600f;
    [SerializeField] Color StartColor;
    [SerializeField] Color EndColor;
    [SerializeField] UpdateMyUpdate updateMy;
    [SerializeField] Animator UIanimator;
    [SerializeField] ManagerSonido sonido;

    private float currentTime;
    private bool isCountingDown = true;
    private string currentState = "";

    void Start()
    {
        if (timerText == null || timerImage == null)
        {
            enabled = false;
            return;
        }

        currentTime = countdownTime;
        UpdateTimerText();
        UpdateTimerImage();

        sonido.Play("sg");
    }

    void MyUpdate()
    {
        if (isCountingDown)
        {
            currentTime -= Time.deltaTime;

            if (currentTime <= 0)
            {
                currentTime = 0;
                isCountingDown = false;
                EndGame();
            }

            UpdateTimerText();
            UpdateTimerImage();
        }
    }

    void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void UpdateTimerImage()
    {
        float progress = currentTime / countdownTime;

        Color timerColor = Color.Lerp(EndColor, StartColor, progress);

        timerImage.color = timerColor;
    }

    public void EndGame()
    {
        UIanimator.Play("Derrota");
        Invoke(nameof(StopAnimator), 3);
        updateMy.PauseAll = true;
    }

    public void Victory()
    {
        UIanimator.Play("Victoria");
        CallState("Win");
        Invoke(nameof(StopAnimator), 3);
        updateMy.PauseAll = true;
    }

    void CallState(string state)
    {
        if (currentState == state) return;
        sonido.Play(state);
        currentState = state;
    }

    void StopAnimator()
    {
        UIanimator.speed = 0;
    }
}
