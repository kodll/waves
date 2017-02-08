using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    [HideInInspector] public int EnemyId;

    public float speedlimit;
    public float acceleration;
    public float lives;
    public bool allowrotation;
    public bool triggerexplosion;
    public float explosiontime;
    public float explosiondamage;
    [HideInInspector] bool istriggered = false;
    [HideInInspector] bool autodestruct = false;
    [HideInInspector] float triggertime = 0;

    public Animator animcontainer;

    public GameObject explosion;
    public GameObject triggeredexplosion;
    public AudioClip[] explosionSoundField;

    //[HideInInspector] public bool IsTargeted = false;
	
	// Update is called once per frame
	void Update ()
    {
        GameObject inst;
		if (lives <=0)
        {
            inst = Instantiate(explosion, this.transform.position, Quaternion.identity) as GameObject;

            MainScript.GetInstance().PlayRandomSound(explosionSoundField, this.transform.position);

            Destroy(inst.gameObject, 1);
            Destroy(this.gameObject);
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

    public void SetId(int i)
    {
        EnemyId = i;
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
    private void OnTriggerStay2D(Collider2D other)
    {
        if (autodestruct)
        {
            if (other.gameObject.tag == "EnemyShip")
            {
                other.gameObject.GetComponent<Enemy>().lives -= explosiondamage * Time.deltaTime;
            }
        }
    }
}
