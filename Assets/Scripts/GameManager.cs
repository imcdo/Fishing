using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }

    //holds records of highest weight fish caught
    public Dictionary<string, double> records;

    
    // "worm", "algae", "minnow", "copepod"
    public Dictionary<string, List<string>> fishToBait = new Dictionary<string, List<string>>()
    {
        {"Largemouth Bass",        new List<string>(){"worm", "minnow"}},
        {"Brown Trout",            new List<string>(){"copepod", "minnow"}},
        {"Whitespotted Char",      new List<string>(){"worm", "copepod"}},
        {"Amago",                  new List<string>(){"worm", "algae", "minnow"}},
        {"Steelhead Trout",        new List<string>(){"worm"}},
        {"Common catfish",         new List<string>(){"algae", "minnow"}},
        {"Northern pike",          new List<string>(){"copepod", "minnow"}},
        {"Ayu Sweetfish",          new List<string>(){"algae"}}
    };
    public FishingLog fishingLog;
    public List<GameObject> baitList;
    public List<string> fishList;
    public GameObject selectedBait;
    public string selectedBaitString;
    public System.Random generator;
    public Bob bob;
    public bool bobLured;
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
        fishToBait = new Dictionary<string, List<string>>();
        records = new Dictionary<string, double>();
        generator = new System.Random();
        foreach (string fish in fishList)
        {
            records.Add(fish, 0);
        }
        bobLured = false;
    }

    private void Start()
    {
        bob = GameObject.FindGameObjectWithTag("bob").GetComponent<Bob>();
        fishingLog = GameObject.FindGameObjectWithTag("log").GetComponent<FishingLog>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}