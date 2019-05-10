using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class FishingLine : MonoBehaviour
{

    private LineRenderer lr;
    
    [SerializeField] private float numLineParticlesPerLengthUnit = 1f;

    private int numLineParticles;
    
    [SerializeField] private Vector3 gravity = new Vector3(0, -10, 0);
    [SerializeField] [Range(0f,1f)]private float dampFactor = .5f;
    
    private FishingRod fr;


    [HideInInspector] public float lineLength = 4f;
    [HideInInspector] public Vector3 firstPoint;    
    private float prevLineLength;

    [SerializeField] private float lineWeight = 5;
    
    
    private class LineParticle
    {
        public Vector3 pos;
        public Vector3 oldPos;
        public Vector3 acc;
    }
    
    private List<LineParticle> particles;
    private float timestep;

    
    public GameObject endObj;
    private Vector3 endOldVel = Vector3.zero;
    
    
    private void Awake()
    {
        prevLineLength = lineLength;
        numLineParticles = Mathf.FloorToInt(lineLength / numLineParticlesPerLengthUnit);
        Debug.Log(lineLength + " " + numLineParticles);
        
        
        particles = new List<LineParticle>();
        for (int i = 0; i < numLineParticles; i++)
        {
            particles.Add(new LineParticle());
        }
        Rigidbody endRb = endObj.GetComponent<Rigidbody>();
        if (endObj != null && endRb != null) endObj.transform.position = particles[numLineParticles - 1].pos;
    }

    // Start is called before the first frame update
    void Start()
    {
        fr = transform.parent.parent.parent.GetComponent<FishingRod>();
        lr = GetComponent<LineRenderer>();
    }

    void Update()
    {
        lr.positionCount = numLineParticles;
        lr.startColor = Color.white;
        lr.endColor = Color.white;
        lr.startWidth = .01f;
        lr.endWidth = .01f;
    
        for (int i = 0; i < numLineParticles; i++)
        {
//            Debug.DrawLine(particles[i].pos,particles[i].pos + particles[i].acc, Color.red);
            lr.SetPosition(i, particles[i].pos);
        }

    }
    
    void FixedUpdate()
    {
        // TODO: handle edge case of 1 and 0 linepoints
        
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
        
        
        // handle all particles in the inside of the line
        for(int i =  1; i < numLineParticles -1; ++i)
        {

            particles[i].acc = lineWeight *  gravity;
            Verlet(particles[i], Time.fixedDeltaTime);
            PoleConstraint(particles[i - 1], particles[i],  lineLength/ (1.0f * numLineParticles) );
        }
        particles[numLineParticles - 1].acc = lineWeight * gravity;

//        particles[0].pos = endPos;

        prevLineLength = lineLength;
        timestep = Time.fixedDeltaTime;

        // Handle the last boi 
        Rigidbody endRb = endObj.GetComponent<Rigidbody>();
        if (endObj != null && endRb != null)
        {
            LineParticle last = particles[numLineParticles - 1];
            last.acc =  (endRb.velocity - endOldVel) / Time.fixedDeltaTime;
            Verlet(last, Time.fixedDeltaTime);
            PoleConstraint(particles[numLineParticles - 2],  last,  lineLength/ (1.0f * numLineParticles) );
            endObj.transform.position = last.pos;
            endRb.velocity = (last.pos - last.oldPos)/ Time.fixedDeltaTime;
            endOldVel = endRb.velocity;
            Debug.DrawLine(last.pos,     last.pos + last.acc, Color.red);
            Debug.DrawLine(endRb.position, endRb.position + endRb.velocity / Time.fixedDeltaTime , Color.blue);
        }
        // fr.ApplyAccelerationToTip(particles[0].acc * 1000000000000000000000.0f);

    }

    private void Verlet( LineParticle p, float dt)
    {
        Vector3 temp = p.pos;
        p.pos += p.pos - p.oldPos + (p.acc*dt*dt);  
        p.oldPos = temp;
    }
    
    // returns teh change in position applied to p2
    private void PoleConstraint( LineParticle p1, LineParticle p2, float restLength)
    {
        Vector3 delta = p2.pos - p1.pos;

        float deltaLength = delta.magnitude;

        float diff = (deltaLength - restLength)/deltaLength;
        p1.pos += delta*diff*dampFactor;
        p2.pos -= delta*diff*dampFactor;
    }
    
    // returns teh change in position applied to p2
    private void PoleConstraint( LineParticle p1, LineParticle p2, float restLength, out Vector3 p1Change , out Vector3 p2Change)
    {
        Vector3 delta = p2.pos - p1.pos;

        float deltaLength = delta.magnitude;

        float diff = (deltaLength - restLength)/deltaLength;
        Vector3 oldPos1 = p1.pos;
        Vector3 oldPos2 = p2.pos;

        p1.pos += delta*diff*dampFactor;
        p2.pos -= delta*diff*dampFactor;

        p1Change = (p1.pos - oldPos1);
        p2Change = (p2.pos - oldPos2);
    }
    
    public Vector3 getTipVelocity()
    {    
//        Debug.Log(timestep + " " + particles[particles.Count - 1].pos + " " + (particles[particles.Count - 1].pos - particles[particles.Count - 1].oldPos));
        return (particles[particles.Count - 1].pos - particles[particles.Count - 1].oldPos);
    }
}


