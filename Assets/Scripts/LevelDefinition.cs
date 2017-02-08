using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDefinition : MonoBehaviour {

    [System.Serializable]
    public struct WaveStruct
    {
        public float StartTime;
        public float Duration;
        public bool[] ActivatedNest;
    }

    public WaveStruct[] WaveField;

    public struct NestStruct
    {
        public GameObject nestinstance;
        public Nest script;
    }
    int maxnests;
    [HideInInspector] public NestStruct[] NestField;

    // Use this for initialization
    void Start ()
    {
        MainScript.GetInstance();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void InitMap()
    {
        int i;

        maxnests = 10;
        NestField = new NestStruct[maxnests];

        for (i = 0; i < maxnests; i++)
        {
            NestField[i].nestinstance = null;
        }

        Nest[] myItems = FindObjectsOfType(typeof(Nest)) as Nest[];

        foreach (Nest item in myItems)
        {
            NestField[item.id].nestinstance = item.gameObject;
            NestField[item.id].script = item;
            NestField[item.id].script.ActiveNest = false;
           
            Debug.Log("Nest Found:" + item.gameObject);
        }
    }
}
