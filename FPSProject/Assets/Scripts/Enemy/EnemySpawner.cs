using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] Enemy enemy;
    [SerializeField] Magazine mag;
    [SerializeField] List<Enemy> enemies;
    [SerializeField] List<Magazine> magazines;

    [SerializeField] Transform[] patrolPoints;

    [SerializeField] int size;
    [SerializeField] public int activeEnemyCount;

    private void Awake()
    {
        patrolPoints = GetComponentsInChildren<Transform>();

        for (int i = 0; i < size; i++)
        {
            Enemy instance = Instantiate(enemy, transform);
            Magazine m = Instantiate(mag, transform);

            enemies.Add(instance);
            magazines.Add(m);

            int index = Random.Range(0, patrolPoints.Length);

            enemies[i].Init(patrolPoints[index].position, patrolPoints, m); 
        }
        activeEnemyCount = size;
    }

    public void OnEnemyDead()
    {
        activeEnemyCount--;
    }
}
