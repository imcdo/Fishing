using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingRod : MonoBehaviour
{
    [SerializeField] Vector3 lineAnchorPos;
    public Vector3 lineAnchor
    {
        get { return lineAnchorPos; }
        private set { lineAnchorPos = value; }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
