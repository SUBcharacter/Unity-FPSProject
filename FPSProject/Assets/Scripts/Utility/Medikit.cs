using System.Collections.Generic;
using UnityEngine;

public class Medikit : MonoBehaviour
{
    [SerializeField] GameObject FAK;
    [SerializeField] List<GameObject> pools;

    [SerializeField] int size;
    [SerializeField] int index;
    private void Awake()
    {
        index = 0;
        for(int i = 0; i < size; i++)
        {
            GameObject kit = Instantiate(FAK, transform);
            pools.Add(kit);
            pools[i].SetActive(false);
        }
    }

    public void Get(Vector3 pos)
    {
        pools[index].GetComponent<FAK>().Init(pos);
        index = (index + 1) % pools.Count;
    }
}
