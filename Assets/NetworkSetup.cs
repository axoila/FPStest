using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkSetup : NetworkBehaviour {
    [SerializeField] Behaviour[] playerOnly;
    // Use this for initialization
    void Start () {
		foreach(Behaviour playerOnlyBehaviour in playerOnly){
            playerOnlyBehaviour.enabled = isLocalPlayer;
        }
	}
}
