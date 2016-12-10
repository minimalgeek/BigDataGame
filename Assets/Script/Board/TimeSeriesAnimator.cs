using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TimeSeriesItem
{
    public float from;
    public float to;
    public float predictedValue;
    public float realValue;
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
    public int rows = 10;
    public int cols = 10;

    public List<CoordinateWithTimeSeries> items;
    private int iterationIndex = 0;
    private GameManager gameManager;

    void Awake()
    {
        gameManager = GetComponent<GameManager>();
    }

    void Start()
    {
        if (shouldGenerateRandom)
        {
            items = GenerateRandom(0, 24, randomDivision);
        }
        gameManager.rounds = items[0].timeSeriesItems.Count;

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

        JumpToNextSeriesItem();
    }

    void Update()
    {

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
                    timeSeriesItem.predictedValue = Random.Range(0, randomValueMax);
                    timeSeriesItem.realValue = Mathf.Clamp(timeSeriesItem.predictedValue * (0.8f + Random.Range(0f, 0.4f)), 0f, 1f);
                    coord.timeSeriesItems.Add(timeSeriesItem);
                }
            }
        }

        return coordinates;
    }

    public void JumpToNextSeriesItem()
    {
        iterationIndex += 1;
        foreach (CoordinateWithTimeSeries coord in items)
        {
            if (coord.bindedHexa != null && iterationIndex < coord.timeSeriesItems.Count)
            {
                TimeSeriesItem item = coord.timeSeriesItems[iterationIndex];
                iTween.ScaleTo(coord.histoColumn, new Vector3(0.5f, item.predictedValue, 1), randomDivision);
            }
        }
    }
}
