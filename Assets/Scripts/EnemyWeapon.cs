using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour {

    public Projectile ProjectileTemplate;
    public GameObject[] weaponsystem;
    public float[] firedelay;
    bool[] fired;
    public float frequency;
    public float maxdistance;
    float actualtime = 0;

    void Start()
    {
        fired = new bool[weaponsystem.Length];
        ResetFireSystem();
        actualtime = -1;
    }

    void ResetFireSystem()
    {
        int i;

        actualtime = 0;

        for (i = 0; i < weaponsystem.Length; i++)
        {
            fired[i] = false;
        }
    }

    // Update is called once per frame
    void Update ()
    {
        Projectile projectile;
        int i;
        Vector3 distvect;

        if (MainScript.GetInstance().LevelLoaded)
        {
            actualtime += Time.deltaTime;
            for (i = 0; i < weaponsystem.Length; i++)
            {
                if (actualtime >= firedelay[i] && !fired[i])
                {
                    fired[i] = true;

                    distvect = this.transform.position - MainScript.GetInstance().PlayerShipInstance.transform.position;

                    if (distvect.magnitude <= maxdistance)
                    {
                        projectile = Instantiate(ProjectileTemplate, weaponsystem[i].transform.position, weaponsystem[i].transform.rotation) as Projectile;
                        projectile.transform.SetParent(MainScript.GetInstance().MapInstance.transform);
                        projectile.InitProjectile(-1, 0);
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
