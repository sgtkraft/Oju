using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour
{
    public GameController gc;

    public Sprite[] sprites;
    public Image imageTemp;
    public GameObject panel, valueParent, lineParent, line;

    private Rigidbody2D rb;
    private Vector3 defaultPos;
    private List<Image> imageList = new List<Image>();

    private bool isScored, isDone = false;

    void Awake()
    {
        rb = line.GetComponent<Rigidbody2D>();
        defaultPos = line.transform.position;
    }
	
	// Update is called once per frame
	void Update()
    {
        switch (gc.state)
        {
            case GameController.State.TITLE:
                if (isDone) { return; }

                imageTemp.gameObject.SetActive(false);
                panel.SetActive(false);
                valueParent.SetActive(false);
                lineParent.SetActive(false);

                isDone = true;
                break;
            case GameController.State.TOSTANDBY:
                isDone = false;
                break;
            case GameController.State.SCORE:
                if (isDone) { return; }

                panel.SetActive(true);
                lineParent.SetActive(true);

                isScored = Physics2D.Linecast(
                    new Vector2(-3, line.transform.position.y),
                    new Vector2(3, line.transform.position.y),
                    1 << (int)GameController.Layer.OjuGrounded | (int)GameController.Layer.Oju
                );

                if (isScored)
                {
                    SetVelocity(4);
                }
                else
                {
                    SetVelocity(0);
                    SetScore(GetScore());
                    valueParent.SetActive(true);
                    gc.Result();

                    isDone = true;
                }
                break;
            case GameController.State.TOTITLE:
                isDone = false;

                imageTemp.gameObject.SetActive(false);
                panel.SetActive(false);
                valueParent.SetActive(false);
                lineParent.SetActive(false);

                line.transform.position = defaultPos;
                for (int i = imageList.Count; i > 0; i--)
                {
                    Image img = imageList[i - 1];
                    imageList.Remove(img);
                    Destroy(img.gameObject);
                }
                break;
        }
    }

    private void SetVelocity(int speed)
    {
        rb.velocity = new Vector2(0f, speed);
    }

    /// <summary>
    /// スコアを計算
    /// </summary>
    private int GetScore()
    {
        float score = line.transform.localPosition.y * 10;
        Debug.Log(score);
        if (Mathf.Approximately(score % 1, 0.5f))
        {
            return (int)score + 1;
        }
        else
        {
            return Mathf.RoundToInt(score);
        }
    }

    private void SetScore(int score)
    {
        gc.score = score;

        string scoreStr = score.ToString();
        for(int i = scoreStr.Length; i > 0; i--)
        {
            ShowScore(int.Parse(scoreStr.Substring(i - 1, 1)));
        }
    }

    private void ShowScore(int score)
    {
        Image image = Instantiate(imageTemp, valueParent.transform);
        image.transform.SetSiblingIndex(0);
        image.sprite = sprites[score];
        image.gameObject.SetActive(true);

        imageList.Add(image);
    }
}
