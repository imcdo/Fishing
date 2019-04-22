using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingLog : MonoBehaviour
{
    public int currentPageIndex;
    public List<GameObject> pagePrefabs;
    public GameObject currentPageInstance;
    public Vector3 pagePosition = new Vector3(0, 0, 0);
    void Start()
    {
        pagePosition = transform.position;
        currentPageIndex = 0;
        changePage(currentPageIndex);
    }

    void changePage(int index)
    {
        currentPageIndex = index;
        Destroy(currentPageInstance);
        currentPageInstance = Instantiate(pagePrefabs[currentPageIndex], pagePosition, Quaternion.identity);
        currentPageInstance.transform.parent = gameObject.transform;
    }

    void Update()
    {
        if (GvrControllerInput.ClickButtonDown)
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
}
