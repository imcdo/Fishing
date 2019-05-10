using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bob : MonoBehaviour
{
    public float viscoscity = 4.0f;
    public float buoyancy = 25.0f;
    Rigidbody rb;
    BoxCollider bc;
    public bool caught;

    bool onWater;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        bc = gameObject.GetComponent<BoxCollider>();

        Vector3 s = bc.size;
        Vector3 c = bc.center;
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "water") {
            Vector3 sum = Vector3.zero;
            float count = 0;

            Vector3 s = bc.size;
            Vector3 origin = bc.center + new Vector3(-s.x/2, -s.y/2, -s.z/2);
            float regions = 5.0f;
            float step = 1/(regions-1);
            for(float x = 0; x <= 1.0f; x+=step) {
                for(float y = 0; y <= 1.0f; y+=step) {
                    for(float z = 0; z <= 1.0f; z+=step) {
                        Vector3 worldPos = transform.TransformPoint(origin + new Vector3(x * s.x, y * s.y, z * s.z));
                        if(other.gameObject.GetComponent<BoxCollider>().bounds.Contains(worldPos)) {
                            sum += worldPos;
                            count++;
                        }
                    }
                }
            }
            float percent = count / (regions * regions * regions);
            // Debug.Log(percent);
            if(percent > 0) {
                Vector3 center = sum/count;
                Vector3 forceup = new Vector3(0, buoyancy * percent, 0);
                rb.AddForceAtPosition(forceup, center);
                Vector3 drag = rb.velocity * -1 * viscoscity;
                rb.AddForce(drag);
                Vector3 flow = new Vector3(0, 0, -.4f);
                rb.AddForce(flow);
            }
        }
    }
}
