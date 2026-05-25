using UnityEngine;
using UnityEditor;

public class ClearPlayerPrefs : EditorWindow
{
    [MenuItem("Tools/Clear PlayerPrefs (Reset Level Lock)")]
    public static void ClearPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("Semua PlayerPrefs telah dihapus! Game sekarang seperti baru dimainkan pertama kali. Level 2 harusnya sudah terkunci lagi.");
    }
}
