using CommonDefine;
using MaterialUI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using NCMB;

public class GameController : MonoBehaviour
{
    public enum State
    {
        TITLE,
        TOSTANDBY,
        STANDBY,
        PLAY,
        TOSCORE,
        SCORE,
        TORANK,
        RANK,
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

    public TextMeshProUGUI messageText;
    public GameObject waitImgGo, subMessage, scoreTitle, kumoMain, rankGo;

    public MaterialButton startBtn, quitBtn, retryBtn, shareBtn, registerBtn, endBtn;

    public int score, rankBorder = 999;
    public bool canRegister = true;

    [Space(8)]
    public Text debugText;
    public bool isDebugActive = false;

    private float timer = 10f;
    private bool isScored;

    private int ttl2StbHash = Animator.StringToHash("TitleToStandby");
    private int stb2PlyHash = Animator.StringToHash("StandbyToPlay");
    private int standbyHash = Animator.StringToHash("Standby");
    private int playHash = Animator.StringToHash("Play");
    private int finishHash = Animator.StringToHash("Finish");
    private int retryHash = Animator.StringToHash("Retry");
    private int rankHash = Animator.StringToHash("Rank");
    private int quitHash = Animator.StringToHash("Quit");

    private string startCountObjId = "JpNZ9fCRkg72BVTY";
    private string playCountObjId = "ljjHcsTPL6IoxTPf";
    private int playCount = 0;

#if OJU_ATSUMARU
    [HideInInspector] public AtsumaruManager am = null;
#endif

    // Use this for initialization
    private void Awake()
    {
        timerImage.enabled = false;

        messageText.SetText(string.Empty);
        subMessage.SetActive(false);
        waitImgGo.SetActive(false);
        scoreTitle.SetActive(false);
        rankGo.SetActive(false);

        quitBtn.gameObject.SetActive(false);
        shareBtn.gameObject.SetActive(false);
        retryBtn.gameObject.SetActive(false);

        kumoMain.SetActive(true);

        isDebugActive = debugText.enabled;

#if OJU_ATSUMARU
        am = GetComponent<AtsumaruManager>();
        registerBtn.gameObject.SetActive(false);
#else
        registerBtn.textText = "スコア登録";
        registerBtn.gameObject.SetActive(true);
#endif
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

        // プレイカウントをリセット
        playCount = 0;

#if OJU_ATSUMARU
        // シーンリセット
        am.ChangeScene("Main", true);
#endif
    }

    public void OnStartButtonClicked()
    {
        state = State.TOSTANDBY;
        animator.Play(ttl2StbHash);

        // プレイカウント加算
        playCount++;

#if OJU_ATSUMARU
        // イベントトリガー
        am.OnEventRaised("Start");
#elif !UNITY_EDITOR
        // ObjectIdをもとにデータ取得を行う
        NCMBObject obj = new NCMBObject("Count");
        obj.ObjectId = startCountObjId;
        obj.FetchAsync((NCMBException e) =>
        {
            if (e != null)
            {
                // 取得失敗時の処理
            }
            else
            {
                // 取得成功時の処理
                // スタートカウント加算
                obj.Increment("count");
                obj.SaveAsync();
            }
        });
#endif
    }

    public void OnStandby()
    {
        state = State.STANDBY;

        messageText.SetText(string.Empty);
        subMessage.SetActive(true);
        kumoMain.SetActive(false);

#if OJU_ATSUMARU
        // イベントトリガー
        am.OnEventRaised("Standby");
#endif
    }

    public void StartStandby()
    {
        subMessage.SetActive(false);
        waitImgGo.SetActive(true);
        animator.Play(stb2PlyHash);
        messageText.color = ColorDef.black;
        messageText.SetText("用\n\n意");
    }

    public void StopStandby()
    {
        animator.Play(standbyHash);
        messageText.SetText(string.Empty);
        subMessage.SetActive(true);
        waitImgGo.SetActive(false);
    }

