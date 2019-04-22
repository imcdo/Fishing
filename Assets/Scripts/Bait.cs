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
            new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), 
            transform.rotation);
        descriptionInstance.transform.parent = gameObject.transform;
    }

    public void exit() {
        myRenderer.material = inactiveMaterial;
        Destroy(descriptionInstance);
        Debug.Log("exit");
    }

    public void click() {
        GameManager.Instance.baitList.Remove(gameObject);
        if(GameManager.Instance.selectedBait != null) {
            GameManager.Instance.baitList.Add(GameManager.Instance.selectedBait);
            GameManager.Instance.selectedBait.SetActive(true);
        }
        
        GameManager.Instance.selectedBait = gameObject;
        GameManager.Instance.selectedBait.SetActive(false);
        exit();
    }

    private void Start()
    {
        myRenderer = GetComponent<Renderer>();
    }
}
