using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Xml.Linq;
using System;
using System.Runtime.InteropServices;

namespace TweetWithScreenShot
{
    public class TweetManager : MonoBehaviour
    {
        //JS関数の呼び出し
        [DllImport("__Internal")] private static extern void OpenNewWindow(string url);

        public UnityEngine.UI.Text debugTxt;
        private static TweetManager sinstance;
        public string[] hashTags;

        [SerializeField]
        private string clientID;

        public string ClientID
        {
            get
            {
                if (string.IsNullOrEmpty(clientID)) throw new Exception("ClientIDをセットしてください");
                return clientID;
            }
        }

        public static TweetManager Instance
        {
            get
            {
                if (sinstance == null)
                {
                    sinstance = FindObjectOfType<TweetManager>();
                    if (sinstance == null)
                    {
                        var obj = new GameObject(typeof(TweetManager).Name);
                        sinstance = obj.AddComponent<TweetManager>();
                    }
                }
                return sinstance;
            }
        }

        public static IEnumerator TweetWithScreenShot(string text)
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
                sinstance.debugTxt.text = www.error;
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

            text += " " + UploadedURL;
            string hashtags = "&hashtags=";
            if (sinstance.hashTags.Length > 0)
            {
                hashtags += string.Join (",", sinstance.hashTags);
            }

            // Twitter投稿用URL
            string TweetURL = "https://twitter.com/intent/tweet?text=" + text;
            sinstance.debugTxt.text += ("\n" + TweetURL);

#if UNITY_EDITOR
            System.Diagnostics.Process.Start(TweetURL);
            sinstance.debugTxt.text += "\nEditor";
#elif UNITY_WEBGL
            Application.ExternalEval(string.Format("window.open('{0}','_blank')", TweetURL));
            sinstance.debugTxt.text += "\nWebGL";
#else
            Application.OpenURL(TweetURL);
            sinstance.debugTxt.text += "\nOther";
#endif
        }
    }
}
