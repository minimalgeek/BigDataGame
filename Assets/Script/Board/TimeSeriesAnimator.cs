using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

[System.Serializable]
public class TimeSeriesItem
{
    public float from;
    public float to;
    public float predictedValue;
    public float realValue;

    public CoordinateWithTimeSeries parent;
}

[System.Serializable]
public class CoordinateWithTimeSeries
{
    public int row;
    public int column;
    public List<TimeSeriesItem> timeSeriesItems = new List<TimeSeriesItem>();
    public GameObject bindedField;
    public GameObject histoColumn;
}

public class TimeSeriesAnimator : MonoBehaviour
{
    public bool shouldGenerateRandom = true;
    public float randomValueMax = 1f;
    public float randomDivision = 1f;

    public float normalizeValue;
    public int rows = 10;
    public int cols = 10;

    public List<CoordinateWithTimeSeries> items;
    private List<TimeSeriesItem> currentTimeSeriesItems;
    private int iterationIndex = 0;
    private GameManager gameManager;
    private SelectManager selectManager;

    void Awake()
    {
        gameManager = GetComponent<GameManager>();
        selectManager = GetComponent<SelectManager>();
    }

    void Start()
    {
        if (shouldGenerateRandom)
        {
            items = GenerateRandom(0, 24, randomDivision);
        }
        else
        {
            items = BuildFromStringMatrix(GetComponent<CSVReader>().grid);
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
                    coord.bindedField = hexa;
                    coord.histoColumn = hexa.transform.GetChild(0).gameObject;
                    break;
                }
            }
        }

        StartCoroutine(NextRound(0.5f));
    }

    void Update()
    {

    }

    private List<CoordinateWithTimeSeries> BuildFromStringMatrix(string[,] grid)
    {
        List<CoordinateWithTimeSeries> coordinates = new List<CoordinateWithTimeSeries>();

        for (int y = 0; y < grid.GetUpperBound(1); y++)
        {
            try
            {
                int row = Int32.Parse(grid[0, y]);
                int col = Int32.Parse(grid[1, y]);

                CoordinateWithTimeSeries cwt = coordinates.Find(x => x.row == row && x.column == col);
                if (cwt == null)
                {
                    cwt = new CoordinateWithTimeSeries();
                    cwt.row = row;
                    cwt.column = col;
                    coordinates.Add(cwt);
                }
                TimeSeriesItem tii = new TimeSeriesItem();
                tii.from = float.Parse(grid[2, y]);
                tii.to = float.Parse(grid[3, y]);
                tii.predictedValue = float.Parse(grid[4, y]) / normalizeValue;
                tii.realValue = float.Parse(grid[5, y]) / normalizeValue;
                tii.parent = cwt;
                cwt.timeSeriesItems.Add(tii);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                Debug.Log("At: " + y);
                throw e;
            }
        }

        return coordinates;
    }

    private List<CoordinateWithTimeSeries> GenerateRandom(float startTime, float endTime, float increment)
    {
        List<CoordinateWithTimeSeries> coordinates = new List<CoordinateWithTimeSeries>();

        for (int rowIdx = 0; rowIdx < rows; rowIdx++)
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
                    timeSeriesItem.predictedValue = UnityEngine.Random.Range(0, randomValueMax);
                    timeSeriesItem.realValue = Mathf.Clamp(timeSeriesItem.predictedValue * (0.8f + UnityEngine.Random.Range(0f, 0.4f)), 0f, 1f);
                    timeSeriesItem.parent = coord;
                    coord.timeSeriesItems.Add(timeSeriesItem);
                }
            }
        }

        return coordinates;
    }

    public void EvaluateBattle()
    {
        if (selectManager.FullSelectedList == null)
        {
            return;
        }

        List<TimeSeriesItem> playerVotes = new List<TimeSeriesItem>();
        foreach (GameObject go in selectManager.FullSelectedList)
        {
            foreach (CoordinateWithTimeSeries coord in items)
            {
                if (coord.bindedField == go)
                {
                    playerVotes.Add(coord.timeSeriesItems[iterationIndex]);
                    break;
                }
            }
        }

        IEnumerable<TimeSeriesItem> biggestIncrementListAI =
          (from item in currentTimeSeriesItems
           orderby (item.predictedValue) descending
           select item).Take(playerVotes.Count);

        List<TimeSeriesItem> aiVotes = new List<TimeSeriesItem>(biggestIncrementListAI);

        IEnumerable<TimeSeriesItem> realBiggestList =
          (from item in currentTimeSeriesItems
           orderby (item.realValue) descending
           select item).Take(playerVotes.Count);

        int playerPoint = 0, aiPoint = 0;
        foreach (TimeSeriesItem bigItem in realBiggestList)
        {
            if (playerVotes.Contains(bigItem))
            {
                playerPoint++;
            }
            if (aiVotes.Contains(bigItem))
            {
                aiPoint++;
            }

            iTween.ScaleTo(bigItem.parent.bindedField,
                iTween.Hash("x", 0.6f,
                            "y", 0.6f,
                            "time", 0.2f, "looptype", "pingpong"));
        }

        gameManager.AiPoints += aiPoint;
        gameManager.PlayerPoints += playerPoint;

        StartCoroutine(NextRound(2f));
    }

    private IEnumerator NextRound(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        currentTimeSeriesItems = new List<TimeSeriesItem>();
        selectManager.ResetAll();
        iterationIndex += 1;
        foreach (CoordinateWithTimeSeries coord in items)
        {
            iTween.ScaleTo(coord.bindedField, Vector3.one * 0.4f, 0.2f);
            if (coord.bindedField != null && iterationIndex < coord.timeSeriesItems.Count)
            {
                TimeSeriesItem item = coord.timeSeriesItems[iterationIndex];
                currentTimeSeriesItems.Add(item);
                iTween.ScaleTo(coord.histoColumn, new Vector3(0.5f, item.predictedValue, 1), 1f);
            }

            if (iterationIndex >= coord.timeSeriesItems.Count)
            {
                gameManager.FinishGame();
            }
        }
        gameManager.UpdateRoundsLabel(iterationIndex + 1);
    }
}
