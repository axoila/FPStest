using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using UnityStandardAssets.Characters.FirstPerson;

public class Shooting : NetworkBehaviour {

    [SerializeField] GameObject shot;
    Camera cam;
    FirstPersonController movement;
    [SyncVar]bool dead = false;

    // Use this for initialization
    void Awake () {
        cam = GetComponentInChildren<Camera>();
        movement = GetComponent<FirstPersonController>();
    }
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Fire1") && !dead){
            CmdShoot();
        }
        if(dead && cam.transform.position.y != -0.7f){
            cam.transform.localPosition = Vector3.MoveTowards(
                cam.transform.localPosition,
                Vector3.up * -0.7f, Time.deltaTime * 2);
        }
        if(!dead && cam.transform.position.y != 0.8f){
            cam.transform.localPosition = Vector3.MoveTowards(
                cam.transform.localPosition,
                Vector3.up * 0.8f, Time.deltaTime * 2);
        }

        //Debug
        if(Input.GetKeyDown(KeyCode.K) && !dead){
            CmdGetShot();
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
		Instantiate(shot).GetComponent<RailgunShot>().Setup(start, end);
    }
	[Command]
    public void CmdGetShot(){
        dead = true;
        RpcGetShot();
        StartCoroutine(CmdRevive(3));
    }
	[ClientRpc]
	public void RpcGetShot(){
        movement.enabled = false;
        StartCoroutine(ClientRevive(3));
    }
    IEnumerator CmdRevive(float delay){
        yield return new WaitForSeconds(delay);
        dead = false;
    }
    IEnumerator ClientRevive(float delay){
        yield return new WaitForSeconds(delay);
        movement.enabled = true;
    }
}
