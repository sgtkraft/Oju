using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OsechiController : MonoBehaviour
{
    public GameController gc;

    private Rigidbody2D rb;
    private float timer = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

	// Update is called once per frame
    private void Update ()
    {
        switch (gc.state)
        {
            case GameController.State.TITLE:
                timer += Time.deltaTime;
                if (timer > 3f) { gameObject.layer = (int)GameController.Layer.OsechiTransparent; }
                break;
            case GameController.State.TOSTANDBY:
                gameObject.layer = (int)GameController.Layer.OsechiTransparent;
                rb.velocity = new Vector2(8f, 0);
                break;
            case GameController.State.SCORE:
                rb.bodyType = RigidbodyType2D.Static;
                break;
            case GameController.State.TOTITLE:
                Destroy(this.gameObject);
                break;
        }

        if (transform.position.y < -8)
        {
            Destroy(gameObject);
        }
	}
}
