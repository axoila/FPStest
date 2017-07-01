using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TeamGoal : NetworkBehaviour {
    public string teamName = "defaultteam";
    public int score = 0;
    public static int maxScore = 10;

	void Start(){
		TeamScoreUI.SetScore(teamName, score);
	}

	[Command]
    public void CmdScore(){
        score++;
        if(score >= maxScore){
            print("code win screen");
            Network.Disconnect();
            MasterServer.UnregisterHost();
        }
        RpcUpdateScore(score);
    }

	[ClientRpc]
	void RpcUpdateScore(int newScore){
        score = newScore;
        TeamScoreUI.SetScore(teamName, score);
    }

	void OnTriggerEnter(Collider coll){
		if(coll.CompareTag("Ball") && hasAuthority){
            coll.GetComponent<Ball>().CmdScore();
            CmdScore();
        }
	}
}
