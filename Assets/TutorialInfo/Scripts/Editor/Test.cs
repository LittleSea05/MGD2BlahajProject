using UnityEngine;
using UnityEditor;

public class GameProgressEditorTools
{
    [MenuItem("Tools/Reset Game Progress")]
    public static void ResetProgress()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("Game Progress Reset!");
    }
}
