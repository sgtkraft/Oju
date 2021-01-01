using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameController gc;

    public float interval = 0.2f;
    public bool isTouched, isTouchedHigh;

    private Rigidbody2D rb;
    private float timer = 0f;
    private Vector3 defaultPos;
    private bool isScored;

	// Use this for initialization
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultPos = transform.position;
        isScored = false;
	}
	
	// Update is called once per frame
    private void Update()
    {
        switch (gc.state)
        {
            case GameController.State.PLAY:
                timer += Time.deltaTime;
                if (timer >= interval)
                {
                    isTouched = Physics2D.Linecast(
                        new Vector2(-3, transform.position.y - 2),
                        new Vector2(3, transform.position.y - 2),
                        1 << (int)GameController.Layer.OjuGrounded
                    );
                    isTouchedHigh = Physics2D.Linecast(
                        new Vector2(-3, transform.position.y),
                        new Vector2(3, transform.position.y),
                        1 << (int)GameController.Layer.OjuGrounded
                    );
                    timer = 0f;
                }

                if (isTouched && isTouchedHigh) { SetVelocity(5); } // カメラとお重の相対高によって移動速度を変化
                else if (isTouched) { SetVelocity(3); }
                else if (transform.position.y > defaultPos.y) { SetVelocity(-6); }
                else { SetVelocity(0); }
                break;
            case GameController.State.TOSCORE: // プレイ終了後、初期位置までカメラを移動させる
                if (transform.position.y > defaultPos.y)
                {
                    SetVelocity(-6);
                }
                else
                {
                    SetVelocity(0);
                    transform.position = defaultPos;
                    if(!isScored)
                    {
                        isScored = true;
                        gc.OnScore();
                    }
                }
                break;
            case GameController.State.SCORE: // お重とラインが重ならなくなるまでカメラを移動させる
            case GameController.State.TORANK:
                isTouched = Physics2D.Linecast(
                    new Vector2(-3, transform.position.y + 2),
                    new Vector2(3, transform.position.y + 2),
                    1 << (int)GameController.Layer.ScoreLine
                );

                if (isTouched) { SetVelocity(4); }
                else { SetVelocity(0); }
                break;
            //case GameController.State.TOTITLE:
            default:
                SetVelocity(0);
                transform.position = defaultPos;
                isTouched = false;
                isTouchedHigh = false;
                isScored = false;
                break;
        }
	}

    private void SetVelocity(int speed)
    {
        rb.velocity = new Vector2(0f, speed);
    }
}
