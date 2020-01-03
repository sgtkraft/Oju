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
	private void Awake ()
    {
        prefab.GetComponent<OjuController>().Init(gc);
    }
	
	// Update is called once per frame
	private void Update ()
    {
        if (gc.state == GameController.State.STANDBY ||
            gc.state == GameController.State.PLAY)
        {
            cursor.SetActive(true);
            cursor.transform.position = GetCursorPosition();
        }
        else
        {
            cursor.SetActive(false);
        }

        // マウス入力で左クリックをした瞬間
        if (Input.GetMouseButtonDown(0))
        {
            if (gc.state == GameController.State.TITLE)
            {
                clickPosition = Input.mousePosition;
                clickPosition.z = 10f;
                clickPosition = Camera.main.ScreenToWorldPoint(clickPosition);

                if (clickPosition.y > -1f)
                {
                    Instantiate(prefab, clickPosition, prefab.transform.rotation, prefabParent);
                }
            }

            if (gc.state == GameController.State.PLAY)
            {
                // オブジェクト生成 : オブジェクト(GameObject), 位置(Vector3), 角度(Quaternion)
                // ScreenToWorldPoint(位置(Vector3))：スクリーン座標をワールド座標に変換する
                Instantiate(prefab, GetCursorPosition(), prefab.transform.rotation, prefabParent);
            }
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