    public void OnPlay()
    {
        state = State.PLAY;
        timerImage.enabled = true;
        timer = 10f;

        animator.Play(playHash);
        messageText.color = ColorDef.black;
        messageText.SetText("は\nじ\nめ");

        quitBtn.textText = "やめる";
        quitBtn.interactable = true;
        quitBtn.gameObject.SetActive(true);

#if OJU_ATSUMARU
        // イベントトリガー
        am.OnEventRaised("Play");
#endif
    }

    public void Finish()
    {
        state = State.TOSCORE;
        timerImage.enabled = false;

        animator.Play(finishHash);
        messageText.color = ColorDef.red;
        messageText.SetText("そこまで");

#if OJU_ATSUMARU
        // イベントトリガー
        am.OnEventRaised("Finish");
#endif
    }

    public void OnScore()
    {
        if (!isScored)
        {
            isScored = true;
            return;
        }
        state = State.SCORE;

        messageText.SetText(string.Empty);
        scoreTitle.SetActive(true);
    }

    public void Result()
    {
        quitBtn.textText = "おわる";

        shareBtn.interactable = true;
        shareBtn.gameObject.SetActive(true);

        retryBtn.interactable = true;
        retryBtn.gameObject.SetActive(true);

#if OJU_ATSUMARU
        // イベントトリガー
        am.OnEventRaised("Result");
#endif
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
        switch (state)
        {
            case State.SCORE:
                OnRankButtonClicked();
                animator.Play(rankHash);
                break;
            default:
                animator.Play(quitHash);
                break;
        }

#if !UNITY_EDITOR && !OJU_ATSUMARU
        // ObjectIdをもとにデータ取得を行う
        NCMBObject obj = new NCMBObject("Count");
        obj.ObjectId = playCountObjId;
        obj.FetchAsync((NCMBException e) =>
        {
            if (e != null)
            {
                // 取得失敗時の処理
            }
            else
            {
                // 取得成功時の処理
                // プレイカウント加算
                obj.Increment("count", playCount);
                obj.SaveAsync();
            }
        });
#endif
    }

    public void OnRankButtonClicked()
    {
        state = State.TORANK;

        rankGo.SetActive(true);

        quitBtn.gameObject.SetActive(false);
        shareBtn.gameObject.SetActive(false);
        retryBtn.gameObject.SetActive(false);
    }

    public void OnRank()
    {
        state = State.RANK;

        registerBtn.interactable = canRegister;
        endBtn.interactable = true;

#if OJU_ATSUMARU
        // イベントトリガー
        am.OnEventRaised("Rank");
#endif
    }

    public void OnBoardButtonClicked()
    {
#if OJU_ATSUMARU
        // イベントトリガー
        am.ShowScoreboard(1);
#endif
    }

    public void OnEndButtonClicked()
    {
        animator.Play(quitHash);
    }

    public void Quit()
    {
        state = State.TOTITLE;
        timerImage.enabled = false;
        isScored = false;

        messageText.SetText(string.Empty);
        subMessage.SetActive(false);
        waitImgGo.SetActive(false);
        scoreTitle.SetActive(false);
        rankGo.SetActive(false);

        quitBtn.gameObject.SetActive(false);
        shareBtn.gameObject.SetActive(false);
        retryBtn.gameObject.SetActive(false);

        kumoMain.SetActive(true);
    }

    public void OnRetryButtonClicked()
    {
        animator.Play(retryHash);

        // プレイカウント加算
        playCount++;
    }

    public void Retry()
    {
        state = State.TOSTANDBY;
        timerImage.enabled = false;
        isScored = false;

        messageText.SetText(string.Empty);
        waitImgGo.SetActive(false);
        scoreTitle.SetActive(false);

        quitBtn.gameObject.SetActive(false);
        shareBtn.gameObject.SetActive(false);
        retryBtn.gameObject.SetActive(false);

        kumoMain.SetActive(false);

#if OJU_ATSUMARU
        // イベントトリガー
        am.OnEventRaised("Retry");
#endif
    }
}
