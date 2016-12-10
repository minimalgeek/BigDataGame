using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour
{
    public int rounds;
    public Text roundsLabel;

    public int playerPoints;
    public int aiPoints;

    public Text playerPointsLabel;
    public Text aiPointsLabel;
    public GameObject endGamePanel;
    public Text endGameLabel;

    public int PlayerPoints
    {
        get
        {
            return playerPoints;
        }

        set
        {
            playerPoints = value;
            playerPointsLabel.text = "Telekom: " + playerPoints.ToString();
        }
    }

    public int AiPoints
    {
        get
        {
            return aiPoints;
        }

        set
        {
            aiPoints = value;
            aiPointsLabel.text = "Vodafone: " + aiPoints.ToString();
        }
    }

    void Start()
    {
        PlayerPoints = 0;
        AiPoints = 0;
    }

    void Update()
    {

    }

    public void UpdateRoundsLabel(int iteration)
    {
        roundsLabel.text = "Round - " + iteration + "/" + rounds;
    }

    public void FinishGame()
    {
        endGamePanel.SetActive(true);
        if (playerPoints > aiPoints)
        {
            endGameLabel.text = "You won! :)";
        } else
        {
            endGameLabel.text = "Vodafone won! :(";
        }
    }
}