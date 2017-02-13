using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour {

    Vector2 joypad;
    [HideInInspector] public bool ButtonFire;
    public float Speed;
    public float SpeedLimit;
    public float RotatingSpeed;
    public float FireRate;
    public float ActualHealth;

    public AudioClip[] LaserSoundField;

    float ActualFireRate;
    public Projectile ProjectileTemplate;
    Rigidbody2D ShipBody;

    // Use this for initialization
    void Start () {
        ActualFireRate = 0;
        ShipBody = this.GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update () {
        float delta;
        delta = Time.deltaTime;
        GetControls();
        MoveShip(delta);
        WeaponUpdate(delta);

        if (MainScript.GetInstance().GuiInstance.healthbarinstance.displayedhealth <= 0)
        {
            MainScript.GetInstance().InitLevel(false);
        }
        
    }

    void WeaponUpdate(float delta)
    {
        Projectile projectile;
        int NearestEnemy;
        if (ButtonFire)
        {
            ActualFireRate = ActualFireRate + delta;
            if (ActualFireRate>FireRate)
            {
                //Generating projectile

                projectile = Instantiate(ProjectileTemplate, this.transform.position, this.transform.rotation) as Projectile;
                projectile.transform.SetParent(MainScript.GetInstance().MapInstance.transform);

                NearestEnemy = MainScript.GetInstance().GetNearestEnemyId(this.gameObject,30);
                if (NearestEnemy != -1)
                {
                    
                    //MainScript.GetInstance().EnemyShipField[NearestEnemy].enemyinstance.IsTargeted = true;
                    projectile.InitProjectile(NearestEnemy,30);
                    
                }
                else
                {
                    
                    projectile.InitProjectile(-1, 0);
                }
                ActualFireRate = 0;

                MainScript.GetInstance().PlayRandomSound(LaserSoundField, this.transform.position);

            }
        }
    }

    void MoveShip(float delta)
    {
        float atan2;

        ShipBody.AddForce(joypad * Speed * delta);
        if (ShipBody.velocity.magnitude > SpeedLimit)
        {
            ShipBody.velocity = ShipBody.velocity.normalized * SpeedLimit;
        }
        ShipBody.velocity = ShipBody.velocity * (1-delta*2);

        /*
        if (ShipBody.velocity.magnitude > 0.5)
        {
            atan2 = Mathf.Atan2(ShipBody.velocity.y, ShipBody.velocity.x);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 0f, atan2 * Mathf.Rad2Deg), RotatingSpeed * delta);
        }*/

        if (joypad.magnitude > 0)
        {
            atan2 = Mathf.Atan2(joypad.y, joypad.x);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 0f, atan2 * Mathf.Rad2Deg), RotatingSpeed * delta);
        }

    }

    public void SetDamage(float damage)
    {
        ActualHealth -= damage;
        if (ActualHealth <= 0)
        {
            ActualHealth = 0;
        }
        if (MainScript.GetInstance().GuiInstance.healthbarinstance != null)
        {
            MainScript.GetInstance().GuiInstance.healthbarinstance.SetHealth(MainScript.GetInstance().PlayerShipInstance.ActualHealth, MainScript.GetInstance().MaxHealth, true);
        }
    }

    void SetControlValues()
    {
        joypad.x = Input.GetAxis("Horizontal");
        joypad.y = Input.GetAxis("Vertical");
        if (Input.GetButtonDown("Fire1"))
        {
            ButtonFire = true;
            ActualFireRate = 100;
        }
        if (Input.GetButtonUp("Fire1"))
        {
            ButtonFire = false;
        }
    }

    void GetControls()
    {
        if (MainScript.GetInstance().LoaderInstance == null)
        {
            SetControlValues();
        }
        else if (MainScript.GetInstance().LoaderInstance.activemenu == -1)
        {
            SetControlValues();
        }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        GameObject inst;

        if (coll.gameObject.tag == "EnemyShip")
        {
            //MainScript.GetInstance().GuiInstance.healthbarinstance.SetHealth();

            SetDamage(10);

            //coll.gameObject.GetComponent<Enemy>().SetDamage(damage);
            //inst = Instantiate(wallexplosion, this.transform.position, Quaternion.identity) as GameObject;
            //Destroy(inst.gameObject, 1);
            //Destroy(this.gameObject);
        }
    }
}
