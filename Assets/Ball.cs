using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Ball : NetworkBehaviour {
    [SerializeField]
    float spawnVelocity;

    [HideInInspector]
	public Rigidbody rigid;
    Vector3 origin;
    BallCarrier carrier;
    SphereCollider trigger;


    void Awake(){
        rigid = GetComponent<Rigidbody>();
		rigid.velocity = Vector3.up * spawnVelocity;
        origin = transform.position;
        SphereCollider[] colls = GetComponents<SphereCollider>();
		foreach(SphereCollider sc in colls)
			if(sc.isTrigger){
                trigger = sc;
                break;
            }
    }

	void Update(){
		//so the server always updates the events
        rigid.MovePosition(rigid.position);
    }

	void OnTriggerEnter(Collider other){
		if(other.tag == "Player" && !rigid.isKinematic && hasAuthority){
            BallCarrier _carrier = other.GetComponent<BallCarrier>();
            Shooting shooty = other.GetComponent<Shooting>();
            if (_carrier && shooty && !shooty.dead){
                _carrier.CmdCarry(gameObject);
                carrier = _carrier;
            }
        }
	}

	[Command]
	public void CmdDrop(){
        Collider[] colls = Physics.OverlapSphere(trigger.center, trigger.radius);
		foreach(Collider c in colls){
            Shooting shooty = c.GetComponent<Shooting>();
            if (shooty && !shooty.dead)
            {
                c.GetComponent<BallCarrier>().CmdCarry(gameObject);
            }
        }
	}

	[Command]
	public void CmdScore(){
        carrier.CmdDrop();
        RpcScore();
    }
	[ClientRpc]
    public void RpcScore()
    {
        transform.position = origin;
        rigid.isKinematic = false;
        rigid.velocity = Vector3.up * spawnVelocity;
    }
}
