using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScript : MonoBehaviour {

    public struct EnemyShipStruct
    {
        public Enemy enemyinstance;
        public Rigidbody2D enemybody;
    }
    [HideInInspector] public int maxenemies;
    public EnemyShipStruct[] EnemyShipField;

    public ShipController PlayerShipPrefab;
    [HideInInspector] public ShipController PlayerShipInstance;
    [HideInInspector] public LevelDefinition MapInstance;
    [HideInInspector] public Loader LoaderInstance = null;

    float WavesTime;
    int ActiveWave;
    bool FinalWave;
    [HideInInspector] public bool LevelLoaded = false;
    public Loader LoaderPrefab;
    public GUInterface GuiPrefab;
    [HideInInspector] public GUInterface GuiInstance;

    //-------------SAVE-------------------
    public int Coins = 0;
    public float MaxHealth = 100;
    //------------------------------------

    static MainScript instance = null;
    public static MainScript GetInstance()
    {
        if (instance==null)
        {
            instance = GameObject.Instantiate(Resources.Load("MainScript", typeof(MainScript))) as MainScript;
            instance.Init();
        }
        return instance;
    }

    // Use this for initialization
    void Init()
    {
        LevelLoaded = false;
        PlayerShipInstance = Instantiate(PlayerShipPrefab, Vector3.zero, Quaternion.identity) as ShipController;
        PlayerShipInstance.gameObject.SetActive(false);
        GuiInstance = Instantiate(GuiPrefab) as GUInterface;
    }

    void Start ()
    {
        maxenemies = 200;
        EnemyShipField = new EnemyShipStruct[maxenemies];
    }

    public void InitLevel(bool init)
    {
        int i;

        for (i = 0; i < maxenemies; i++)
        {
            if (EnemyShipField[i].enemyinstance != null) Destroy(EnemyShipField[i].enemyinstance.gameObject);
            EnemyShipField[i].enemyinstance = null;
        }

        if (init)
        {
            LoaderInstance = GameObject.FindObjectOfType(typeof(Loader)) as Loader;

            MapInstance = GameObject.FindObjectOfType(typeof(LevelDefinition)) as LevelDefinition;
            MapInstance.InitMap();
            WavesTime = 0;
            ActiveWave = 0;
            FinalWave = false;

            PlayerShipInstance.transform.position = MapInstance.LevelStart.transform.position;

            PlayerShipInstance.gameObject.SetActive(true);

            GuiInstance.transform.SetParent(LoaderInstance.transform);
            PlayerShipInstance.transform.SetParent(LoaderInstance.transform);

            PlayerShipInstance.ActualHealth = MaxHealth;
            PlayerShipInstance.ButtonFire = false;

            LevelLoaded = true;
            GuiInstance.InitGui();
        }
        else
        {
            LevelLoaded = false;
            PlayerShipInstance.gameObject.SetActive(false);
            if (LoaderInstance != null)
            {
                StartCoroutine(LoaderInstance.UnLoadLevel());
            }
        }
    }

    // Update is called once per frame
    void Update ()
    {
        float delta;

        if (LevelLoaded)
        {
            delta = Time.deltaTime;
            UpdateEnemies(delta);
            SetCamera(delta);
            WaveControl(delta);

            if (Input.GetButtonDown("Exit"))
            {
                if (LoaderInstance != null)
                {
                    if (LoaderInstance.activemenu == -1) //Open Pause Menu
                    {
                        LoaderInstance.OpenMenu(1);
                        Time.timeScale = 0.0f;
                    }
                    else if (LoaderInstance.activemenu == 1) //Back to Game
                    {
                        LoaderInstance.ResumeGame();
                    }
                }
            }
        }
    }

    void WaveControl(float delta)
    {
        int i;
        if (!FinalWave)
        {
            WavesTime = WavesTime + delta;
            if (ActiveWave == MapInstance.WaveField.Length)
            {
                Debug.Log("Activating Portal");
                FinalWave = true;
            }
            else if (WavesTime > MapInstance.WaveField[ActiveWave].StartTime)
            {
                //WAVE STARTED
                Debug.Log("Incoming New Wave:" + ActiveWave);
                for (i = 0; i < MapInstance.WaveField[ActiveWave].ActivatedNest.Length; i++)
                {
                    if (MapInstance.WaveField[ActiveWave].ActivatedNest[i])
                        MapInstance.NestField[i].script.StartWave(MapInstance.WaveField[ActiveWave].Duration);
                }                      
                ActiveWave = ActiveWave + 1;

            }
        }
    }

    void SetCamera(float delta)
    {
        Camera.main.transform.parent.transform.position = PlayerShipInstance.transform.position;
    }

    public Quaternion GetDirection (Vector3 from, Vector3 to)
    {
        float atan2;
        Vector3 dir;

        dir = to - from;

        atan2 = Mathf.Atan2(dir.y, dir.x);
        return Quaternion.Euler(0f, 0f, atan2 * Mathf.Rad2Deg);
    }

    public void MoveToDirection(float delta, Rigidbody2D movingbody, GameObject finalposition, float speed, float speedlimit, bool rotatealways, float magnetdist)
    {
        float atan2;
        Vector3 dir;
        Vector3 distvect;
        float dist;
        float power;

        distvect = Vector3.zero;
        dist = -10;
        if (finalposition != null)
        {
            distvect = finalposition.transform.position - movingbody.transform.position;
            dist = distvect.magnitude;
        }

        if (dist < magnetdist || magnetdist == -1)
        {
            if (finalposition != null)
            {
                dir = finalposition.transform.position - movingbody.transform.position;
            }
            else
            {
                dir = movingbody.transform.right;
            }

            power = 1;
            if (magnetdist != -1)
            {
                power = Mathf.Cos(dist / magnetdist * Mathf.PI / 2 + 0.5f);
            }

            movingbody.AddForce(dir.normalized * speed * Time.deltaTime * 70);

            if (movingbody.velocity.magnitude > speedlimit * power)
            {
                movingbody.velocity = movingbody.velocity.normalized * speedlimit * power;
            }

            if (movingbody.velocity.magnitude > 0.5 && rotatealways)
            {
                atan2 = Mathf.Atan2(movingbody.velocity.y, movingbody.velocity.x);
                movingbody.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, atan2 * Mathf.Rad2Deg);
                //movingbody.gameObject.transform.rotation = Quaternion.Lerp(movingbody.gameObject.transform.rotation, Quaternion.Euler(0f, 0f, atan2 * Mathf.Rad2Deg), 0.1f);

            }
            if (speedlimit == 0 && rotatealways && distvect != Vector3.zero)
            {
                atan2 = Mathf.Atan2(distvect.y, distvect.x);
                movingbody.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, atan2 * Mathf.Rad2Deg);
            }

        }
        else
        {
            movingbody.velocity = Vector3.zero;
        }
    }

    void UpdateEnemies(float delta)
    {
        int i;
        for (i = 0; i< maxenemies; i++)
		{
            if (EnemyShipField[i].enemyinstance != null)
            {
                MoveToDirection(delta, EnemyShipField[i].enemybody, PlayerShipInstance.gameObject, EnemyShipField[i].enemyinstance.acceleration, EnemyShipField[i].enemyinstance.speedlimit, EnemyShipField[i].enemyinstance.allowrotation, EnemyShipField[i].enemyinstance.magnetdistance);
            }
        }
    }

    public float getAngle(GameObject from, GameObject to)
    {
        float difference;
        Vector3 v1;
        Vector3 v2;

        v1 = from.transform.right;
        v2 = to.transform.position - from.transform.position;

        difference = Vector3.Angle(v1, v2);

        return difference;
    }

    public int GetNearestEnemyId(GameObject from, float limitdeg)
    {
        Vector3 distvect;
        float dist;
        int nearestId = -1;
        float nearestdistance = Mathf.Infinity;
        int i;

        for (i = 0; i < maxenemies; i++)
        {
            if (EnemyShipField[i].enemyinstance != null/* && !EnemyShipField[i].enemyinstance.IsTargeted*/ && getAngle(from, EnemyShipField[i].enemyinstance.gameObject) < limitdeg)
            {
                distvect = EnemyShipField[i].enemyinstance.transform.position - PlayerShipInstance.transform.position;
                dist = distvect.magnitude;
                if (dist<nearestdistance)
                {
                    nearestId = i;
                    nearestdistance = dist;
                }
            }
        }
        return nearestId;
    }

    public void PlayRandomSound(AudioClip[] audiofield, Vector3 where)
    {
        int r;
        r = Random.Range(0, audiofield.Length);
        AudioSource.PlayClipAtPoint(audiofield[r], where);
    }


}
