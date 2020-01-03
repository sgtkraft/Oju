using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObonController : MonoBehaviour
{
    public GameController gc;

    public GameObject cursorGo;
    public float interval = 0.5f;

    private Rigidbody2D rb;
    private Vector3 defaultPos;
    private int direction;
    private float timer;
    public bool isRendered = false;

    // Use this for initialization
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultPos = transform.position;
        direction = (Random.Range(-1, 1) == 0) ? 1 : -1;
	}

    // Update is called once per frame
    void Update()
    {
        switch (gc.state)
        {
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
            case GameController.State.SCORE:
                rb.bodyType = RigidbodyType2D.Static;
                ShowCursor();
                break;
            case GameController.State.TOTITLE:
                transform.position = defaultPos;
                break;
            default:
                rb.bodyType = RigidbodyType2D.Static;
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
        if (Camera.current.name == "MainCamera" && Camera.current.name != "SceneCamera")
        {
            isRendered = true;
        }
    }

    private void ShowCursor()
    {
        cursorGo.SetActive(!isRendered);
        cursorGo.GetComponent<RectTransform>().anchoredPosition = new Vector3(RectTransformUtility.WorldToScreenPoint(Camera.main, transform.localPosition).x, 0f, 0f);
        //cursorGo.transform.position = new Vector3(transform.position.x, 0f, 0f);
        isRendered = false;
    }
}
