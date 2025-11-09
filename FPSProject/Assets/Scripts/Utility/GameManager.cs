using UnityEngine;

public class GameManager : MonoBehaviour
{
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
    }

    private void Update()
    {
        if(player.isDead)
        {
            Time.timeScale = 0f;
            stopInput = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            gameoverScreen.SetActive(true);
        }

        if (bossManager.isCleared)
        {
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
}
