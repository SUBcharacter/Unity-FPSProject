using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] AudioSource BGMAudio;
    [SerializeField] AudioClip BGM1Clip;
    [SerializeField] GameObject barricade;
    [SerializeField] BossManager bossManager;
    [SerializeField] EnemyManager enemyManager;
    [SerializeField] LocalPlayer player;
    [SerializeField] GameObject resultScreen;
    [SerializeField] GameObject gameoverScreen;
    [SerializeField] public static GameManager instance;

    [SerializeField] public bool stopInput;


    private void Awake()
    {
        instance = this;
        stopInput = false;
        BGMAudio.clip = BGM1Clip;
        BGMAudio.loop = true;
        BGMAudio.Play();
    }

    private void Update()
    {
        if(player.isDead)
        {
            EnemySoundStop();
            BGMAudio.Stop();
            Time.timeScale = 0f;
            stopInput = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            gameoverScreen.SetActive(true);
        }

        if (bossManager.isCleared)
        {
            EnemySoundStop();
            BGMAudio.Stop();
            Time.timeScale = 0f;
            stopInput = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            resultScreen.SetActive(true);
            return;
        }

        if (enemyManager.isCleared)
        {
            if(!bossManager.engage)
            {
                barricade.SetActive(false);
            }
            else
            {
                barricade.SetActive(true);
            }
        }
    }

    void EnemySoundStop()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        Minion[] minions = FindObjectsByType<Minion>(FindObjectsSortMode.None);

        foreach(var e in enemies)
        {
            e.actAudio.audioSource.Stop();
            e.moveAudio.audioSource.Stop();
            e.shotAudio.audioSource.Stop();
        }
        foreach(var m in minions)
        {
            m.moveAudio.audioSource.Stop();
            m.actAudio.audioSource.Stop();
            m.shotAudio.audioSource.Stop();
        }
    }
}
