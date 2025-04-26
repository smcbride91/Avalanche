using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticResetter
{
    public static void ResetStaticFlags()
    {
        DetectCollision.ResetDeathFlags();
        GamePauseManager.ResetGameOver();
    }
}
