using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Shooting : NetworkBehaviour {

    Camera cam;

	[SyncVar]int health = 5;

    // Use this for initialization
    void Awake () {
        cam = GetComponentInChildren<Camera>();
    }
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Fire1")){
            CmdShoot();
        }
	}

	[Command]
    public void CmdShoot()
    {
		RaycastHit hit;
        Ray ray = cam.ViewportPointToRay(cam.rect.size * 0.5f);
        if (Physics.Raycast(ray, out hit, 15))
        {
            RpcShoot(ray.origin, hit.point);
            Shooting enemyShot = hit.collider.GetComponent<Shooting>();
            if (enemyShot && enemyShot != this)
            {
                enemyShot.CmdGetShot();
            }
        }
        else
        {
            RpcShoot(ray.origin, ray.direction * 15);
        }
    }

	[ClientRpc]
	public void RpcShoot(Vector3 start, Vector3 end){
		Debug.DrawLine(start, end, Color.red, 1, true);
    }
	[Command]
    public void CmdGetShot(){
        health--;
        RpcGetShot();
    }
	[ClientRpc]
	public void RpcGetShot(){
        print("I got shot");
    }
}
