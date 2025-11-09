using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] EnemySpawner[] spawners;
    [SerializeField] GameObject objective1;
    [SerializeField] GameObject Objective2;
    [SerializeField] Text enemyCount;

    [SerializeField] int allActiveEnemy;

    [SerializeField] public bool isCleared;

    private void Awake()
    {
        spawners = GetComponentsInChildren<EnemySpawner>();
        isCleared = false;
    }

    private void Start()
    {
        foreach(var spawner in spawners)
        {
            allActiveEnemy += spawner.activeEnemyCount;
        }
    }
    private void Update()
    {
        AliveEnemyCount();
        AllEnemiesCleared();
        if(isCleared)
        {
            objective1.SetActive(false);
            Objective2.SetActive(true);
        }
    }

    void AllEnemiesCleared()
    {
        foreach(var spawner in spawners)
        {
            if(spawner.activeEnemyCount > 0)
            {
                isCleared = false;
                return;
            }
        }
        isCleared = true;
    }

    void AliveEnemyCount()
    {
        int count = 0;
        foreach(var spawner in spawners)
        {
            count += spawner.activeEnemyCount;
        }
        allActiveEnemy = count;
        enemyCount.text = allActiveEnemy.ToString();
    }
}
