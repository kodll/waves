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

    public GameObject PlayerShipPrefab;
    [HideInInspector] public GameObject PlayerShipInstance;
    [HideInInspector] public LevelDefinition MapInstance;

    float WavesTime;
    int ActiveWave;
    bool FinalWave;

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
        PlayerShipInstance = Instantiate(PlayerShipPrefab, Vector3.zero, Quaternion.identity) as GameObject;
    }

    void Start ()
    {
        int i;
        maxenemies = 200;
        EnemyShipField = new EnemyShipStruct[maxenemies];

        Vector3 pos;       
        pos = Vector3.zero;
        pos.x = 2;
        pos.y = 5;
        PlayerShipInstance.transform.position = pos;

        
        for (i = 0; i < maxenemies; i++)
        {
            EnemyShipField[i].enemyinstance = null;
        }

        MapInstance = GameObject.FindObjectOfType(typeof(LevelDefinition)) as LevelDefinition;
        MapInstance.InitMap();
        WavesTime = 0;
        ActiveWave = 0;
        FinalWave = false;
    }

    // Update is called once per frame
    void Update ()
    {
        float delta;
        delta = Time.deltaTime;
        UpdateEnemies(delta);
        SetCamera(delta);
        WaveControl(delta);
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

    public void MoveToDirection(float delta, Rigidbody2D movingbody, GameObject finalposition, float speed, float speedlimit, bool rotatealways)
    {
        float atan2;
        Vector3 dir;

        if (finalposition != null)
        {
            dir = finalposition.transform.position - movingbody.transform.position;    
        }
        else
        {
            dir = movingbody.transform.right;
        }

        movingbody.AddForce(dir.normalized * speed);

        if (movingbody.velocity.magnitude > speedlimit)
        {
            movingbody.velocity = movingbody.velocity.normalized * speedlimit;
        }

        if (movingbody.velocity.magnitude > 0.5 && rotatealways)
        {
            atan2 = Mathf.Atan2(movingbody.velocity.y, movingbody.velocity.x);
            movingbody.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, atan2 * Mathf.Rad2Deg);
            //movingbody.gameObject.transform.rotation = Quaternion.Lerp(movingbody.gameObject.transform.rotation, Quaternion.Euler(0f, 0f, atan2 * Mathf.Rad2Deg), 0.1f);

        }
        
    }

    void UpdateEnemies(float delta)
    {
        int i;
        for (i = 0; i< maxenemies; i++)
		{
            if (EnemyShipField[i].enemyinstance != null)
            {
                MoveToDirection(delta, EnemyShipField[i].enemybody, PlayerShipInstance, EnemyShipField[i].enemyinstance.acceleration, EnemyShipField[i].enemyinstance.speedlimit, EnemyShipField[i].enemyinstance.allowrotation);
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
