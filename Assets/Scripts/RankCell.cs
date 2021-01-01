using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MaterialUI;
using TMPro;
using NCMB;

public class RankCell : MonoBehaviour
{
    public TextMeshProUGUI rankText, nameText, scoreText;

    public void Init(int rank, string name, int score)
    {
        rankText.SetText(rank.ToString());
        nameText.SetText(name);
        scoreText.SetText(score.ToString());

        gameObject.SetActive(true);
    }
}
