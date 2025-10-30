using System.Collections.Generic;
using UnityEngine;

public class CoroutineCasher : MonoBehaviour
{
    static Dictionary<float, WaitForSeconds> casher = new Dictionary<float, WaitForSeconds>();

    public static WaitForSeconds Wait(float time)
    {
        if(casher.ContainsKey(time))
        {
            return casher[time];
        }
        casher[time] = new WaitForSeconds(time);
        return casher[time];
    }
}
