using UnityEngine;
using UnityEditor;

public class PlayerPrefsHelper : MonoBehaviour
{
    [MenuItem("Tools/Reset Progress (Clear PlayerPrefs)")]
    public static void ResetPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("PlayerPrefs berhasil dihapus! Semua level terkunci kecuali Level 1.");
    }
}
