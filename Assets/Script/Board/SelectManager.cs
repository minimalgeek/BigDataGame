using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelectManager : MonoBehaviour
{

    public int maxSelectable = 3;
    public Color restoreColor;
    public Color markColor;
    private List<GameObject> selectedList = new List<GameObject>();

    private Vector3 selectedScale;


    public float scaleMultiplier;

    public List<GameObject> FullSelectedList
    {
        get
        {
            if (selectedList.Count == maxSelectable)
                return selectedList;
            return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 screenPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(screenPos, Vector2.zero);
            if (hit.collider != null && hit.collider.tag == Constants.HEXA_TAG)
            {
                GameObject go = hit.collider.gameObject;
                if (selectedList.Contains(go))
                {
                    selectedList.Remove(go);
                    RestoreOldObject(go);
                }
                else if (selectedList.Count < maxSelectable)
                {
                    selectedList.Add(go);
                    AnimateNewObject(go);
                }

            }
        }
    }

    private void RestoreOldObject(GameObject hit)
    {
        iTween.Stop(hit);
        iTween.ScaleTo(hit, selectedScale, 0.2f);
        hit.GetComponent<SpriteRenderer>().color = restoreColor;
    }

    private void AnimateNewObject(GameObject hit)
    {
        selectedScale = hit.transform.localScale;
        
        // change values
        iTween.ScaleTo(hit,
            iTween.Hash("x", selectedScale.x * scaleMultiplier,
                        "y", selectedScale.y * scaleMultiplier,
                        "time", 0.5, "looptype", "pingpong"));
        hit.GetComponent<SpriteRenderer>().color = markColor;
    }

    public void ResetAll()
    {
        foreach (GameObject go in selectedList)
        {
            RestoreOldObject(go);
        }

        selectedList.Clear();
    }

    //public List<TimeSeriesItem> GetSelectedTimeSeriesItems(int idx)
    //{
    //    List<TimeSeriesItem> ret = new List<TimeSeriesItem>();

    //    foreach (GameObject sel in selectedList)
    //    {
    //        ret.Add
    //    }

    //    return ret;
    //}
}
