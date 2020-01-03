#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
public class EditorExWindow : EditorWindow
{
    [MenuItem("Window/EditorEx")]
    static void Open()
    {
        EditorWindow.GetWindow<EditorExWindow>("EditorEx");
    }
    bool toggle;
    bool toggleLeft;
    bool foldout;
    string textField = "";
    string textArea = "";
    string password = "";
    int intField = 0;
    int intSlider = 0;
    float floatField = 0.0f;
    float slider = 0.0f;
    float minMaxSliderMinValue = 20.0f;
    float minMaxSliderMaxValue = 50.0f;
    int popup = 0;
    int intPopup = 0;
    public enum EnumPopup
    {
        Enum1,
        Enum2,
        Enum3
    }
    EnumPopup enumPopup = EnumPopup.Enum1;
    int maskField = 0;
    EnumPopup enumMaskField = 0;
    int layer = 0;
    string tag = "";
    Vector2 vector2Field = Vector2.zero;
    Vector3 vector3Field = Vector3.zero;
    Vector4 vector4Field = Vector4.zero;
    Rect rectField;
    Color colorField = Color.white;
    Bounds boundsField;
    AnimationCurve curveField = AnimationCurve.Linear(0.0f, 0.0f, 60.0f, 1.0f);
    Object objectField = null;
    bool inspectorTitlebar = false;
    void OnGUI()
    {
        EditorGUILayout.LabelField("ようこそ！　Unityエディタ拡張の沼へ！"); // やっぱり残しておこう。
        EditorGUILayout.PrefixLabel("PrefixLabel : EditorGUILayout");
        EditorGUILayout.LabelField("LabelField", "EditorGUILayoutはEditor拡張用に調整されてる系");
        EditorGUILayout.SelectableLabel("SelectableLabel : 選択してコピペできる。\n変更はできない");
        toggle = EditorGUILayout.Toggle("Toggle", toggle);
        toggleLeft = EditorGUILayout.ToggleLeft("ToggleLeft", toggleLeft);
        foldout = EditorGUILayout.Foldout(foldout, "Foldout");
        if (foldout)
        {
            EditorGUILayout.LabelField("ﾁﾗｯ");
        }
        textField = EditorGUILayout.TextField("TextField", textField);
        textArea = EditorGUILayout.TextArea(textArea);
        password = EditorGUILayout.PasswordField("PasswordField", password);
        intField = EditorGUILayout.IntField("IntField", intField);
        intSlider = EditorGUILayout.IntSlider("IntSlider", intSlider, 0, 100);
        floatField = EditorGUILayout.FloatField("FloatField", floatField);
        slider = EditorGUILayout.Slider("Slider", slider, 0.0f, 100.0f);
        EditorGUILayout.MinMaxSlider(new GUIContent("MinMaxSlider"), ref minMaxSliderMinValue, ref minMaxSliderMaxValue, 0.0f, 100.0f);
        EditorGUILayout.LabelField("MinValue = ", minMaxSliderMinValue.ToString());
        EditorGUILayout.LabelField("MaxValue = ", minMaxSliderMaxValue.ToString());
        popup = EditorGUILayout.Popup("Popup", popup, new string[] { "Index 0", "Index 1", "Index 2" });
        EditorGUILayout.LabelField("Popup = ", popup.ToString());
        intPopup = EditorGUILayout.IntPopup("IntPopup", intPopup, new string[] { "Index 0", "Index 1", "Index 2" }, new int[] { 0, 3, 99 });
        EditorGUILayout.LabelField("IntPopup = ", intPopup.ToString());
        enumPopup = (EnumPopup)EditorGUILayout.EnumPopup("EnumPopup", (System.Enum)enumPopup);
        maskField = EditorGUILayout.MaskField("MaskField", maskField, new string[] { "Mask 1", "Mask 2", "Mask 3" });
        enumMaskField = (EnumPopup)EditorGUILayout.EnumMaskField("EnumMaskField", (System.Enum)enumMaskField);
        layer = EditorGUILayout.LayerField("LayerField", layer);
        tag = EditorGUILayout.TagField("TagField", tag);
        vector2Field = EditorGUILayout.Vector2Field("Vector2Field", vector2Field);
        vector3Field = EditorGUILayout.Vector3Field("Vector3Field", vector3Field);
        vector4Field = EditorGUILayout.Vector3Field("Vector4Field", vector4Field);
        rectField = EditorGUILayout.RectField("RectField", rectField);
        colorField = EditorGUILayout.ColorField("ColorField", colorField);
        boundsField = EditorGUILayout.BoundsField("BoundsField", boundsField);
        curveField = EditorGUILayout.CurveField("CurveField", curveField);
        objectField = EditorGUILayout.ObjectField("ObjectField", objectField, typeof(Object), true);
        if (objectField != null)
        {
            inspectorTitlebar = EditorGUILayout.InspectorTitlebar(inspectorTitlebar, objectField);
            if (inspectorTitlebar)
            {
                EditorGUILayout.LabelField("ﾁﾗｯﾁﾗｯ");
            }
        }
        EditorGUILayout.LabelField("ここからSpace");
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("ここまでSpace");
        EditorGUILayout.HelpBox("Heeeeeelllllp!!!!!", MessageType.Warning);
    }
}
#endif