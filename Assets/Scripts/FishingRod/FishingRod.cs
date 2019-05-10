using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingRod : MonoBehaviour
{
    [SerializeField] Vector3 lineAnchorPos;

    private GameObject RigidRod;
    private GameObject FollowRod;

    private GameObject controller;

    [HideInInspector] public bool pickedup = false;

    [SerializeField] private float reelSpeed = 10;
    [SerializeField] private float releaseSpeed = 20;

    private Transform[] rodBones;

    Vector3[] rodPoints;

    private Vector3 startpos;
    private FishingLine fl;
    float L;

//    private GameObject[] cubes;

    public Vector3 lineAnchor
    {
        get { return lineAnchorPos; }
        private set { lineAnchorPos = value; }
    }

    private void Awake()
    {
        fl = GetComponentInChildren<FishingLine>();
        
        RigidRod = transform.Find("handle").Find("RigidRod").gameObject;
        FollowRod = transform.Find("FollowRod").gameObject;

        startpos = transform.position;
        ConfigurableJoint[] joints = FollowRod.GetComponents<ConfigurableJoint>();

        GameObject rodMesh = GetComponentInChildren<Animator>().gameObject;
        int numBones = 0;
        Transform bone = rodMesh.transform.GetChild(0);
        while (bone.childCount != 0)
        {
            numBones++;
            bone = bone.GetChild(0);
        }

        rodPoints = new Vector3[numBones];

        Vector3 startRod = joints[0].anchor;
        Vector3 endRod = joints[1].anchor;
        Vector3 endVector = endRod - startRod;

        L = Vector3.Distance(endRod, startRod);

        for (int c = 0; c < numBones; c++)
        {
            rodPoints[c] = startRod + endVector * c / (numBones - 1);
            //            Debug.Log(rodPoints[c]);
        }

        bone = rodMesh.transform.GetChild(0);
        rodBones = new Transform[numBones];
        for (int b = 0; b < numBones; b++)
        {
            rodBones[b] = bone.GetChild(0);
            bone = bone.GetChild(0);
        }

    }


    // Start is called before the first frame update
    void Start()
    {
        
        controller = GameObject.FindWithTag("Controller");
        
//        cubes = new GameObject[rodPoints.Length];
        int i = 0;
        foreach (Vector3 point in rodPoints)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.SetParent(FollowRod.transform.parent);
            cube.transform.localScale = new Vector3(.1f, .1f, .1f);
//            Debug.Log(point);
            cube.transform.SetPositionAndRotation(FollowRod.transform.localPosition + point, Quaternion.identity);
//            cubes[i++] = cube;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // TODO: temp
        controller = GameObject.FindWithTag("Controller");
        if (controller != null) pickedup = pickedup && controller.activeInHierarchy;
        if (pickedup)
        {
            if (controller != null)
            {
                Transform handle = RigidRod.transform.parent;
                handle.position = controller.transform.position;
                handle.rotation = controller.transform.rotation;
            }


            Vector2 touchPos = GvrControllerInput.TouchPos;
            if (Input.GetKey("z") ||
                (GvrControllerInput.ClickButton && touchPos.y > .5))
            {
                reel(Time.deltaTime);
            }
            if (Input.GetKey("x") ||
                (GvrControllerInput.ClickButton && touchPos.y <= .5))
            {
                release(Time.deltaTime);
            }
        } else
        {
            transform.position = startpos;
        }
        
        Matrix4x4 ltwFollow = FollowRod.transform.localToWorldMatrix;
        Matrix4x4 ltwRigid = RigidRod.transform .localToWorldMatrix;

        for (int i = 0; i < rodPoints.Length; i++)
        {
            Vector4 worldPointRigid =  ltwRigid * new Vector4(rodPoints[i].x/10.0f, rodPoints[i].y/10.0f, rodPoints[i].z/10.0f, 1);
            Vector4 worldPointFollow = ltwFollow * new Vector4(rodPoints[i].x, rodPoints[i].y, rodPoints[i].z, 1);


            float w = RodEquation((1.0f * i) / (rodPoints.Length -1));
            
            Vector3 worldPoint = (1-w) * new Vector3(worldPointRigid.x, worldPointRigid.y, worldPointRigid.z) +
                (w) * new Vector3(worldPointFollow.x, worldPointFollow.y, worldPointFollow.z);
            Quaternion worldRot = Quaternion.Slerp(RigidRod.transform.rotation ,FollowRod.transform.rotation, w);
            
            if (i == rodPoints.Length - 1) fl.firstPoint = worldPoint;

//            cubes[i].transform.SetPositionAndRotation(worldPoint, FollowRod.transform.rotation);
            rodBones[i].transform.SetPositionAndRotation(worldPoint, worldRot);
            rodBones[i].transform.Rotate(new Vector3(90, 0, 0));
        }
    }

    // returns the weight towards bendy/follow rod
    float RodEquation(float t)
    {
        // NOTE w is a weight between 0 - 1
        // w = t^2(3L-t)/2L^3

        float tSq = Mathf.Pow(t, 2);

        float w = tSq *(3  - t);
        w /= 2;
        return w;
    }

    void reel(float amount)
    {
        amount *= reelSpeed;
        fl.lineLength = Mathf.Max(fl.lineLength - amount, 0);
    }

    void release(float amount)
    {
        // amount *= releaseSpeed;
//        Debug.Log(fl.getTipVelocity() + " " + Vector3.Magnitude(fl.getTipVelocity()));
        fl.lineLength += Vector3.Magnitude(fl.getTipVelocity());
    }

    public void ApplyAccelerationToTip(Vector3 acc)
    {
        Matrix4x4 ltwFollow = FollowRod.transform.localToWorldMatrix;
        Vector3 tipPos = ltwFollow * new Vector4(
            rodPoints[rodPoints.Length-1].x, 
            rodPoints[rodPoints.Length - 1].y, 
            rodPoints[rodPoints.Length - 1].z, 1);

        FollowRod.GetComponent<Rigidbody>().AddForceAtPosition(acc, tipPos);
    }

    public void grab() { pickedup = true;  }
}
