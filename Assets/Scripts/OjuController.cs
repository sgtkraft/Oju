using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OjuController : MonoBehaviour
{
    private float timer = 0f;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > 1f)
        {
            gameObject.layer = (int)GameController.LayerName.OjuGrounded;
        }

        //if (GameController.gameState == GameController.GameState.RESULT)
        //{
        //    if(gameObject.layer == (int)GameController.LayerName.OjuGrounded)
        //    {
        //        GetComponent<Renderer>().material.color = Color.red;
        //    }
        //}
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        gameObject.tag = "Untagged";
        GetComponent<Renderer>().material.color = Color.red;
        if (collision.gameObject.layer == (int)GameController.LayerName.Obon ||
            collision.gameObject.tag == "Scored")
        {
            gameObject.tag = "Scored";
            GetComponent<Renderer>().material.color = Color.yellow;
        }
    }
}
