using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Link : MonoBehaviour {

    GameObject myparent;
	// Use this for initialization
	public void Init (GameObject parent)
    {
        myparent = parent;
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (myparent !=null)
        {
            this.transform.position = myparent.transform.position;
        }
	}
}
