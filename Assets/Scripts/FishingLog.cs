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
        currentPageIndex = 0;
        changePage(currentPageIndex);
    }

    void changePage(int index) {
        currentPageIndex = index;
        Destroy(currentPageInstance);
        currentPageInstance = Instantiate(pagePrefabs[currentPageIndex], pagePosition, Quaternion.identity);
    }
}
