using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fish : MonoBehaviour
{
    public string fishType;
    bool lured;
    bool isSwimming;
    public bool caught;
    public float weight;
    public float maxWeight = 5;
    public float minWeight = 1;
    Rigidbody rb;
    public const float turnSpeed = .05f;
    public const float turnFrames = 10;
    public const float swimForce = 100.0f;
    public const float attractionRadius = 10.0f;
    public const float chances = 1f;
    public const float caughtSpeed = 2.0f;
    
    public GameObject descriptionPrefab;
    public GameObject descriptionInstance;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        isSwimming = false;
        lured = false;
        weight = (float) GameManager.Instance.generator.NextDouble() * (maxWeight - minWeight) + minWeight;
        transform.localScale *= weight/maxWeight;
        rb.mass = weight;
    }

    void Update()
    {
        if(!caught) {
            if(!isSwimming) {
                isLured();
                if(lured) {
                    Vector3 toBob = GameManager.Instance.bob.transform.position - transform.position;
                    rb.velocity = caughtSpeed * Vector3.Normalize(toBob);
                    transform.forward = Vector3.Normalize(toBob);
                } else { 
                    StartCoroutine(swim());
                }
            }
        }
    }

    void OnCollisionEnter(Collision coll)
    {
        if(coll.gameObject.tag == "bob" && !caught) {
            Debug.Log("caught");
            caught = true;
            HingeJoint tempHinge = coll.gameObject.AddComponent<HingeJoint>();
            tempHinge.connectedBody = rb;
            tempHinge.anchor = transform.InverseTransformPoint(coll.GetContact(0).point);
            //rb.useGravity = true;
        }
    }

    void isLured() {
        if(GameManager.Instance.bobLured && !lured) {
            return;
        }

        Vector3 toBob = GameManager.Instance.bob.transform.position - transform.position;
        float dist = toBob.magnitude;
        
        //List<string> baits = GameManager.Instance.fishToBait[fishType];
        //bool isBait = baits.Contains(GameManager.Instance.selectedBaitString) ;
        bool isBait = true;
        if (dist < attractionRadius && isBait) {
            System.Random gen = GameManager.Instance.generator;
            if((float)gen.NextDouble() < chances) {
                lured = true;
                GameManager.Instance.bobLured = true;
                Debug.Log("gottem");
            }
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

    public void hover()
    {
        if(caught) {
            Vector3 s = GetComponent<Collider>().bounds.size;
            Debug.Log(s);
            descriptionInstance = Instantiate(descriptionPrefab,
                new Vector3(transform.position.x, transform.position.y, transform.position.z),
                transform.rotation);
            descriptionInstance.transform.LookAt(2 * descriptionInstance.transform.position - Camera.main.transform.position);
            RectTransform   rt = descriptionInstance.GetComponent<RectTransform>();
            float translationY = rt.sizeDelta.y * rt.localScale.y/2;
            descriptionInstance.transform.position =  new Vector3(descriptionInstance.transform.position.x, descriptionInstance.transform.position.y + s.y/2 + translationY, descriptionInstance.transform.position.z); 
            
            bool isRecord = GameManager.Instance.records[fishType] < weight;
            for (int i = 0; i < descriptionInstance.transform.childCount; ++i) {  
                Transform currentItem = descriptionInstance.transform.GetChild(i);
                if (currentItem.tag == "weight"){
                    currentItem.GetComponent<Text>().text = weight.ToString("n2") + " lbs";
                    if(isRecord) {
                        currentItem.GetComponent<Text>().color = new Color(0.5411765f, 1f, 0);
                    } else {
                        currentItem.GetComponent<Text>().color = new Color(0.67f, .07f, 0);
                    }
                }
                if (currentItem.tag == "recordtext"){
                    if(isRecord) {
                        currentItem.GetComponent<Text>().text = "NEW RECORD!";
                        currentItem.GetComponent<Text>().color = new Color(0.5411765f, 1f, 0);
                    } else {
                        currentItem.GetComponent<Text>().text = "Your record is " + GameManager.Instance.records[fishType].ToString("n2") + " lbs";
                        currentItem.GetComponent<Text>().color = new Color(0.67f, .07f, 0);
                    }
                }
            }
            
            descriptionInstance.transform.parent = gameObject.transform;
        }
    }

    public void exit()
    {
        if(caught) {
            Destroy(descriptionInstance);
        }
    }

    public void click()
    {
        //"consume" fish
        if(caught) {
            if(GameManager.Instance.records[fishType] < weight) {
                GameManager.Instance.records[fishType] = weight;
                GameManager.Instance.bobLured = false;
            }
            Destroy(gameObject);
            exit();
        }
    }
}
