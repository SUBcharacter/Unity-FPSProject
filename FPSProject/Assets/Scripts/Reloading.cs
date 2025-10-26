using UnityEngine;

public class Reloading : MonoBehaviour
{
    [SerializeField] LocalPlayer player;

    private void Awake()
    {
        player = GetComponentInParent<LocalPlayer>();
    }

    public void Reload()
    {
        player.Reloading();
    }
}
