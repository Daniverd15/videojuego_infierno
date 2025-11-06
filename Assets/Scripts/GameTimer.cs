using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameTimer : MonoBehaviour
{
    [Header("Timer")]
    public float startingTime = 30f;                 // Segundos iniciales
    public TextMeshProUGUI timerText;                // Referencia al texto TMP en Canvas

    [Header("Advertencia visual/sonora")]
    public int warningThreshold = 5;                 // Últimos segundos con aviso
    public Color normalColor = Color.white;
    public Color warningColor = Color.red;
    public float warningPulseScale = 1.15f;          // Efecto de pulso
    public float warningPulseSpeed = 6f;

    [Header("Sonidos (opcional)")]
    public AudioSource audioSource;                  // Arrastra un AudioSource
    public AudioClip beepClip;                       // Sonido de beep
    public AudioClip timeUpClip;                     // Sonido al terminar

    [Header("Acción al terminar")]
    public string sceneToLoadOnTimeUp = "";          // Deja vacío para recargar actual
    public bool quitOnTimeUp = false;                // Terminar aplicación al llegar a 0

    float timeRemaining;
    bool running;
    int lastSecondShown = -1;
    Vector3 baseScale;

    void Start()
    {
        timeRemaining = startingTime;
        running = true;
        baseScale = timerText ? timerText.rectTransform.localScale : Vector3.one;
        UpdateUI(force: true);
    }

    void Update()
    {
        if (!running) return;

        timeRemaining -= Time.deltaTime;
        if (timeRemaining < 0f) timeRemaining = 0f;

        UpdateUI();

        // Fin del tiempo
        if (timeRemaining <= 0f)
        {
            running = false;

            // Sonido final (opcional)
            if (audioSource && timeUpClip) audioSource.PlayOneShot(timeUpClip);

            if (quitOnTimeUp)
            {
                Application.Quit();
            }
            else if (!string.IsNullOrEmpty(sceneToLoadOnTimeUp))
            {
                SceneManager.LoadScene(sceneToLoadOnTimeUp);
            }
            else
            {
                // Por defecto, recarga la escena actual
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void UpdateUI(bool force = false)
    {
        if (!timerText) return;

        int totalSeconds = Mathf.CeilToInt(timeRemaining);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        if (force || totalSeconds != lastSecondShown)
        {
            timerText.text = $"{minutes:00}:{seconds:00}";
            lastSecondShown = totalSeconds;

            // Beep en los últimos segundos
            if (totalSeconds > 0 && totalSeconds <= warningThreshold)
            {
                if (audioSource && beepClip) audioSource.PlayOneShot(beepClip);
            }
        }

        // Cambiar color y efecto visual
        if (totalSeconds <= warningThreshold)
        {
            timerText.color = warningColor;

            float scale = Mathf.Lerp(1f, warningPulseScale,
                (Mathf.Sin(Time.time * warningPulseSpeed) + 1f) * 0.5f);
            timerText.rectTransform.localScale = baseScale * scale;
        }
        else
        {
            timerText.color = normalColor;
            timerText.rectTransform.localScale = baseScale;
        }
    }

    // === Métodos auxiliares ===
    public void PauseTimer(bool pause) => running = !pause;

    public void AddTime(float extra)
    {
        timeRemaining = Mathf.Max(0f, timeRemaining + extra);
        UpdateUI(force: true);
    }

    public void ResetTimer()
    {
        timeRemaining = startingTime;
        running = true;
        lastSecondShown = -1;
        UpdateUI(force: true);
    }
}
