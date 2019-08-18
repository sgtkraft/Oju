using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OsechiController : MonoBehaviour
{
    private Rigidbody2D rb;
    private float timer = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

	// Update is called once per frame
	void Update ()
    {
        if (GameController.gameState == GameController.GameState.TITLE)
        {
            timer += Time.deltaTime;
            if (timer > 3f) { gameObject.layer = (int)GameController.LayerName.OsechiTransparent; }
        }

        if (GameController.gameState == GameController.GameState.TOSTANDBY)
        {
            gameObject.layer = (int)GameController.LayerName.OsechiTransparent;
            rb.velocity = new Vector2(8f, 0);
        }

        if (GameController.gameState == GameController.GameState.FINISH)
        {
            rb.bodyType = RigidbodyType2D.Static;
        }

        if (transform.position.y < -8)
        {
            Destroy(gameObject);
        }
	}
}
