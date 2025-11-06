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
        player.Launch();
    }

    public void FillUp()
    {
        player.FillUp();
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

    public void HolsterIn()
    {
        player.HolsterInSound();
    }

    public void HolsterOut()
    {
        player.HolsterOutSound();
    }
}
