using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    [HideInInspector] public int EnemyId;

    public float speedlimit;
    public float acceleration;
    public float lives;
    [HideInInspector] float maxlives;
    public bool allowrotation;
    public bool triggerexplosion;
    public float explosiontime;
    public float explosiondamage;
    public float magnetdistance;
    [HideInInspector] bool istriggered = false;
    [HideInInspector] bool autodestruct = false;
    [HideInInspector] float triggertime = 0;

    public Animator animcontainer;

    public GameObject explosion;
    public GameObject triggeredexplosion;
    public AudioClip[] explosionSoundField;

    public HealthBar healthbarprefab;
    [HideInInspector] public HealthBar healthbarinstance = null;

    //[HideInInspector] public bool IsTargeted = false;

    // Update is called once per frame

    void Update ()
    {
        GameObject inst;

        if (lives<maxlives)
        {
            if (healthbarinstance == null)
            {
                DisplayHealthBar(true);
            }
        }

        if (triggerexplosion && istriggered)
        {
            triggertime += Time.deltaTime;

            if (triggertime>explosiontime) //autodestruct explosion
            {
                inst = Instantiate(triggeredexplosion, this.transform.position, Quaternion.identity) as GameObject;
                Destroy(inst.gameObject, 1);
                autodestruct = true;
                Destroy(this.gameObject,0.2f);
            }
        }
	}
    public void DisplayHealthBar(bool display)
    {
        Vector3 healthbarpos;

        if (display)
        {
            if (healthbarinstance == null)
            {
                healthbarpos = Vector3.zero;
                healthbarpos.y = 0.3f;
                healthbarinstance = Instantiate(healthbarprefab);
                healthbarinstance.transform.SetParent(this.transform);
                healthbarinstance.transform.localPosition = healthbarpos;
            }
        }
        else
        {
            if (healthbarinstance != null)
            {
                Destroy(healthbarinstance.gameObject);
                healthbarinstance = null;
            }
        }
    }

    public void SetDamage(float damage)
    {
        GameObject inst;

        lives -= damage;
        if (lives<=0)
        {
            lives = 0;
            inst = Instantiate(explosion, this.transform.position, Quaternion.identity) as GameObject;

            MainScript.GetInstance().PlayRandomSound(explosionSoundField, this.transform.position);

            Destroy(inst.gameObject, 1);
            Destroy(this.gameObject);
            MainScript.GetInstance().GuiInstance.AddCoins(1);
        }
        if (healthbarinstance == null)
        {
            DisplayHealthBar(true);
            healthbarinstance.SetHealth(lives, maxlives, false);
        }
        else
        {
            healthbarinstance.SetHealth(lives, maxlives, true);
        }
    }

    public void SetId(int i)
    {
        EnemyId = i;
        maxlives = lives;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "PlayerShip")
        {
            if (triggerexplosion && !istriggered)
            {
                animcontainer.SetTrigger("Enter");
                istriggered = true;
                //Debug.Log("Trigger Colider detected: " + other);
            }
        }
    }
    private void OnTriggerStay2D(Collider2D col)
    {
        if (autodestruct)
        {
            if (col.gameObject.tag == "EnemyShip")
            {
                col.gameObject.GetComponent<Enemy>().SetDamage(explosiondamage * Time.deltaTime);
            }
            else if (col.gameObject.tag == "PlayerShip")
            {
                MainScript.GetInstance().PlayerShipInstance.SetDamage(explosiondamage * Time.deltaTime);
            }
        }
    }
}
