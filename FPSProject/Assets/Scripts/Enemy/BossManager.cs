using UnityEngine;

public class BossManager : MonoBehaviour
{
    [SerializeField] Trigger[] triggers;
    [SerializeField] Boss boss;
    [SerializeField] GameObject bossHPBar;

    [SerializeField] public bool engage;
    [SerializeField] public bool isCleared;

    private void Awake()
    {
        triggers = GetComponentsInChildren<Trigger>();
        isCleared = false;
        engage = false;
    }

    private void FixedUpdate()
    {
        if(boss.isDead)
        {
            isCleared = true;
            return;
        }
        IsPlayerEntered(out Transform player);
        if(engage)
        {
            boss.target = player;
            bossHPBar.SetActive(true);
        }
    }

    void IsPlayerEntered(out Transform player)
    {
        foreach(var t in triggers)
        {
            if(t.entered)
            {
                player = t.player;
                engage = true;
                return;
            }
        }
        player = null;
        engage = false;
    }

}
