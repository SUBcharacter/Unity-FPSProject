using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [SerializeField] GameObject pauseScreen;
    [SerializeField] Slider sensitivity;
    [SerializeField] Text sensitivityText;
    [SerializeField] LocalPlayer player;

    public static bool IsPaused { get; private set; }

    bool isPaused = false;

    private void Awake()
    {
        pauseScreen.SetActive(false);
        sensitivity.onValueChanged.AddListener(SenseChange);
        sensitivity.value = player.mouseSensitivity;
        sensitivityText.text = player.mouseSensitivity.ToString();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        Time.timeScale = 0;
        pauseScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isPaused = true;
        IsPaused = isPaused;

    }

    public void Resume()
    {
        pauseScreen.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isPaused = false;
        IsPaused = isPaused;
        Time.timeScale = 1f;
    }

    public void ToMain()
    {
        Time.timeScale = 1f;
        isPaused = false;
        IsPaused = isPaused;
        SceneManager.LoadScene("Title");
    }

    public void SenseChange(float value)
    {
        player.SetSensitivity(value);
        sensitivityText.text = value.ToString();
    }
}
