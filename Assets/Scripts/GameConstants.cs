using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConstants
{
    // Time constants
    public static float playerRotateTime = 0.25f;
    public static float hitShakeDuration = 0.5f;
    public static float stunTime = 5f;

    public static float aimAppearTime = 0.2f;
    public static float shieldAppearTime = 0.1f;
    public static float invulnerabilityDuration = 0.2f;
    public static float attackShow = 0.2f;
    public static float attackMove = 0.2f;
    public static float healthUpdate = 1f;
    public static float scoreUpdate = 0.5f;

    public static float bulletHitLife = 1.0f;
    public static float shieldHitLife = 0.2f;
    public static float explodeLifetime = 2f;


    // Enemy Constants
    public static float turretRange = 15f;
    public static float turretReload = 5f;

    internal static int gunTurretPoolSize = 10;
    internal static int shotTurretPoolSize = 10;
    internal static int dualTurretPoolSize = 5;

    internal static float gunTurretHealth = 5;
    internal static float shotTurretHealth = 5;
    internal static float dualTurretHealth = 10;


    // Bullet Constants
    public static int bulletPoolSize = 200;
    public static int shotBulletPoolSize = 200;
    public static int grenadeBulletPoolSize = 200;

    // Effect Pools
    public static int shieldEffectsPoolSize = 100;
    public static int bulletEffectsPoolSize = 100;
    public static int explosionPoolSize = 10;

    // Effect Sizes
    internal static float bulletHitSize = 2f;
    internal static float shieldHitSize = 0.5f;
    internal static float explodeSize = 1.5f;

    //  Value Constants
    public static int maxHealth = 40;
    public static float maxTime = 120;
    public static float parrySize = 7.5f;
    public static float parryForce = 500;

    public static float aimEndScale = 3f;
    public static float aimThreshold = 1f;
    public static float cameraLeadConstant = 0.3f;

    public static float cameraDistance = -20f;
    public static float timeSlowDown = 0.1f;

    public static float attackHealthGain = 10f;
    public static float attackTimeGain = 30f;
    public static float waveCompleteTimeGain = 50f;

    // Score
    public static string enemiesPrefix = "Enemies: ";
    public static string scorePrefix = "Score: ";
    public static int successAttackScore = 25;
    public static int enemyKillScore = 10;
    public static int clearSceneScore = 100;

    public static int EnemyBaseCount = 10;
    public static string HighScorePlayerPref = "HighScore";

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

    public enum EffectTypes
    {
        ShipExplosion,
        ShieldHit,
        BulletHit
    }

    public enum AttackDirection
    {
        Up,
        Down,
        Left,
        Right,
        None
    }
}
