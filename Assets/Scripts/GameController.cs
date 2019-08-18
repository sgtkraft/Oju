using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public enum GameState
    {
        TITLE,
        TOSTANDBY,
        STANDBY,
        PLAY,
        FINISH,
        RESULT,
    }
    public static GameState gameState = GameState.TITLE;

    public enum LayerName
    {
        Default = 0,
        Oju = 8,
        Osechi = 9,
        Guzai = 10,
        Obon = 11,
        OjuGrounded = 12,
        OsechiTransparent = 13,
    }

    public Animator animator;

    public Sprite[] numberSprites;
    public Image timerImage;
    private float timer = 10f;

    public Text messageText;
    public GameObject subMessage, kumoMain;

	// Use this for initialization
	private void Awake()
    {
        timerImage.enabled = false;
        messageText.text = "";
        subMessage.SetActive(false);
        kumoMain.SetActive(true);
	}
	
	// Update is called once per frame
	private void Update()
    {
        switch (gameState)
        {
            case GameState.STANDBY:
                if (Input.GetMouseButtonDown(0)) { StartStandby(); }
                if (Input.GetMouseButton(0)) { timer -= Time.deltaTime; }
                if (Input.GetMouseButtonUp(0)) { StopStandby(); }
                if (timer < 0f) { OnPlay(); }
                break;
            case GameState.PLAY:
                timer -= Time.deltaTime;
                timerImage.sprite = numberSprites[(int)timer];
                if (timer < 0f) { Finish(); }
                break;
        }
	}

    public void OnStandby()
    {
        gameState = GameState.STANDBY;
        timer = 3f;
        subMessage.SetActive(true);
        kumoMain.SetActive(false);
    }

    public void StartStandby()
    {
        timer = 3f;
        subMessage.SetActive(false);
        animator.Play("StandbyToPlay");
        messageText.text = "用\n\n意";
    }

    public void StopStandby()
    {
        animator.Play("Standby");
        messageText.text = "";
        subMessage.SetActive(true);
    }

    public void OnPlay()
    {
        gameState = GameState.PLAY;
        timerImage.enabled = true;
        timer = 10f;

        animator.Play("Play");
        messageText.text = "は\nじ\nめ";
    }

    public void Finish()
    {
        gameState = GameState.FINISH;
        timerImage.enabled = false;

        animator.Play("Finish");
        messageText.color = Color.red;
        messageText.text = "そこまで";
    }

    public void Result()
    {
        gameState = GameState.RESULT;

        messageText.text = "そ\nこ\nま\nで";
    }

    public void OnStartButtonClicked()
    {
        gameState = GameState.TOSTANDBY;
    }
}
