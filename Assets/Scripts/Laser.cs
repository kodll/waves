using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour {

    EnemyWeapon ParentWeapon = null;
    int ParentWeaponId = -1;
    float ParentWeaponDuration = 0;

    public bool Following;

    public LineRenderer[] Line;

    public void InitLaser(EnemyWeapon parent, int id, float duration)
    {
        ParentWeapon = parent;
        ParentWeaponId = id;
        ParentWeaponDuration = duration;
        UpdateLaser();

    } 
	
    void DestroyLaser()
    {
        Destroy(this.gameObject);
    }

    void UpdateLaser()
    {
        int i;

        ParentWeaponDuration -= Time.deltaTime;

        if (ParentWeaponDuration<=0)
        {
            DestroyLaser();
        }

        if (ParentWeaponId != -1)
        {
            if (ParentWeapon == null)
            {
                DestroyLaser();
            }
            else
            {
                for (i = 0; i < Line.Length; i++)
                {
                    Line[i].SetPosition(0, ParentWeapon.Weapons[ParentWeaponId].WeaponPosition.transform.position);
                    if (Following)
                    {
                        Line[i].SetPosition(1, MainScript.GetInstance().PlayerShipInstance.transform.position);
                    }
                    else
                    {
                        Debug.DrawRay(ParentWeapon.Weapons[ParentWeaponId].WeaponPosition.transform.position, ParentWeapon.Weapons[ParentWeaponId].WeaponPosition.transform.right * 10, Color.green);

                        RaycastHit2D hit = Physics2D.Raycast(ParentWeapon.Weapons[ParentWeaponId].WeaponPosition.transform.position, ParentWeapon.Weapons[ParentWeaponId].WeaponPosition.transform.right);
                        /*
                        Vector3 hit3d = Vector3.zero;
                        hit3d.x = hit.point.x;
                        hit3d.y = hit.point.y;*/

                        /*if (Physics.Raycast(ParentWeapon.Weapons[ParentWeaponId].WeaponPosition.transform.position, ParentWeapon.Weapons[ParentWeaponId].WeaponPosition.transform.right, out hit, Mathf.Infinity))
                        {
                            Debug.Log("Object hit on position: " + hit.point);
                        }*/


                        Line[i].SetPosition(1, hit.point);
                    }
                }
            }
        }
    }

	// Update is called once per frame
	void Update ()
    {
        UpdateLaser();
    }
}
