using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TimeSeriesItem
{
    public float from;
    public float to;
    public float value;
}

[System.Serializable]
public class CoordinateWithTimeSeries
{
    public int row;
    public int column;
    public List<TimeSeriesItem> timeSeriesItems = new List<TimeSeriesItem>();
    public GameObject bindedHexa;
    public GameObject histoColumn;
}

public class TimeSeriesAnimator : MonoBehaviour
{
    public bool shouldGenerateRandom = true;
    public float randomValueMax = 1f;
    public float randomDivision = 1f;
    public int rows = 7;
    public int cols = 11;

    public List<CoordinateWithTimeSeries> items;
    private float timePassed = 0f;

    void Start()
    {
        if (shouldGenerateRandom)
        {
            items = GenerateRandom(0, 24, randomDivision);
        }

        GameObject[] hexas = GameObject.FindGameObjectsWithTag(Constants.HEXA_TAG);
        foreach (CoordinateWithTimeSeries coord in items)
        {
            foreach (GameObject hexa in hexas)
            {
                HexaProperties propz = hexa.GetComponent<HexaProperties>();
                if (propz.row == coord.row && propz.column == coord.column)
                {
                    coord.bindedHexa = hexa;
                    coord.histoColumn = hexa.transform.GetChild(0).gameObject;
                    break;
                }
            }
        }
    }

    void Update()
    {
        timePassed += Time.deltaTime;
        foreach (CoordinateWithTimeSeries coord in items)
        {
            if (coord.bindedHexa != null)
            {
                foreach (TimeSeriesItem item in coord.timeSeriesItems)
                {
                    if (item.from <= timePassed && item.to >= timePassed)
                    {
                        iTween.ScaleTo(coord.histoColumn, new Vector3(0.5f, item.value, 1), randomDivision);
                        break;
                    }
                }
            }
        }
    
    }

    private List<CoordinateWithTimeSeries> GenerateRandom(float startTime, float endTime, float increment)
    {
        List<CoordinateWithTimeSeries> coordinates = new List<CoordinateWithTimeSeries>();

        for (int rowIdx = 0; rowIdx < rows; rowIdx ++)
        {
            for (int colIdx = 0; colIdx < cols; colIdx++)
            {
                CoordinateWithTimeSeries coord = new CoordinateWithTimeSeries();
                coord.row = rowIdx;
                coord.column = colIdx;
                coordinates.Add(coord);

                for (float i = startTime; i < endTime; i += increment)
                {
                    TimeSeriesItem timeSeriesItem = new TimeSeriesItem();
                    timeSeriesItem.from = i;
                    timeSeriesItem.to = i + increment;
                    timeSeriesItem.value = Random.Range(0, randomValueMax);
                    coord.timeSeriesItems.Add(timeSeriesItem);
                }
            }
        }

        return coordinates;
    }
}
