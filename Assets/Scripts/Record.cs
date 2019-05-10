using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Record : MonoBehaviour
{
    public string fishName;
    Text record;
    // Start is called before the first frame update
    void Start()
    {
        record = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        record.text = "Record: " + GameManager.Instance.records[fishName].ToString("n2") + " lbs";
    }
}
