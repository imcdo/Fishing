using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider))]
public class Bait : MonoBehaviour
{
    public Material inactiveMaterial;
    public Material gazedAtMaterial;
    private Renderer myRenderer;
    public GameObject descriptionPrefab;

    public GameObject descriptionInstance;

    public void hover()
    {
        myRenderer.material = gazedAtMaterial;
        descriptionInstance = Instantiate(descriptionPrefab, 
            new Vector3(transform.position.x, transform.position.y, transform.position.z + 1), 
            transform.rotation);
            
    }

    public void exit() {
        myRenderer.material = inactiveMaterial;
        Destroy(descriptionInstance);
        Debug.Log("exit");
    }


    private void Start()
    {
        myRenderer = GetComponent<Renderer>();
    }
}
