using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameController gc;

    // 生成したいPrefab
    public GameObject prefab;
    public Transform prefabParent;

    // プレイ中のカーソル
    public GameObject cursor;
    private Vector3 cursorPosition;

    // クリックした位置座標
    private Vector3 clickPosition;

	// Use this for initialization
	private void Awake()
    {
        prefab.GetComponent<OjuController>().Init(gc);
    }
	
	// Update is called once per frame
	private void Update()
    {
        switch (gc.state)
        {
            case GameController.State.TITLE:
                if (Input.GetMouseButtonDown(0))
                {
                    clickPosition = Input.mousePosition;
                    clickPosition.z = 10f;
                    clickPosition = Camera.main.ScreenToWorldPoint(clickPosition);

                    if (clickPosition.y > -1f)
                    {
                        Instantiate(prefab, clickPosition, prefab.transform.rotation, prefabParent);
                    }
                }
                cursor.SetActive(false);
                break;
            case GameController.State.STANDBY:
                cursor.transform.position = GetCursorPosition();
                cursor.SetActive(true);
                break;
            case GameController.State.PLAY:
                if (Input.GetMouseButtonDown(0))
                {
                    Instantiate(prefab, GetCursorPosition(), prefab.transform.rotation, prefabParent);
                }
                cursor.transform.position = GetCursorPosition();
                cursor.SetActive(true);
                break;
            default:
                cursor.SetActive(false);
                break;
        }
	}

    private Vector3 GetCursorPosition()
    {
        Vector3 tempPosition = Input.mousePosition;
        // Z軸修正
        tempPosition.z = 10f;

        tempPosition = Camera.main.ScreenToWorldPoint(tempPosition);
        tempPosition.y = Camera.main.transform.position.y + 3f;

        return tempPosition;
    }
}