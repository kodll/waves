using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour {

    // Use this for initialization
    public Rigidbody2D body;

	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (MainScript.GetInstance().LevelLoaded)
        {
            MainScript.GetInstance().MoveToDirection(Time.deltaTime, body, MainScript.GetInstance().PlayerShipInstance.gameObject, 100, 10, false, 3);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "PlayerShip")
        {
            MainScript.GetInstance().GuiInstance.AddCoins(10);
            Destroy(this.gameObject);
        }
    }
}
