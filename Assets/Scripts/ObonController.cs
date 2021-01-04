using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObonController : MonoBehaviour
{
    public GameController gc;

    public GameObject cursorGo;
    public float interval = 0.5f;
    public bool isRendered = false;

    private Rigidbody2D rb;
    private RectTransform cursorRt;
    private Vector3 defaultPos;
    private int direction;
    private float timer;

    // Use this for initialization
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cursorRt = cursorGo.GetComponent<RectTransform>();
        defaultPos = transform.position;
        direction = (Random.Range(-1, 1) == 0) ? 1 : -1;
    }

    // Update is called once per frame
    private void Update()
    {
        switch (gc.state)
        {
            //case GameController.State.TITLE:
            //case GameController.State.TOSTANDBY:
            //    transform.position = defaultPos;
            //    cursorGo.SetActive(false);
            //    break;
            case GameController.State.PLAY:
                timer += Time.deltaTime;
                if (timer >= interval || transform.position.x < -1.45f || transform.position.x > 1.45f)
                {
                    direction *= -1;
                    timer = 0f;
                }
                rb.bodyType = RigidbodyType2D.Dynamic;
                SetVelocity();
                ShowCursor();
                break;
            case GameController.State.TOSCORE:
                rb.bodyType = RigidbodyType2D.Static;
                ShowCursor();
                break;
            case GameController.State.SCORE:
            case GameController.State.TORANK:
                cursorGo.SetActive(false);
                break;
            default:
                rb.bodyType = RigidbodyType2D.Static;
                transform.position = defaultPos;
                cursorGo.SetActive(false);
                isRendered = false;
                break;
        }
    }

    private void SetVelocity()
    {
        rb.velocity = new Vector2(Random.Range(0, 4) * direction, 0f);
    }

    private void OnWillRenderObject()
    {
        //if (Camera.current.name == "MainCamera" && Camera.current.name != "SceneCamera")
        //{
        //    isRendered = true;
        //}
        isRendered = (Camera.current.name == "MainCamera");
    }

    private void ShowCursor()
    {
        cursorGo.SetActive(!isRendered);
        cursorRt.anchoredPosition = new Vector3(RectTransformUtility.WorldToScreenPoint(Camera.main, transform.localPosition).x / (Screen.width / 540.0f), 0f, 0f);
        isRendered = false;
        if (gc.isDebugActive) { gc.debugText.text = "IsRendered: " + isRendered + "\nCursor: " + cursorRt.anchoredPosition.x; }
    }
}
