using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingLine : MonoBehaviour
{
    [SerializeField] private int numLineParticles = 25;
    [SerializeField] private Vector3 gravity = new Vector3(0, -1, 0);
    
    private FishingRod fr;
    private Floater ftr;
    
    [HideInInspector] public float lineLength;


    private struct LineParticle
    {
        public Vector3 pos;
        public Vector3 oldPos;
        public Vector3 acc;
    }
    
    private LineParticle[] particles;
    private GameObject[] cubes;

   
    
    private void Awake()
    {
        particles = new LineParticle[numLineParticles];
        cubes = new GameObject[numLineParticles];
        for (int i = 0; i < numLineParticles; i++)
        {
            cubes[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cubes[i].transform.localScale = new Vector3(.1f, .1f, .1f);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        fr = GetComponentInChildren<FishingRod>();
        ftr = GetComponentInChildren<Floater>();
    }

    void Update()
    {
        for (int i = 0; i < numLineParticles; i++)
        {
            cubes[i].transform.position = particles[i].pos;
//            Debug.Log(particles[i].acc + " " + particles[i].pos);
        }
    }
    
    void FixedUpdate()
    {
        Verlet(ref particles[0], Time.deltaTime);
        for(int i =  1; i < numLineParticles; ++i)
        {
            particles[i].acc = gravity;
            Verlet(ref particles[i], Time.deltaTime);
            PoleConstraint(ref particles[i - 1], ref particles[i], 1);
        }
    }
    
    private void Verlet(ref LineParticle p, float dt)
    {
        Vector3 temp = p.pos;
        p.pos += p.pos - p.oldPos + (p.acc*dt*dt);
        p.oldPos = temp;
    }    
    
    private void PoleConstraint(ref LineParticle p1, ref LineParticle p2, float restLength)
    {
        Vector3 delta = p2.pos - p1.pos;

        float deltaLength = delta.magnitude;

        float diff = (deltaLength - restLength)/deltaLength;

        p1.pos += delta*diff*0.5f;
        p2.pos -= delta*diff*0.5f;
    }
}
