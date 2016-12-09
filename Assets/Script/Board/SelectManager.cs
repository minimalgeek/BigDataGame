using UnityEngine;
using System.Collections;

public class SelectManager : MonoBehaviour {

    private GameObject selected = null;
    private Vector3 selectedScale;
    private float alphaRestoreValue;

    public float scaleMultiplier;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 screenPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(screenPos, Vector2.zero);
            if (hit.collider != null && hit.collider.tag == Constants.HEXA_TAG)
            {
                if (hit.collider.gameObject == selected)
                {
                    return;
                }

                RestoreOldObject();
                AnimateNewObject(hit);
            }
        }
    }

    private void RestoreOldObject()
    {
        if (selected != null)
        {
            iTween.Stop(selected);
            iTween.ScaleTo(selected, selectedScale, 0.2f);
            Color restoreColor = selected.GetComponent<SpriteRenderer>().color;
            restoreColor.a = alphaRestoreValue;
            selected.GetComponent<SpriteRenderer>().color = restoreColor;
        }
    }

    private void AnimateNewObject(RaycastHit2D hit)
    {
        selected = hit.collider.gameObject;

        selectedScale = selected.transform.localScale;
        Color newColor = selected.GetComponent<SpriteRenderer>().color;
        alphaRestoreValue = newColor.a;
        newColor.a = 255;

        // change values
        iTween.ScaleTo(selected, 
            iTween.Hash("x", selectedScale.x * scaleMultiplier, 
                        "y", selectedScale.y * scaleMultiplier, 
                        "time", 0.5, "looptype", "pingpong"));
        selected.GetComponent<SpriteRenderer>().color = newColor;
    }
}
