using System.Collections.Generic;
using UnityEngine;

public class Magazine : MonoBehaviour
{
    [SerializeField] GameObject bullet;
    [SerializeField] List<GameObject> magazine;

    [SerializeField] int index;
    [SerializeField] int magazineCount;

    private void Awake()
    {
        index = 0;
        for(int i =0; i< magazineCount; i++)
        {
            GameObject cartrige = Instantiate(bullet, transform);

            magazine.Add(cartrige);
            magazine[i].SetActive(false);
        }
    }

    public void Fire(Vector3 dir, Transform pos)
    {
        magazine[index].GetComponent<Bullet>().Init(dir, pos);
        index = (index + 1) % magazineCount;
    }

    public void ReturnAllBullet()
    {
        foreach(var b in magazine)
        {
            if(b.activeSelf)
            {
                b.SetActive(false);
            }
        }
    }
}
