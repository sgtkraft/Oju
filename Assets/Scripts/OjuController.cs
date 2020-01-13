using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonDefine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class OjuController : MonoBehaviour
{
    public GameController gc;

    public Vector3[] corners = new Vector3[4];

    private RectTransform rt;
    private Material material;

    private float timer = 0f;

    public void Init(GameController gc)
    {
        this.gc = gc;
        foreach (OsechiController oc in GetComponentsInChildren<OsechiController>())
        {
            oc.gc = gc;
        }
    }

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        material = GetComponent<Renderer>().material;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > 1f)
        {
            //gameObject.layer = (int)GameController.Layer.OjuGrounded;
        }

        switch(gc.state)
        {
            case GameController.State.SCORE:
                if (gameObject.layer == (int)GameController.Layer.OjuGrounded)
                {
                    material.color = ColorDef.red;
                }
                break;
            default:
                break;
        }

        if (rt != null)
        {
            rt.GetWorldCorners(corners);
        }
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    //if (collision.gameObject.layer == (int)GameController.Layer.Obon ||
    //    //    collision.gameObject.layer == (int)GameController.Layer.OjuGrounded ||
    //    //    collision.gameObject.tag == "Scored")
    //    {
    //        gameObject.layer = (int)GameController.Layer.OjuGrounded;
    //        material.color = ColorDef.red;
    //    }
    //}

    private void OnCollisionStay2D(Collision2D collision)
    {
        gameObject.layer = (int)GameController.Layer.OjuGrounded;
        //material.color = ColorDef.red;
        //if (collision.gameObject.layer == (int)GameController.Layer.Obon ||
        //    collision.gameObject.layer == (int)GameController.Layer.OjuGrounded ||
        //    collision.gameObject.tag == "Scored")
        //{
        //    gameObject.tag = "Scored";
        //    material.color = ColorDef.gold;
        //    gameObject.layer = (int)GameController.Layer.OjuGrounded;
        //}
        //else
        //{
        //    gameObject.tag = "Untagged";
        //    material.color = Color.white;
        //    gameObject.layer = (int)GameController.Layer.Oju;
        //}
    }

    //private void OnCollisionExit2D(Collision2D collision)
    //{
    //    gameObject.layer = (int)GameController.Layer.Oju;
    //    material.color = Color.white;
    //}
}

#if UNITY_EDITOR
[CustomEditor(typeof(OjuController))]
public class OjuEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var oju = target as OjuController;

        oju.corners[2] = EditorGUILayout.Vector3Field("Rgt Top Pos", oju.corners[2]);
        oju.corners[3] = EditorGUILayout.Vector3Field("Rgt Btm Pos", oju.corners[3]);
        oju.corners[1] = EditorGUILayout.Vector3Field("Lft Top Pos", oju.corners[1]);
        oju.corners[0] = EditorGUILayout.Vector3Field("Lft Btm Pos", oju.corners[0]);
    }
}
#endif
