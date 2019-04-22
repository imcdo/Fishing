using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingRodDebugInput : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        float rotspeed = 200;
        if (Input.GetKey("left shift"))
        {
            rotspeed *= 3;
        }
        Vector3 axis = transform.right;
        if (Input.GetKey("r"))
        {
            transform.Rotate(axis, rotspeed * Time.deltaTime, Space.World);
        } else if (Input.GetKey("f"))
        {
            transform.Rotate(axis, -rotspeed * Time.deltaTime, Space.World);
        }
    }
}
