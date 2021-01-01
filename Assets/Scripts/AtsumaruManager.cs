﻿using RpgAtsumaruApiForUnity;
using UnityEngine;

public class AtsumaruManager : MonoBehaviour
{
#if OJU_ATSUMARU
    // SyncSaveDataAsync() 関数を待機するためには、関数に async 定義をしなければなりません
    private async void Awake()
    {
        // もしプラグインの初期化が終わっていないなら
        if (!RpgAtsumaruApi.Initialized)
        {
            // プラグインの初期化
            RpgAtsumaruApi.Initialize();
        }

        // ストレージAPIを取得して、初回同期をする
        // 同期を必ず最初に行い、サーバーからデータを貰うようにして下さい
        await RpgAtsumaruApi.StorageApi.SyncSaveDataAsync();
    }
#endif

    /// <summary>
    /// データ入出力
    /// </summary>

    // ゲームのシステムデータを取得する
    public string GetSystemData()
    {
        // システムデータをゲームロジックに返す
        return RpgAtsumaruApi.StorageApi.GetSystemData();
    }

    // ゲームのシステムデータをセーブ
    public async void SaveSystemData(string systemData)
    {
        // システムデータを設定してサーバーと同期する
        RpgAtsumaruApi.StorageApi.SetSystemData(systemData);
        await RpgAtsumaruApi.StorageApi.SyncSaveDataAsync();
    }

    // 指定されたスロット番号のセーブデータを取得する
    public string GetGameSaveData(int slotNumber)
    {
        // セーブデータを返す
        return RpgAtsumaruApi.StorageApi.GetSaveData(slotNumber);
    }

    // 指定されたスロット番号にセーブデータをセーブする
    public async void SaveGameData(int slotNumber, string saveData)
    {
        // セーブデータを設定してサーバーと同期する
        RpgAtsumaruApi.StorageApi.SetSaveData(slotNumber, saveData);
        await RpgAtsumaruApi.StorageApi.SyncSaveDataAsync();
    }

    // セーブデータが存在するスロット番号を全て取得する
    public int[] GetAvailableSaveDataSlotNumbers()
    {
        // セーブデータの有効な番号を全て返す
        return RpgAtsumaruApi.StorageApi.GetAllSaveDataSlotId();
    }

    /// <summary>
    /// コメント制御
    /// </summary>

    // ゲーム内の場面「シーン」を切り替える
    // さらに、その場面が全体の流れの最初に戻ることがあるのなら、リセットする
    public void ChangeScene(string sceneName, bool reset)
    {
        // コメントAPIにシーン名とリセットするかどうかのフラグを渡してシーンの切り替えをする
        RpgAtsumaruApi.CommentApi.ChangeScene(sceneName, reset);
    }

    // ゲーム内のイベントをトリガーします（会話が始まった、宝箱を調べた、何かのアクションが実行された、等）
    public void OnEventRaised(string eventName)
    {
        // ゲーム内で起きたイベントをRPGアツマールに通知します
        RpgAtsumaruApi.CommentApi.SetContext(eventName);
    }

    // ゲーム内で発生したイベントのステップを実行します（会話の選択肢で "はい", "いいえ" を選んだ、アイテムを所持 "している", "していない"、等）
    public void OnEventStep(string stepName)
    {
        // トリガーされたイベントのステップを通知します
        RpgAtsumaruApi.CommentApi.PushContextFactor(stepName);
    }

    // イベントのステップ内で起きた小さなステップを実行します（次の会話に続いた、アクションタイマーが経過した、等）
    public void OnEventSubStep()
    {
        // イベントのステップの進行を通知します
        RpgAtsumaruApi.CommentApi.PushMinorContext();
    }

    /// <summary>
    /// 外部リンク誘導
    /// </summary>

    public async void OpenLink(string url)
    {
        // 外部リンクを開いてRPGアツマールから結果を受け取るまで待機（表示完了の待機ではありません）
        await RpgAtsumaruApi.GeneralApi.OpenLinkAsync(url);
    }
}
