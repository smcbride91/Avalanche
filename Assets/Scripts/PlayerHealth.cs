using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public bool isAlive = true;

    // Call this method when the player dies
    public void Die()
    {
        isAlive = false;
    }

    public bool IsAlive()
    {
        return isAlive;
    }
}
