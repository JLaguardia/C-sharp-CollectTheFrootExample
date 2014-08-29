using UnityEngine;
using System.Collections;

public class Collectable : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(gameObject.tag != "Life"){
			transform.position -= new Vector3(0,(Player.currentMultiplier/4 + Player.fallSpeed) * Time.deltaTime, 0);
		} else{
			transform.position -= new Vector3(0, 0.5f * Player.fallSpeed * Time.deltaTime, 0);
		}
	}
}
