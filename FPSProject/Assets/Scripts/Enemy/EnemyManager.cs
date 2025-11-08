using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] GameObject barricade;
    [SerializeField] EnemySpawner[] spawners;
    [SerializeField] GameObject subject;
    [SerializeField] Text enemyCount;

    [SerializeField] int allActiveEnemy;

    private void Awake()
    {
        barricade.SetActive(true);
        spawners = GetComponentsInChildren<EnemySpawner>();
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
        if(AllEnemiesCleared())
        {
            barricade.SetActive(false);
            subject.SetActive(false);
        }
    }

    bool AllEnemiesCleared()
    {
        foreach(var spawner in spawners)
        {
            if(spawner.activeEnemyCount > 0)
            {
                return false;
            }
        }
        return true;
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
