using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MaterialUI;
using TMPro;
using NCMB;

public class RankController : MonoBehaviour
{
    public GameController gc;

    public TextMeshProUGUI myRankText;
    public InputField nameInput;

    public RankCell tempCell;
    public RectTransform listParentRt;
    public Animator listAnimator;

    private List<RankCell> cellList = new List<RankCell>();
    private bool isDone;

    private int showHash = Animator.StringToHash("Show");
    private int defaultHash = Animator.StringToHash("Default");

    private void Awake()
    {
        isDone = false;
    }

    // Update is called once per frame
    private void Update()
    {
        switch (gc.state)
        {
            case GameController.State.TORANK:
                if (isDone) { return; }

                FetchRank(gc.score);
                FetchTopData();

                isDone = true;
                break;
            default:
                if (isDone) { isDone = false; }
                break;
        }
    }

    public void OnNameValueChanged()
    {
        // アルファベット小文字が入力されたら大文字にする
        nameInput.text = nameInput.text.ToUpper();
    }

    public void RegisterData()
    {
        string name = nameInput.text;
        int score = gc.score;

        NCMBObject obj = new NCMBObject("HighScore");
        obj["name"] = name;
        obj["score"] = score;
        obj.SaveAsync((NCMBException e) =>
        {
            if (e != null)
            {
                // 保存失敗時の処理
                ToastManager.Show("データの保存に失敗しました");
            }
            else
            {
                // 保存成功時の処理
                ToastManager.Show("データの保存に成功しました");
                // リスト非表示
                listAnimator.Play(defaultHash);
                FetchTopData();
            }
        });
    }

    public void FetchRank(int score)
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

    public void FetchTopData()
    {
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
}
