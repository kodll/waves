using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour {

    public GameObject healthslider;
    [HideInInspector] public float actualhealth = 1;
    [HideInInspector] public float displayedhealth = 1;

	// Update is called once per frame
	void Update ()
    {
        if (displayedhealth > actualhealth)
        {
            displayedhealth -= Time.deltaTime;
        }
        if (displayedhealth< actualhealth)
        {
            displayedhealth = actualhealth;
        }
                
        Vector3 scale;
        scale = Vector3.one;
        scale.x = displayedhealth;
        healthslider.transform.localScale = scale;
        this.transform.rotation = Quaternion.identity;
	}

    public void SetHealth(float actual, float max, bool interpolation)
    {
        if (interpolation)
        {
            actualhealth = actual / max;
        }
        else
        {
            actualhealth = actual / max;
            displayedhealth = actual / max;
        }
    }
}
