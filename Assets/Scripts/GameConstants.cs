using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConstants
{
    // Time constants
    public static float playerRotateTime = 0.25f;
    public static float aimAppearTime = 0.5f;

    // Enemy Constants
    public static float turretRange = 15f;
    public static float turretReload = 5f;

    // Bullet Constants
    public static int bulletPoolSize = 200;
    public static int shotBulletPoolSize = 200;
    public static int grenadeBulletPoolSize = 200;


    //  Value Constants
    public static float aimEndScale = 5f;
    public static float aimThreshold = 1f;
    public static float cameraLeadConstant = 0.3f;
    public static float cameraDistance = -15f;
    public enum GunTypes
    {
        Machine,
        Shot,
        Grenade
    }

    public enum EnemyTypes
    {

    }
}
