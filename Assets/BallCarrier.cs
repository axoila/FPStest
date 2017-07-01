using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BallCarrier : NetworkBehaviour {

	[SerializeField] Transform ballHolder;

    Ball ball = null;

	void LateUpdate(){
		if(ball){
            ball.transform.position = (ballHolder.position);
        }
	}

	[Command]
    public void CmdCarry(GameObject ball){
        RpcCarry(ball);
    }

	[ClientRpc]
	void RpcCarry(GameObject ballGo){
		ball = ballGo.GetComponent<Ball>();
		ball.rigid.isKinematic = true;
	}

	[Command]
	public void CmdDrop(){
		if(ball){
            ball.CmdDrop();
        }
        RpcDrop();
    }

	[ClientRpc]
	public void RpcDrop(){
        if (ball)
        {
            ball.rigid.isKinematic = false;
            print("Ball dropped");
        }
        ball = null;
	}
}
