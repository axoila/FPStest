using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailgunShot : MonoBehaviour {

	LineRenderer line;

	void Start(){
		StartCoroutine(stop(1));
	}

	public void Setup(Vector3 start, Vector3 end){
		if(!line)
			line = GetComponent<LineRenderer>();
		transform.position = Vector3.zero;
		transform.rotation = Quaternion.identity;
		transform.localScale = Vector3.one;

		line.SetPosition(0, start);
		line.SetPosition(1, end);
	}

	IEnumerator stop(float lingerTime){
		yield return new WaitForSeconds(lingerTime);
		Destroy(gameObject);
	}
}
