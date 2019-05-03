using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FishingLine : MonoBehaviour
{

    private LineRenderer lr;
    
    [SerializeField] private float numLineParticlesPerLengthUnit = 1f;

    private int numLineParticles;
    
    [SerializeField] private Vector3 gravity = new Vector3(0, -10, 0);
    [SerializeField] [Range(0f,1f)]private float dampFactor = .5f;
    
    private FishingRod fr;
    private Floater ftr;


    [HideInInspector] public float lineLength = 4f;
    [HideInInspector] public Vector3 firstPoint;    
    private float prevLineLength;

    [SerializeField] GameObject floater;

    private class LineParticle
    {
        public Vector3 pos;
        public Vector3 oldPos;
        public Vector3 acc;
    }
    
    private List<LineParticle> particles;
    private GameObject[] cubes;

    private float timestep;
   
    
    private void Awake()
    {
        prevLineLength = lineLength;
        numLineParticles = Mathf.FloorToInt(lineLength / numLineParticlesPerLengthUnit);
        Debug.Log(lineLength + " " + numLineParticles);
        
        
        particles = new List<LineParticle>();
        cubes = new GameObject[numLineParticles];
        for (int i = 0; i < numLineParticles; i++)
        {
            particles.Add(new LineParticle());
            cubes[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cubes[i].transform.localScale = new Vector3(.1f, .1f, .1f);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        fr = GetComponentInChildren<FishingRod>();
        ftr = GetComponentInChildren<Floater>();
        lr = GetComponent<LineRenderer>();
    }

    void Update()
    {
        lr.positionCount = numLineParticles;
        lr.startColor = Color.white;
        lr.endColor = Color.white;
        lr.startWidth = .1f;
        lr.endWidth = .1f;
        
        
        for (int i = 0; i < cubes.Length; i++) Destroy(cubes[i]);
        cubes = new GameObject[numLineParticles];
        for (int i = 0; i < numLineParticles; i++)
        {
            cubes[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cubes[i].transform.localScale = new Vector3(.1f, .1f, .1f);
        }
        for (int i = 0; i < numLineParticles; i++)
        {
            lr.SetPosition(i, particles[i].pos);
            cubes[i].transform.position = particles[i].pos;
//            Debug.Log(particles[i].acc + " " + particles[i].pos);
        }
    }
    
    void FixedUpdate()
    {
        particles[0].pos = firstPoint;
//        Verlet(ref particles[0], Time.deltaTime);
        Vector3 endPos = particles[0].pos;
//        Debug.Log(lineLength);
        // Update number of particles
        numLineParticles = Mathf.FloorToInt(lineLength / numLineParticlesPerLengthUnit);
        
        int prevNumLineParticles = Mathf.FloorToInt(prevLineLength / numLineParticlesPerLengthUnit);
        if (prevNumLineParticles != numLineParticles)
        {
            // is dirty need to recalculate particles
            int diffParticle = Math.Abs(numLineParticles - prevNumLineParticles);
            if (numLineParticles < prevNumLineParticles)
            {
                // add the differnce to the front
                for(int i = 0; i < diffParticle; i++)
                    particles.RemoveAt(1);
            }
            else
            {
                // remove difference from the front
                for (int i = 0; i < diffParticle; i++)
                {
                    particles.Insert(1, new LineParticle());
                    particles[1].pos = particles[0].pos + (particles[2].pos - particles[0].pos)  * ((i + 1)/ diffParticle);
                    particles[1].oldPos =  particles[0].oldPos + (particles[2].oldPos - particles[0].oldPos)  * ((i + 1)/ diffParticle);
                }
            }
        }
        
        
        for(int i =  1; i < numLineParticles; ++i)
        {
            particles[i].acc = gravity;
            Verlet(particles[i], Time.deltaTime);
            PoleConstraint(particles[i - 1], particles[i],  lineLength/ (1.0f * numLineParticles) );
        }
        particles[numLineParticles - 1].acc = 10 * gravity;

        particles[0].pos = endPos;

        prevLineLength = lineLength;
        timestep =Time.deltaTime;
    }
    
    private void Verlet( LineParticle p, float dt)
    {
        Vector3 temp = p.pos;
        p.pos += p.pos - p.oldPos + (p.acc*dt*dt);
        p.oldPos = temp;
    }    
    
    private void PoleConstraint( LineParticle p1, LineParticle p2, float restLength)
    {
        Vector3 delta = p2.pos - p1.pos;

        float deltaLength = delta.magnitude;

        float diff = (deltaLength - restLength)/deltaLength;
        p1.pos += delta*diff*dampFactor;
        p2.pos -= delta*diff*dampFactor;
    }
    public Vector3 getTipVelocity()
    {
        Debug.Log(timestep + " " + particles[particles.Count - 1].pos + " " + (particles[particles.Count - 1].pos - particles[particles.Count - 1].oldPos));
        
        return (particles[particles.Count - 1].pos - particles[particles.Count - 1].oldPos) ;
    }
}
