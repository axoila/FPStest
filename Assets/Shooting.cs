using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using UnityStandardAssets.Characters.FirstPerson;

public class Shooting : NetworkBehaviour {

    [SerializeField] GameObject shot;
    [SerializeField] Vector2 cameraHeightRange = new Vector2(-0.7f, 0.8f);
    [SerializeField] float railgunRange = 50f;
    [SerializeField] float impactForce = 10f;
    [SerializeField] float fireDelay = 1f;
    Camera cam;
    FirstPersonController movement;
    BallCarrier carrier;
    [SyncVar]public bool dead = false;
    bool canShoot = true;

    // Use this for initialization
    void Awake () {
        cam = GetComponentInChildren<Camera>();
        movement = GetComponent<FirstPersonController>();
        carrier = GetComponent<BallCarrier>();
    }
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Fire1") && !dead && isLocalPlayer){
            LocalShoot();
        }
        if(dead && cam.transform.position.y != cameraHeightRange.x){
            cam.transform.localPosition = Vector3.MoveTowards(
                cam.transform.localPosition,
                Vector3.up * cameraHeightRange.x, Time.deltaTime * 2);
        }
        if(!dead && cam.transform.position.y != cameraHeightRange.y){
            cam.transform.localPosition = Vector3.MoveTowards(
                cam.transform.localPosition,
                Vector3.up * cameraHeightRange.y, Time.deltaTime * 2);
        }

        //Debug
        if(Input.GetKeyDown(KeyCode.K) && !dead && isLocalPlayer){
            CmdGetShot();
        }
	}
    [Client]
    void LocalShoot(){
        if (!canShoot) return;
        RaycastHit hit;
        Ray ray = cam.ViewportPointToRay(cam.rect.size * 0.5f);
        if (Physics.Raycast(ray, out hit, railgunRange))
        {
            Instantiate(shot).GetComponent<RailgunShot>().Setup(ray.origin, hit.point);
            Shooting enemyShot = hit.collider.GetComponent<Shooting>();
            Rigidbody rigid = hit.collider.GetComponent<Rigidbody>();
            CmdShoot(ray.origin, hit.point, enemyShot||rigid?hit.collider.gameObject:null);
        }
        else
        {
            Instantiate(shot).GetComponent<RailgunShot>().Setup(ray.origin, ray.origin + ray.direction * railgunRange);
            CmdShoot(ray.origin, ray.origin + ray.direction * railgunRange, null);
        }
        canShoot = false;
        StartCoroutine(ReactivateShot(fireDelay));
    }

	[Command]
    public void CmdShoot(Vector3 start, Vector3 end, GameObject victim)
    {
        if(victim){
            Shooting enemyShot = victim.GetComponent<Shooting>();
            if (enemyShot)
            {
                enemyShot.CmdGetShot();
            } else {
                Rigidbody rb = victim.GetComponent<Rigidbody>();
                rb.AddForce((end-start).normalized * impactForce, ForceMode.VelocityChange);
            }
        }
        RpcShoot(start, end);
    }

	[ClientRpc]
	public void RpcShoot(Vector3 start, Vector3 end){
        if(!isLocalPlayer)
		    Instantiate(shot).GetComponent<RailgunShot>().Setup(start, end);
    }
	[Command]
    public void CmdGetShot(){
        dead = true;
        carrier.CmdDrop();
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
        movement.enabled = isLocalPlayer;
    }
    IEnumerator ReactivateShot(float delay){
        yield return new WaitForSeconds(delay);
        canShoot = true;
    }
}
