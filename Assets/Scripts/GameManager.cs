using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
	private static GameManager instance;
	public static GameManager Instance {
		get {
			return instance;
		}
	}

    //holds records of highest weight fish caught
    public Dictionary<string, double> records;
    public FishingLog fishingLog;
    
	void Awake () {
		if(instance != null && instance != this) {
			Destroy(this.gameObject);
		} else {
			instance = this;
		}
		records = new Dictionary<string, double>();
        fishingLog = GameObject.FindGameObjectWithTag("log").GetComponent<FishingLog>();
	}

	// Update is called once per frame
	void Update () {

	}
}