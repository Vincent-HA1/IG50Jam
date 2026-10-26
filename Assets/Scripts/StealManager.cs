
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StealManager : Minigame
{
    [Header("UI References")]
    [SerializeField] private RectTransform bar;
    [SerializeField] private RectTransform marker;
    [SerializeField] private RectTransform targetZone;
    [SerializeField] private TMP_Text statusText;

    [Header("Movement Settings")]
    [SerializeField] private float speed = 400f;

    [Header("Target Settings")]
    [SerializeField] private float targetWidth = 80f;

    [Header("Optional Difficulty")]
    [SerializeField] private bool increaseDifficulty = true;
    [SerializeField] private float speedIncrease = 50f;
    [SerializeField] private float targetWidthDecrease = 10f;
    [SerializeField] private int maxRounds = 3;

    private float markerX;
    private float direction = 1f;
    private float targetCenterX;

    private float currentSpeed;
    private float currentTargetWidth;
    private int currentRound;

    private bool playing;
    private bool finished;

    private float BarHalfWidth =>
        bar.rect.width / 2f;

    private float MarkerHalfWidth =>
        marker.rect.width / 2f;

    private void Start()
    {
        StartMinigame();
    }

    private void Update()
    {
        if (!playing || finished)
            return;

        if (!playing)
            return;

        MoveMarker();

        if (Input.GetKeyDown(KeyCode.Space) ||
            Input.GetMouseButtonDown(0))
        {
            TryHit();
        }
    }

    private void MoveMarker()
    {
        float limit = BarHalfWidth - MarkerHalfWidth;

        markerX += direction * currentSpeed * Time.deltaTime;

        // Bounce off the ends of the bar.
        if (markerX >= limit)
        {
            markerX = limit;
            direction = -1f;
        }
        else if (markerX <= -limit)
        {
            markerX = -limit;
            direction = 1f;
        }

        marker.anchoredPosition = new Vector2(
            markerX,
            marker.anchoredPosition.y
        );
    }

    public override void StartMinigame()
    {
        if (bar == null || marker == null || targetZone == null)
        {
            Debug.LogError(
                "BouncingBarMinigame: Assign the Bar, Marker and Target Zone."
            );
            return;
        }

        playing = true;
        finished = false;

        currentRound = 1;
        currentSpeed = speed;
        currentTargetWidth = targetWidth;

        SetupRound();
    }

    private void SetupRound()
    {
        direction = Random.value < 0.5f ? -1f : 1f;

        float limit = BarHalfWidth - MarkerHalfWidth;

        // Start at the appropriate end for the direction.
        markerX = direction > 0f ? -limit : limit;

        marker.anchoredPosition = new Vector2(
            markerX,
            marker.anchoredPosition.y
        );

        // Keep the target entirely inside the bar.
        float maxCenter = Mathf.Max(
            0f,
            BarHalfWidth - currentTargetWidth / 2f
        );

        targetCenterX = Random.Range(-maxCenter, maxCenter);

        targetZone.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal,
            currentTargetWidth
        );

        targetZone.anchoredPosition = new Vector2(
            targetCenterX,
            targetZone.anchoredPosition.y
        );


        if (statusText != null)
        {
            statusText.text =
                $"Round {currentRound}/{maxRounds} - Press SPACE in the target!";
        }
    }

    private void TryHit()
    {
        float distance = Mathf.Abs(markerX - targetCenterX);

        if (distance <= currentTargetWidth / 2f)
        {
            // Successful hit.
            if (increaseDifficulty && currentRound < maxRounds)
            {
                currentRound++;

                currentSpeed += speedIncrease;

                currentTargetWidth = Mathf.Max(
                    20f,
                    currentTargetWidth - targetWidthDecrease
                );

                SetupRound();
            }
            else
            {
                EndMinigame(true);
            }
        }
        else
        {
            // Pressed outside the target.
            EndMinigame(false);
        }
    }

    private void EndMinigame(bool success)
    {
        playing = false;
        finished = true;

        if (statusText != null)
        {
            statusText.text = success
                ? "SUCCESS!"
                : "FAILED! TRY AGAIN";
        }

        if (success)
        {
            MinigameCleared?.Invoke();
        }
        else
        {
            MinigameFailed?.Invoke();
        }

    }

}
