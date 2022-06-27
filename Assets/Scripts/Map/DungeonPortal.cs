using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonPortal : MonoBehaviour
{
    public void EnterDungeon()
    {
        SceneManager.LoadScene("DungeonScene");
    }

    public void ShowInfo()
    {
        SceneManager.LoadScene("DungeonScene");
    }
}
