using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObonController : MonoBehaviour
{
    private Rigidbody2D rb;

    private float timer;
    public float interval = 0.5f;
    private int direction;

	// Use this for initialization
	void Awake ()
    {
        rb = GetComponent<Rigidbody2D>();
        direction = (Random.Range(-1, 1) == 0) ? 1 : -1;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (GameController.gameState == GameController.GameState.PLAY)
        {
            timer += Time.deltaTime;
            if (timer >= interval || transform.position.x < -1.45f || transform.position.x > 1.45f)
            {
                direction *= -1;
                timer = 0f;
            }
            SetVelocity();
        }

        if (GameController.gameState == GameController.GameState.FINISH)
        {
            rb.bodyType = RigidbodyType2D.Static;
        }
	}

    private void SetVelocity()
    {
        rb.velocity = new Vector2(Random.Range(0, 4) * direction, 0f);
    }
}
