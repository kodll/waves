using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nest : MonoBehaviour {

    public int id;
    public float BirthDelay;
    public float EndBeforeWaveDuration;
    public float BirthRate;
    public Enemy EnemyShipPrefab;
    [HideInInspector] public float ActualNestTime;
    [HideInInspector] public bool ActiveNest;
    [HideInInspector] public float NestDuration;
    [HideInInspector] public float ActualBirthRate;
    [HideInInspector] public Vector2 Nest2dPosition;

    // Use this for initialization
    void Start ()
    {

    }

    void Update()
    {
        float delta;
        int i;

        delta = Time.deltaTime;

        if (ActiveNest)
        {
            ActualNestTime = ActualNestTime + delta;
            if (ActualNestTime >= BirthDelay)
            {
                ActualBirthRate = ActualBirthRate - delta;
                if (ActualBirthRate<= 0)
                {
                    //Spawn Enemy
                    ActualBirthRate = BirthRate;                    
                    for (i = 0; i < MainScript.GetInstance().maxenemies; i++)
                    {
                        if (MainScript.GetInstance().EnemyShipField[i].enemyinstance == null)
                        {
                            if (EnemyShipPrefab.allowrotation)
                            {
                                MainScript.GetInstance().EnemyShipField[i].enemyinstance = Instantiate(EnemyShipPrefab, this.transform.position, MainScript.GetInstance().GetDirection(this.transform.position, MainScript.GetInstance().PlayerShipInstance.transform.position)) as Enemy;
                            }
                            else
                            {
                                MainScript.GetInstance().EnemyShipField[i].enemyinstance = Instantiate(EnemyShipPrefab, this.transform.position, Quaternion.identity) as Enemy;
                            }
                            MainScript.GetInstance().EnemyShipField[i].enemyinstance.transform.SetParent(MainScript.GetInstance().MapInstance.transform);
                            MainScript.GetInstance().EnemyShipField[i].enemybody = MainScript.GetInstance().EnemyShipField[i].enemyinstance.GetComponent<Rigidbody2D>();
                            MainScript.GetInstance().EnemyShipField[i].enemyinstance.SetId(i);

                            MainScript.GetInstance().MoveToDirection(delta, MainScript.GetInstance().EnemyShipField[i].enemybody, MainScript.GetInstance().PlayerShipInstance.gameObject, MainScript.GetInstance().EnemyShipField[i].enemyinstance.acceleration, MainScript.GetInstance().EnemyShipField[i].enemyinstance.speedlimit, MainScript.GetInstance().EnemyShipField[i].enemyinstance.allowrotation, MainScript.GetInstance().EnemyShipField[i].enemyinstance.magnetdistance);

                            break;
                        }
                    }
                    
                }

                if (ActualNestTime > NestDuration - EndBeforeWaveDuration)
                {
                    //Nest Closed
                    ActiveNest = false;
                }
            }
        }
    }

    public void StartWave(float duration)
    {
        Debug.Log("Wave is generated");
        ActiveNest = true;
        NestDuration = duration;
        ActualBirthRate = 0;
        ActualNestTime = 0;
        Nest2dPosition = Vector2.zero;
        Nest2dPosition.x = this.transform.position.x;
        Nest2dPosition.y = this.transform.position.y;
    }
	
}
