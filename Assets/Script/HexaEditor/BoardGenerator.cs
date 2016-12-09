using UnityEngine;
using UnityEditor;
using System.Collections;

[System.Serializable]
public class BoardLayout {
    public int rows;
    public int maxColumns;
    public Vector2 startPosition;
    public float xShift;
    public float yShift;
}

[ExecuteInEditMode]
public class BoardGenerator : MonoBehaviour {

    public GameObject hexaPrefab;
    public BoardLayout layout;

	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
        int childs = transform.childCount;
        for (int i = 0; i < childs; i++)
        {
            GameObject.DestroyImmediate(transform.GetChild(0).gameObject);
        }

        Vector2 newPos = new Vector2();
        for (int i = 0; i < layout.rows; i++)
        {
            for (int j = 0; j < layout.maxColumns; j++) //  - absOfI
            {
                newPos.x = layout.startPosition.x + j * layout.xShift;
                newPos.y = layout.startPosition.y + i * layout.yShift;
                GameObject instantiatedHexa = (GameObject)Instantiate(hexaPrefab, newPos, Quaternion.identity);
                instantiatedHexa.transform.SetParent(this.gameObject.transform);

                HexaProperties propz = instantiatedHexa.GetComponent<HexaProperties>();
                propz.row = i;
                propz.column = j;
            }
        }
    }
}
