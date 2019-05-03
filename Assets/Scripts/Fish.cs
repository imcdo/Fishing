using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    public string fishType;
    bool lured;
    bool isSwimming;
    Rigidbody rb;
    public const float turnSpeed = .05f;
    public const float turnFrames = 10;
    public const float swimForce = 100.0f;
    public const float attractionRadius = 10.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        isSwimming = false;
        lured = false;
    }

    void Update()
    {
        isLured();
        if(!isSwimming) {
            StartCoroutine(swim());
        }
    }

    void isLured() {
        Vector3 toBob = GameManager.Instance.bob.transform.position - transform.position;
        float dist = toBob.magnitude;
        
        //List<string> baits = GameManager.Instance.fishToBait[fishType];
        //bool isBait = baits.Contains(GameManager.Instance.selectedBaitString) ;
        
        if (dist < attractionRadius) {
            lured = true;
            Debug.Log("gottem");
        }
    }
    
    IEnumerator swim(){
        isSwimming = true;
        System.Random gen = GameManager.Instance.generator;
        float deg  = (float)gen.Next(-90, 91);
        float force  = (float)gen.Next(30, 120);
        rb.AddForce(Quaternion.Euler(0, deg, 0) * transform.forward * swimForce);
        for(int i = 0; i <= turnFrames; i++) {
            transform.Rotate(0, deg/turnFrames, 0);
            yield return new WaitForSeconds(turnSpeed / turnFrames);
        }
        float time  = (float)gen.NextDouble() * 3;
        yield return new WaitForSeconds(time);
        isSwimming = false;
    }
}
