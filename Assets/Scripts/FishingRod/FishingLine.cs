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

    private void Verlet(LineParticle p, float dt)
    {
        Vector3 temp = p.pos;
        p.pos += p.pos - p.oldPos + (p.acc*dt*dt);
        p.oldPos = temp;
    }    
    
    private void Awake()
    {
        particles = new LineParticle[numLineParticles];
    }

    // Start is called before the first frame update
    void Start()
    {
        fr = GetComponentInChildren<FishingRod>();
        ftr = GetComponentInChildren<Floater>();
    }

    void FixedUpdate()
    {
        for(int i =  0; i < numLineParticles; ++i)
        {
            particles[i].acc = gravity;
        }
    }
}
