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
        RESULT,
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
    public GameObject waitImgGo;
    public GameObject subMessage, scoreTitle;
    public GameObject quitButton, shareButton, retryButton;
    public GameObject kumoMain, yuka;

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
        quitButton.SetActive(false);
        shareButton.SetActive(false);
        retryButton.SetActive(false);

        kumoMain.SetActive(true);
        yuka.SetActive(true);
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

    public void SetState(State nextState)
    {
        state = nextState;
    }

    public void OnTitle()
    {
        state = State.TOTITLE;
        timerImage.enabled = false;
        isScored = false;

        messageText.text = "";
        subMessage.SetActive(false);
        waitImgGo.SetActive(false);
        scoreTitle.SetActive(false);
        quitButton.SetActive(false);
        shareButton.SetActive(false);
        retryButton.SetActive(false);

        kumoMain.SetActive(true);
        yuka.SetActive(true);
    }

    public void OnStartButtonClicked()
    {
        state = State.TOSTANDBY;
        animator.Play("TitleToStandby");
    }

    public void OnStandby()
    {
        state = State.STANDBY;
        subMessage.SetActive(true);
        kumoMain.SetActive(false);
        yuka.SetActive(false);
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

        quitButton.SetActive(true);
        quitButton.GetComponent<MaterialButton>().textText = "やめる";
    }

    public void OnQuitButtonClicked()
    {
        animator.Play("Quit");
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
            Debug.Log("scored");
            return;
        }

        Debug.Log("scored2");
        state = State.SCORE;

        messageText.text = "";
        scoreTitle.SetActive(true);
    }

    public void Result()
    {
        state = State.RESULT;

        quitButton.GetComponent<MaterialButton>().textText = "おわる";
        shareButton.SetActive(true);
    }

    protected Texture2D currentScreenShotTexture;

    protected IEnumerator UpdateCurrentScreenShot()
    {
        // これがないとReadPixels()でエラーになる
        yield return new WaitForEndOfFrame();

        currentScreenShotTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        currentScreenShotTexture.Apply();
    }

    public void OnShareButtonClicked()
    {
        // スクリーンショット用のTexture2D用意
        //currentScreenShotTexture = new Texture2D(Screen.width, Screen.height);

        //StartCoroutine(UpdateCurrentScreenShot());

        //byte[] imageBytes = currentScreenShotTexture.EncodeToPNG();
        //var encodedImage = System.Convert.ToBase64String(imageBytes);
        //Debug.Log(imageBytes);
        //Debug.Log(encodedImage);

        string scoreText = ""; // ツイートに挿入するテキスト
        if (score == 0)
        {
            scoreText = string.Format("お重を10秒でたくさん積み上げ、徳の高さを誇示しよう！");
        }
        else if (score < 50)
        {
            scoreText = string.Format("お重を10秒で{0}cm積み上げました！めでたい！", score);
        }
        else if (score < 100)
        {
            scoreText = string.Format("お重を10秒で{0}cm積み上げました！すばらしい！", score);
        }
        else if (score < 150)
        {
            scoreText = string.Format("お重を10秒で{0}cm積み上げました！すごすぎる！", score);
        }
        else
        {
            scoreText = string.Format("お重を10秒で{0}cm積み上げました！やばすぎる！", score);
        }

        string linkUrl = "https://sgtkraft.github.io/oju-10seconds/";   // ツイートに挿入するURL
        string hashtags = "すがたくらふと,お重10Seconds,Unity";        // ツイートに挿入するハッシュタグ

        // ツイート画面を開く
        var url = "https://twitter.com/intent/tweet?"
            + "text=" + scoreText
            + "&url=" + linkUrl
            + "&hashtags=" + hashtags;

        Debug.Log(url);

#if UNITY_EDITOR
        Application.OpenURL(url);
#elif UNITY_WEBGL
        // WebGLの場合は、ゲームプレイ画面と同じウィンドウでツイート画面が開かないよう、処理を変える
        Application.ExternalEval(string.Format("window.open('{0}','_blank')", url));
#else
        Application.OpenURL(url);
#endif
    }

    public void OnRetryButtonClicked()
    {
        state = State.TOSTANDBY;
        animator.Play("Retry");
    }

    public void Retry()
    {
        timerImage.enabled = false;
        scoreTitle.SetActive(false);
        shareButton.SetActive(false);
        retryButton.SetActive(false);

        OnStandby();
    }
}
