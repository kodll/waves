using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public float acceleration;
    public float speedlimit;
    public float initspeed;
    public float damage;
    public GameObject wallexplosion;
    float projectilelimitdeg;
    Rigidbody2D projectilebody;
    int nearestenemyID;
    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        int id;
        float delta;
        delta = Time.deltaTime;

        if (nearestenemyID != -1)
        {
            if (MainScript.GetInstance().EnemyShipField[nearestenemyID].enemyinstance != null)
            {
                MainScript.GetInstance().MoveToDirection(delta, projectilebody, MainScript.GetInstance().EnemyShipField[nearestenemyID].enemyinstance.gameObject, acceleration, speedlimit, true, Mathf.Infinity);
            }
            else
            {
                nearestenemyID = -1;
            }
        }
        
        if (nearestenemyID == -1)
        {

            MainScript.GetInstance().MoveToDirection(delta, projectilebody, null, acceleration, speedlimit, true, Mathf.Infinity);
        }

    }

    public void InitProjectile(int enemyID, float limitdeg)
    {
        projectilebody = this.gameObject.GetComponent<Rigidbody2D>();
        nearestenemyID = enemyID;
        projectilelimitdeg = limitdeg;
        projectilebody.velocity = this.transform.right * initspeed;
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        GameObject inst;

        if (coll.gameObject.tag == "EnemyShip")
        {
            coll.gameObject.GetComponent<Enemy>().SetDamage(damage);
            inst = Instantiate(wallexplosion, this.transform.position, Quaternion.identity) as GameObject;
            Destroy(inst.gameObject, 1);
            Destroy(this.gameObject);
        }
        else
        {

            inst = Instantiate(wallexplosion, this.transform.position, Quaternion.identity) as GameObject;

            //MainScript.GetInstance().PlayRandomSound(explosionSoundField, this.transform.position);

            Destroy(inst.gameObject, 1);

            Destroy(this.gameObject);
        }
        
        
    }
}
