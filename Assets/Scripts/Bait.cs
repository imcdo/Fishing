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
    public string baitString;
    public GameObject descriptionInstance;

    public void hover()
    {
        Vector3 s = GetComponent<Collider>().bounds.size;
        myRenderer.material = gazedAtMaterial;
        descriptionInstance = Instantiate(descriptionPrefab,
            new Vector3(transform.position.x, transform.position.y, transform.position.z),
            transform.rotation);
        descriptionInstance.transform.LookAt(2 * descriptionInstance.transform.position - Camera.main.transform.position);
        RectTransform rt = descriptionInstance.GetComponent<RectTransform>();
        float translationY = rt.sizeDelta.y * rt.localScale.y/2;
        descriptionInstance.transform.position =  new Vector3(descriptionInstance.transform.position.x, 
            descriptionInstance.transform.position.y + s.y/2 + 
            translationY, descriptionInstance.transform.position.z) + 
            new Vector3(Camera.main.transform.forward.x * s.x, 
                0,
                Camera.main.transform.forward.z * s.z); 
        descriptionInstance.transform.parent = gameObject.transform;
    }

    public void exit()
    {
        myRenderer.material = inactiveMaterial;
        Destroy(descriptionInstance);
        Debug.Log("exit");
    }

    public void click()
    {
        GameManager.Instance.baitList.Remove(gameObject);
        if (GameManager.Instance.selectedBait != null)
        {
            GameManager.Instance.baitList.Add(GameManager.Instance.selectedBait);
            GameManager.Instance.selectedBait.SetActive(true);
        }

        GameManager.Instance.selectedBait = gameObject;
        GameManager.Instance.selectedBait.SetActive(false);
        GameManager.Instance.selectedBaitString = baitString;
        exit();
    }

    private void Start()
    {
        myRenderer = GetComponent<Renderer>();
    }
}
