using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

	public GameObject[] gamePrefabs;
	private float spawnTime = 2.0f;
	private	GameObject go;
	private bool canSpawn = true, ph1 = false, ph2 = false, ph3 = false, ph4 = false;

	void Update () {
		transform.position = new Vector3(Random.Range(PlayerManager.xBoundLeft, PlayerManager.xBoundRight), 1.2f, 2);
		if(Player.currentLife < 3 && Random.Range(0, 10) > 8.5f){
			go = gamePrefabs[(int)Mathf.Floor(Random.Range(0, 4.9f))];
		} else {
			if(Random.Range(0, 100) > 80)
				go = gamePrefabs[(int)Mathf.Floor(Random.Range(0, 2.9f))];
			else
				go = gamePrefabs[(int)Mathf.Floor(Random.Range(0, 3.9f))];
		}
		var waitTime = Random.Range(0.1f, spawnTime);
		if(canSpawn){
			StartCoroutine(spawn(waitTime));
		}
		
		if(!ph1 && Player.runtime > 50){
			spawnTime = 1.2f;
			ph1 = true;
		}
		
		if(!ph2 && Player.runtime > 120){
			spawnTime = 0.5f;
			ph2 = true;
		}
		
		if(!ph3 && Player.runtime > 250){
			spawnTime = 0.3f;
			ph3 = true;
		}
		if(!ph4 && Player.runtime > 400){
			spawnTime = 0.2f;
			ph4 = true;
		}
	}

	private IEnumerator spawn(float delay){
		canSpawn = false;
		yield return new WaitForSeconds(delay);
		Rigidbody.Instantiate(go, transform.position, go.transform.rotation);
		canSpawn = true;
	}
}
