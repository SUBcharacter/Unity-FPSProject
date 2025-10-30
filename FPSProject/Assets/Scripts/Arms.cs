using UnityEngine;

public class Arms : MonoBehaviour
{
    [SerializeField] LocalPlayer player;

    private void Awake()
    {
        player = GetComponentInParent<LocalPlayer>();
    }

    public void Fire()
    {

    }

    public void Reload()
    {
        player.Reloading();
    }

    public void MuzzleFlashOn()
    {
        player.MuzzleFlashOn();
    }

    public void MuzzleFlashOff()
    {
        player.MuzzleFlashOff();
    }
}
