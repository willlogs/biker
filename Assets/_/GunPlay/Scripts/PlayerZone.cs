using PT.Bike;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        try
        {
            other.GetComponent<PlayerBikeController>().ActivateShootingMode();
        }
        catch { }
    }

    private void OnTriggerExit(Collider other)
    {
        try
        {
            other.GetComponent<PlayerBikeController>().DeactivateShootingMode();
        }
        catch { }
    }
}
