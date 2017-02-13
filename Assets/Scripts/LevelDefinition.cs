using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public GameObject LevelStart;

    // Use this for initialization
    void Start ()
    {
        Loader test;
        MainScript.GetInstance();

        //test = MainScript.GetInstance().LoaderInstance;
        test = GameObject.FindObjectOfType(typeof(Loader)) as Loader;
        if (test==null)
        {
            StartCoroutine(LoadMenu());
        }  
    }
    public IEnumerator LoadMenu()
    {
        AsyncOperation async;

        async = SceneManager.LoadSceneAsync("Scenes/MainScene", LoadSceneMode.Additive);

        async.allowSceneActivation = true;

        while (!async.isDone)
        {
            yield return null;
        }

        MainScript.GetInstance().LoaderInstance = GameObject.FindObjectOfType(typeof(Loader)) as Loader;
        Debug.Log("Loader found: " + MainScript.GetInstance().LoaderInstance);
        MainScript.GetInstance().LoaderInstance.InitMenu();
        MainScript.GetInstance().LoaderInstance.CloseMenu(0);
        MainScript.GetInstance().LoaderInstance.activelevel = "Scenes/" + SceneManager.GetActiveScene().name;
        MainScript.GetInstance().LoaderInstance.LoaderCamera.transform.SetParent(Camera.main.transform);
        MainScript.GetInstance().LoaderInstance.LoaderCamera.transform.localPosition = Vector3.zero;
        MainScript.GetInstance().transform.SetParent(MainScript.GetInstance().LoaderInstance.transform);
        MainScript.GetInstance().InitLevel(true);
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
