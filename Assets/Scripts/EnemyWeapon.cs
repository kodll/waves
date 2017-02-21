using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponTypeEnum
{
    bullet, laser
}

public class EnemyWeapon : MonoBehaviour {

    [System.Serializable]
    public struct WeaponStruct
    {
        public WeaponTypeEnum WeaponType;
        public Projectile ProjectileTemplate;
        public Laser LaserTemplate;
        public GameObject WeaponPosition;
        public float FireDelay;
        public float FireDuration;
    }

    public WeaponStruct[] Weapons;

    bool[] fired;
    public float frequency;
    public float maxdistance;
    float actualtime = 0;

    void Start()
    {
        fired = new bool[Weapons.Length];
        ResetFireSystem();
        actualtime = -1;
    }

    void ResetFireSystem()
    {
        int i;

        actualtime = 0;

        for (i = 0; i < Weapons.Length; i++)
        {
            fired[i] = false;
        }
    }

    // Update is called once per frame
    void Update ()
    {
        Projectile projectile;
        Laser laser;
        int i;
        Vector3 distvect;

        if (MainScript.GetInstance().LevelLoaded)
        {
            actualtime += Time.deltaTime;
            for (i = 0; i < Weapons.Length; i++)
            {
                if (actualtime >= Weapons[i].FireDelay && !fired[i])
                {
                    fired[i] = true;

                    /*if (Weapons[i].WeaponType == WeaponTypeEnum.bullet)
                    {
                        fired[i] = true;
                    }
                    if (Weapons[i].WeaponType == WeaponTypeEnum.laser && actualtime >= Weapons[i].FireDelay + Weapons[i].FireDuration)
                    {
                        fired[i] = true;
                    }*/

                    distvect = this.transform.position - MainScript.GetInstance().PlayerShipInstance.transform.position;

                    if (distvect.magnitude <= maxdistance)
                    {
                        if (Weapons[i].WeaponType == WeaponTypeEnum.bullet)
                        {
                            projectile = Instantiate(Weapons[i].ProjectileTemplate, Weapons[i].WeaponPosition.transform.position, Weapons[i].WeaponPosition.transform.rotation) as Projectile;
                            projectile.transform.SetParent(MainScript.GetInstance().MapInstance.transform);
                            projectile.InitProjectile(-1, 0);
                        }
                        else if (Weapons[i].WeaponType == WeaponTypeEnum.laser)
                        {
                            laser = Instantiate(Weapons[i].LaserTemplate, Weapons[i].WeaponPosition.transform.position, Weapons[i].WeaponPosition.transform.rotation) as Laser;
                            laser.transform.SetParent(MainScript.GetInstance().MapInstance.transform);
                            laser.InitLaser(this, i, Weapons[i].FireDuration);
                        }
                    }
                }
            }
        }

        if (actualtime >= frequency)
        {
            ResetFireSystem();
        }
    }
}
