﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Xml.Linq;
using System;
using System.Runtime.InteropServices;

public class ShareManager : MonoBehaviour
{
    // JS関数の呼び出し
    [DllImport("__Internal")] private static extern void OpenNewWindow(string url);

    [SerializeField] string clientID;

    [SerializeField] Text debugText;
    private static bool isDebugActive = false;

    private static ShareManager instance;

#if OJU_ATSUMARU
    private static AtsumaruManager am = null;
#endif

    public string ClientID
    {
        get
        {
            if (string.IsNullOrEmpty(clientID)) throw new Exception("ClientIDをセットしてください");
            return clientID;
        }
    }

    public static ShareManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ShareManager>();
                if (instance == null)
                {
                    var obj = new GameObject(typeof(ShareManager).Name);
                    instance = obj.AddComponent<ShareManager>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        isDebugActive = debugText.enabled;

#if OJU_ATSUMARU
        am = GetComponent<AtsumaruManager>();
#endif
    }

    public static IEnumerator TweetWithScreenShot(string str)
    {
        yield return new WaitForEndOfFrame();
        var tex = ScreenCapture.CaptureScreenshotAsTexture();

        // imgurへアップロード
        string UploadedURL = "";

        UnityWebRequest www;

        WWWForm wwwForm = new WWWForm();
        wwwForm.AddField("image", Convert.ToBase64String(tex.EncodeToJPG()));
        wwwForm.AddField("type", "base64");

        www = UnityWebRequest.Post("https://api.imgur.com/3/image.xml", wwwForm);

        www.SetRequestHeader("AUTHORIZATION", "Client-ID " + Instance.ClientID);

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Data: " + www.downloadHandler.text);
            XDocument xDoc = XDocument.Parse(www.downloadHandler.text);
            // TwitterCard用に拡張子を外す
            string url = xDoc.Element("data").Element("link").Value;
            url = url.Remove(url.Length - 4, 4);
            UploadedURL = url;
        }

        str += UploadedURL;
        //string hashtags = "&hashtags=";
        //if (sinstance.hashTags.Length > 0)
        //{
        //    hashtags += string.Join (",", sinstance.hashTags);
        //}

        // Twitter投稿用URL
        string tweetUrl = "https://twitter.com/intent/tweet?text=" + UnityWebRequest.EscapeURL(str);
        if (isDebugActive) { Instance.debugText.text += ("\n" + tweetUrl); }

#if UNITY_EDITOR
        System.Diagnostics.Process.Start(tweetUrl);
        if (isDebugActive) { Instance.debugText.text += "\nEditor"; }
#elif OJU_ATSUMARU
        am.OpenLink(tweetUrl);
        if (isDebugActive) { Instance.debugText.text += "\nAtsumaru"; }
#elif UNITY_WEBGL
        OpenNewWindow(tweetUrl);
        if (isDebugActive) { Instance.debugText.text += "\nWebGL"; }
#else
        Application.OpenURL(tweetUrl);
        if (isDebugActive) { Instance.debugText.text += "\nOther"; }
#endif
    }
}