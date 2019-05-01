using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingRod : MonoBehaviour
{
    [SerializeField] Vector3 lineAnchorPos;

    private GameObject RigidRod;
    private GameObject FollowRod;

    private const int NUM_ROD_POINTS = 6;
    Vector3[] rodPoints;

    float L;

    private GameObject[] cubes;

    public Vector3 lineAnchor
    {
        get { return lineAnchorPos; }
        private set { lineAnchorPos = value; }
    }

    private void Awake()
    {
        RigidRod = transform.Find("GvrControllerPointer").Find("handle").Find("RigidRod").gameObject;
        FollowRod = transform.Find("FollowRod").gameObject;

        ConfigurableJoint[] joints = FollowRod.GetComponents<ConfigurableJoint>();

        Vector3 startRod = joints[0].anchor;
        Vector3 endRod   = joints[1].anchor;

        L = Vector3.Distance(endRod, startRod); 

        rodPoints = new Vector3[NUM_ROD_POINTS];

        for (int i = 0; i < NUM_ROD_POINTS; i++)
        {
            rodPoints[i] = startRod + endRod  * i / (NUM_ROD_POINTS - 1);
            Debug.Log(rodPoints[i]);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        cubes = new GameObject[rodPoints.Length];
        int i = 0;
        foreach (Vector3 point in rodPoints)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.SetParent(FollowRod.transform.parent);
            cube.transform.localScale = new Vector3(.1f, .1f, .1f);
            Debug.Log(point);
            cube.transform.SetPositionAndRotation(FollowRod.transform.localPosition + point, Quaternion.identity);
            cubes[i++] = cube;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Matrix4x4 ltwFollow = FollowRod.transform.localToWorldMatrix;
        Matrix4x4 ltwRigid = RigidRod.transform .localToWorldMatrix;
        for (int i = 0; i < rodPoints.Length; i++)
        {
            Vector4 worldPointRigid =  ltwRigid  * new Vector4(rodPoints[i].x, rodPoints[i].y, rodPoints[i].z, 1);
            // Vector4 worldPointFollow = ltwFollow * new Vector4(rodPoints[i].x, rodPoints[i].y, rodPoints[i].z, 1);
            Vector4 worldPointFollow = worldPointRigid;
            // Vector4 worldPointRigid = worldPointFollow;

            float w = RodEquation((1.0f * i) / rodPoints.Length);


            Vector3 worldPoint = (1-w) * new Vector3(worldPointRigid.x, worldPointRigid.y, worldPointRigid.z) +
                (w) * new Vector3(worldPointFollow.x, worldPointFollow.y, worldPointFollow.z);
            Debug.Log(w + " " + worldPointRigid + " " + worldPointFollow + " " + worldPoint);

            cubes[i].transform.SetPositionAndRotation(
                worldPoint, FollowRod.transform.rotation);
        }
    }


    // returns the weight twords bendy/follow rod
    float RodEquation(float t)
    {
        // NOTE w is a weight between 0 - 1
        // w = t^2(3L-t)/2L^3
        // L = length of beam, TODO: get it to the actual transorm length

        float tSq = Mathf.Pow(t, 2);
        float Lcb = Mathf.Pow(L, 3);

        float w = tSq *(3 * L - t);
        w /= 2 * Lcb;
        return w;
    }
}
