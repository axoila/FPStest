using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSetup : MonoBehaviour {

    [SerializeField] string[] additionalScenes;

    // Use this for initialization
    void Start () {
		foreach(string sceneName in additionalScenes){
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
