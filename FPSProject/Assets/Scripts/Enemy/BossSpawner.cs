using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    [SerializeField] Minion enemy;
    [SerializeField] Magazine magazine;
    [SerializeField] List<List<Minion>> minions;
    [SerializeField] List<List<Magazine>> mags;

    [SerializeField] Transform[] spawnPoints;

    [SerializeField] int size;
    [SerializeField] int waveCount;
    public int activeEnemyCount;

    private void Awake()
    {
        spawnPoints = GetComponentsInChildren<Transform>();

        minions = new List<List<Minion>>();
        mags = new List<List<Magazine>>();
        for(int i = 0; i<waveCount; i++)
        {
            minions.Add(new List<Minion>());
            mags.Add(new List<Magazine>());
            for(int j = 0; j<size; j++)
            {
                Minion minion = Instantiate(enemy, transform);
                Magazine mag = Instantiate(magazine, transform);

                minions[i].Add(minion);
                mags[i].Add(mag);

                minions[i][j].gameObject.SetActive(false);
            }
        }
    }

    public void WaveStart()
    {
        StartCoroutine(Wave());
    }

    public void OnEnemyDead()
    {
        activeEnemyCount--;
    }

    IEnumerator Wave()
    {
        for(int i = 0; i<waveCount; i++)
        {
            for(int j = 0; j < size; j++)
            {
                int index = Random.Range(0, spawnPoints.Length);
                minions[i][j].Init(spawnPoints[index].position, spawnPoints, mags[i][j]);
                activeEnemyCount++;
            }

            yield return CoroutineCasher.Wait(8f);
        }
    }
}
