using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingLog : MonoBehaviour
{
    public bool hovered = false;
    public int currentPageIndex;
    public List<GameObject> pagePrefabs;
    public GameObject currentPageInstance;
    void Start()
    {
        currentPageIndex = 0;
        changePage(currentPageIndex);
    }

    void changePage(int index)
    {
        currentPageIndex = index;
        Destroy(currentPageInstance);
        currentPageInstance = Instantiate(pagePrefabs[currentPageIndex], transform.position + transform.rotation * new Vector3(0, 0, -.125f), transform.rotation);
        currentPageInstance.transform.parent = gameObject.transform;
    }

    void Update()
    {
        if (GvrControllerInput.ClickButtonDown && hovered)
        {
            Vector2 touchPos = GvrControllerInput.TouchPos;
            if (touchPos.x < .5 && currentPageIndex > 0)
            {
                currentPageIndex--;
                changePage(currentPageIndex);
            }
            else if (touchPos.x > .5 && currentPageIndex < pagePrefabs.Count - 1)
            {
                currentPageIndex++;
                changePage(currentPageIndex);
            }
        }
    }

    public void hover()
    {
        hovered = true;
    }

    public void exit()
    {
        hovered = false;
    }
}
