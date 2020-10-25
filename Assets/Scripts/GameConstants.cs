using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConstants
{
    // Time constants
    public static float playerRotateTime = 0.25f;
    public static float aimAppearTime = 0.5f;
    public static float shieldAppearTime = 0.15f;
    public static float invulnerabilityDuration = 0.2f;

    // Enemy Constants
    public static float turretRange = 15f;
    public static float turretReload = 5f;

    internal static int gunTurretPoolSize = 10;
    internal static int shotTurretPoolSize = 10;
    internal static int dualTurretPoolSize = 5;

    // Bullet Constants
    public static int bulletPoolSize = 200;
    public static int shotBulletPoolSize = 200;
    public static int grenadeBulletPoolSize = 200;

    //  Value Constants
    public static int maxHealth = 100;
    public static float parrySize = 7.5f;
    public static float parryForce = 500;

    public static float aimEndScale = 3f;
    public static float aimThreshold = 1f;
    public static float cameraLeadConstant = 0.3f;

    public static float cameraDistance = -20f;
    internal static float timeSlowDown = 0.1f;

    public enum GunTypes
    {
        Machine,
        Shot,
        Grenade
    }

    public enum EnemyTypes
    {
        GunTurret,
        ShotTurret,
        DualTurret
    }
}
