using RpgAtsumaruApiForUnity;
using MaterialUI;
using NCMB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankController : MonoBehaviour
{
    public GameController gc;

    public TextMeshProUGUI titleText, myRankText, myNameValueText, myScoreText;
    public MaterialDropdown[] nameDropdowns = new MaterialDropdown[3];

    public RankCell tempCell;
    public RectTransform listParentRt;
    public Animator listAnimator;

    private List<RankCell> cellList = new List<RankCell>();
    private bool needFetch;

    private int showHash = Animator.StringToHash("Show");
    private int defaultHash = Animator.StringToHash("Default");

    // ハイスコアボードID(アツマール用)
#if OJU_ATSUMARU
    private int highScoreBoardId = 1;
#endif

    private void Awake()
    {
        needFetch = true;

#if OJU_ATSUMARU
        titleText.SetText("これまでのハイスコア");
        myNameValueText.transform.parent.gameObject.SetActive(true);
        myScoreText.transform.parent.gameObject.SetActive(true);
        nameDropdowns[0].transform.parent.gameObject.SetActive(false);
#else
        titleText.SetText("今回のスコア");
        myNameValueText.transform.parent.gameObject.SetActive(false);
        myScoreText.transform.parent.gameObject.SetActive(false);
        nameDropdowns[0].transform.parent.gameObject.SetActive(true);
#endif
    }

    // Update is called once per frame
    private void Update()
    {
        switch (gc.state)
        {
            case GameController.State.TORANK:
                if (!needFetch) { return; }
                FetchData();
                needFetch = false;
                break;

            default:
                if (!needFetch) { needFetch = true; }
                break;
        }
    }

    /// <summary>
    /// スコア登録/更新
    /// </summary>
    public void RegisterData()
    {
#if OJU_ATSUMARU
        RegisterData4Atsumaru();
#else
        RegisterData4Ncmb();
#endif
    }

    /// <summary>
    /// ランク情報取得
    /// </summary>
    public void FetchData()
    {
#if OJU_ATSUMARU
        FetchRank4Atsumaru();
#else
        FetchRank4Ncmb(gc.score);
        FetchTopData4Ncmb();
#endif
    }

    /// <summary>
    /// スコア登録(NCMB)
    /// </summary>
    private void RegisterData4Ncmb()
    {
        string name = string.Empty;
        foreach (MaterialDropdown dropdown in nameDropdowns)
        {
            name += dropdown.buttonTextContent.text;
        }
        int score = gc.score;

        NCMBObject obj = new NCMBObject("HighScore");
        obj["name"] = name;
        obj["score"] = score;
        obj.SaveAsync((NCMBException e) =>
        {
            if (e != null)
            {
                // 登録失敗時の処理
                ToastManager.Show("スコア登録に失敗しました");
            }
            else
            {
                // 登録成功時の処理
                ToastManager.Show("スコアを登録しました");

                // リスト非表示
                listAnimator.Play(defaultHash);
                FetchTopData4Ncmb();
            }
        });
    }

    /// <summary>
    /// NCMBからランク情報を取得
    /// </summary>
    private void FetchRank4Ncmb(int score)
    {
        // データスコアの「HighScore」から検索
        NCMBQuery<NCMBObject> rankQuery = new NCMBQuery<NCMBObject>("HighScore");
        rankQuery.WhereGreaterThan("score", score);

        rankQuery.CountAsync((int count, NCMBException e) =>
        {
            if (e != null)
            {
                // 順位取得失敗
                ToastManager.Show("データの取得に失敗しました");
                myRankText.SetText("---");
                gc.canRegister = false;
            }
            else
            {
                // 順位取得成功
                // ランクが閾値を下回っていなければ登録可能
                int rank = count + 1;
                if (rank <= gc.rankBorder)
                {
                    myRankText.SetText(rank.ToString());
                    gc.canRegister = true;
                }
                else
                {
                    myRankText.SetText(string.Format("{0}+", gc.rankBorder));
                    gc.canRegister = false;
                }
            }
        });
    }

    /// <summary>
    /// NCMBからTOP100情報を取得
    /// </summary>
    private void FetchTopData4Ncmb()
    {
        // データスコアの「HighScore」から検索
        NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>("HighScore");

        // Scoreフィールドの降順でデータを取得
        query.OrderByDescending("score");

        // 検索件数を100件に設定
        query.Limit = 100;

        // データストアでの検索を行う
        query.FindAsync((List<NCMBObject> objList, NCMBException e) =>
        {
            if (e != null)
            {
                // 検索失敗時の処理
                ToastManager.Show("データの取得に失敗しました");
            }
            else
            {
                // 検索成功時の処理
                foreach (RankCell cell in cellList)
                {
                    Destroy(cell.gameObject);
                }
                cellList.Clear();

                for (int i = 0; i < objList.Count; i++)
                {
                    string name = Convert.ToString(objList[i]["name"]);
                    int score = Convert.ToInt32(objList[i]["score"]);

                    RankCell cell = Instantiate(tempCell, listParentRt);
                    cell.Init(i + 1, name, score);
                    cellList.Add(cell);
                }
            }
        });

        // リスト表示
        listAnimator.Play(showHash, 0, 0.0f);
    }

    /// <summary>
    /// スコア登録(アツマール)
    /// </summary>
    private void RegisterData4Atsumaru()
    {
        // スコアを登録
        gc.am.SendScore(highScoreBoardId, gc.score);
    }

    /// <summary>
    /// アツマールからランク情報を取得
    /// </summary>
    private void FetchRank4Atsumaru()
    {
#if OJU_ATSUMARU
        myRankText.SetText("---");
        myNameValueText.SetText("？？？");
        myScoreText.SetText("0");

        // 更新成功時の処理
        gc.am.GetScoreboardData(highScoreBoardId, (data, isError2) =>
        {
            if (isError2)
            {
                // 取得失敗時の処理
                ToastManager.Show("データ取得に失敗しました");

                // リスト表示
                listAnimator.Play(showHash, 0, 0.0f);
            }
            else
            {
                // 取得成功時の処理
                // ユーザー名取得
                if (data.myBestRecor.Available)
                {
                    if (data.myBestRecor.rank <= gc.rankBorder)
                    {
                        myRankText.SetText(data.myBestRecor.rank.ToString());
                    }
                    else
                    {
                        myRankText.SetText(string.Format("{0}+", gc.rankBorder.ToString()));
                    }
                    myNameValueText.SetText(string.Format("{0}", data.myBestRecor.userName));
                    myScoreText.SetText(data.myBestRecor.score.ToString());
                }

                // リストリセット
                foreach (RankCell cell in cellList)
                {
                    Destroy(cell.gameObject);
                }
                cellList.Clear();

                // ランク取得
                for (int count = 0; count < data.ranking.Length; count++)
                {
                    RpgAtsumaruRanking rankData = data.ranking[count];

                    int curRank = count + 1;
                    string curName = string.Format("{0}", rankData.userName);
                    int curScore = (int)rankData.score;

                    RankCell cell = Instantiate(tempCell, listParentRt);
                    cell.Init(curRank, curName, curScore);
                    cellList.Add(cell);
                }

                // リスト表示
                listAnimator.Play(showHash, 0, 0.0f);
            }
        });
#endif
    }
}
