using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Rigidbody2D rb;

    //[SerializeField]
    //AnimationCurve curve;

    public float timer = 0f;
    private Vector3 defaultPos;

    public float interval = 0.2f;
    private bool isTouched, isTouchedHigh;

	// Use this for initialization
	void Awake ()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultPos = transform.position;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (GameController.gameState == GameController.GameState.FINISH)
        {
            if (transform.position.y > defaultPos.y) { SetVelocity(-4); }
            else
            {
                SetVelocity(0);
                GameController.gameState = GameController.GameState.RESULT;
            }
        }

        if (GameController.gameState != GameController.GameState.PLAY) { return; }

        timer += Time.deltaTime;
        if (timer >= interval)
        {
            isTouched = Physics2D.Linecast(
                new Vector2(-3, transform.position.y - 3),
                new Vector2(3, transform.position.y - 3),
                1 << (int)GameController.LayerName.OjuGrounded
            );
            isTouchedHigh = Physics2D.Linecast(
                new Vector2(-3, transform.position.y),
                new Vector2(3, transform.position.y),
                1 << (int)GameController.LayerName.OjuGrounded
            );
            timer = 0f;
        }

        if (isTouchedHigh) { SetVelocity(4); }
        else if (isTouched) { SetVelocity(2); }
        else if (transform.position.y > defaultPos.y) { SetVelocity(-6); }
        else { SetVelocity(0); }
	}

    private void SetVelocity(int speed)
    {
        rb.velocity = new Vector2(0f, speed);
    }
}
