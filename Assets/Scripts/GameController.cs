using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using MaterialUI;
using CommonDefine;

public class GameController : MonoBehaviour
{
    public enum State
    {
        TITLE,
        TOSTANDBY,
        STANDBY,
        PLAY,
        //FINISH,
        TOSCORE,
        SCORE,
        //TORESULT,
        //RESULT,
        TOTITLE,
    }
    public State state = State.TITLE;

    public enum Layer
    {
        Default = 0,
        Oju = 8,
        Osechi = 9,
        Guzai = 10,
        Obon = 11,
        OjuGrounded = 12,
        OsechiTransparent = 13,
        ScoreLine = 14,
    }

    public Animator animator;

    public Sprite[] numberSprites;
    public Image timerImage;

    public Text messageText;
    public GameObject waitImgGo, subMessage, scoreTitle, kumoMain;

    public MaterialButton startBtn, quitBtn, shareBtn, retryBtn;

    public int score;
    private float timer = 10f;
    private bool isScored;

    // Use this for initialization
    private void Awake()
    {
        timerImage.enabled = false;

        messageText.text = "";
        subMessage.SetActive(false);
        waitImgGo.SetActive(false);
        scoreTitle.SetActive(false);

        quitBtn.gameObject.SetActive(false);
        shareBtn.gameObject.SetActive(false);
        retryBtn.gameObject.SetActive(false);

        kumoMain.SetActive(true);
    }

    // Update is called once per frame
    private void Update()
    {
        switch (state)
        {
            case State.STANDBY:
                if (Input.GetMouseButtonDown(0)) { StartStandby(); }
                if (Input.GetMouseButtonUp(0)) { StopStandby(); }
                break;
            case State.PLAY:
                timer -= Time.deltaTime;
                timerImage.sprite = numberSprites[(int)timer];
                if (timer < 0f) { Finish(); }
                break;
        }
	}

    public void OnTitle()
    {
        state = State.TITLE;

        startBtn.interactable = true;
    }

    public void OnStartButtonClicked()
    {
        state = State.TOSTANDBY;
        animator.Play("TitleToStandby");
    }

    public void OnStandby()
    {
        state = State.STANDBY;

        messageText.text = "";
        subMessage.SetActive(true);
    }

    public void StartStandby()
    {
        subMessage.SetActive(false);
        waitImgGo.SetActive(true);
        animator.Play("StandbyToPlay");
        messageText.color = ColorDef.black;
        messageText.text = "用\n\n意";
    }

    public void StopStandby()
    {
        animator.Play("Standby");
        messageText.text = "";
        subMessage.SetActive(true);
        waitImgGo.SetActive(false);
    }

    public void OnPlay()
    {
        state = State.PLAY;
        timerImage.enabled = true;
        timer = 10f;

        animator.Play("Play");
        messageText.color = ColorDef.black;
        messageText.text = "は\nじ\nめ";

        quitBtn.textText = "やめる";
        quitBtn.interactable = true;
        quitBtn.gameObject.SetActive(true);
    }

    public void Finish()
    {
        state = State.TOSCORE;
        timerImage.enabled = false;

        animator.Play("Finish");
        messageText.color = ColorDef.red;
        messageText.text = "そこまで";
    }

    public void OnScore()
    {
        if(!isScored)
        {
            isScored = true;
            return;
        }
        state = State.SCORE;

        messageText.text = "";
        scoreTitle.SetActive(true);
    }

    public void Result()
    {
        quitBtn.textText = "おわる";

        shareBtn.interactable = true;
        shareBtn.gameObject.SetActive(true);

        retryBtn.interactable = true;
        retryBtn.gameObject.SetActive(true);
    }

    public void OnShareButtonClicked()
    {
        StartCoroutine(Share());
    }

    private IEnumerator Share()
    {
        string scoreStr; // ツイートに挿入するテキスト
        if (score == 0)
        {
            scoreStr = string.Format("お重を10秒でたくさん積み上げ、徳の高さを誇示しよう！");
        }
        else if (score < 50)
        {
            scoreStr = string.Format("お重を10秒で{0}cm積み上げました！めでたい！", score);
        }
        else if (score < 100)
        {
            scoreStr = string.Format("お重を10秒で{0}cm積み上げました！すばらしい！", score);
        }
        else if (score < 150)
        {
            scoreStr = string.Format("お重を10秒で{0}cm積み上げました！すごすぎる！", score);
        }
        else
        {
            scoreStr = string.Format("お重を10秒で{0}cm積み上げました！やばすぎる！", score);
        }

        string linkUrl = "https://sgtkraft.github.io/oju-10seconds/";   // ツイートに挿入するURL
        string hashtags = "#すがたくらふと #お重10Seconds #Unity";        // ツイートに挿入するハッシュタグ

        quitBtn.gameObject.SetActive(false);
        shareBtn.gameObject.SetActive(false);
        retryBtn.gameObject.SetActive(false);

        // スクリーンショットを撮影しツイート
        string tweetStr = string.Format("{0}\nゲームはこちら→ {1}\n{2}\n", scoreStr, linkUrl, hashtags);
        Debug.Log(tweetStr);
        yield return StartCoroutine(ShareManager.TweetWithScreenShot(tweetStr));

        quitBtn.gameObject.SetActive(true);
        shareBtn.gameObject.SetActive(true);
        retryBtn.gameObject.SetActive(true);

        yield break;
    }

    public void OnQuitButtonClicked()
    {
        animator.Play("Quit");
    }

    public void Quit()
    {
        state = State.TOTITLE;
        timerImage.enabled = false;
        isScored = false;

        messageText.text = "";
        subMessage.SetActive(false);
        waitImgGo.SetActive(false);
        scoreTitle.SetActive(false);

        quitBtn.gameObject.SetActive(false);
        shareBtn.gameObject.SetActive(false);
        retryBtn.gameObject.SetActive(false);

        kumoMain.SetActive(true);
    }

    public void OnRetryButtonClicked()
    {
        animator.Play("Retry");
    }

    public void Retry()
    {
        state = State.TOSTANDBY;
        timerImage.enabled = false;
        isScored = false;

        messageText.text = "";
        waitImgGo.SetActive(false);
        scoreTitle.SetActive(false);

        quitBtn.gameObject.SetActive(false);
        shareBtn.gameObject.SetActive(false);
        retryBtn.gameObject.SetActive(false);

        kumoMain.SetActive(false);
    }
}
