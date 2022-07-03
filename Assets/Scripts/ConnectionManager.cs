using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConnectionManager : MonoBehaviour
{
    [SerializeField]
    private GameObject connectionButton;

    public void Start()
    {
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    internal void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            // Continue with Play Games Services
            Social.ReportProgress(GPGSIds.achievement_bienvenue__kheg, 100.0f, null);
            PlayGamesPlatform.Instance.RequestServerSideAccess(false, code => 
            {

            });
            SceneManager.LoadScene("DungeonScene");
        }
        else
        {
            connectionButton.SetActive(true);
        }
    }

    public void Connect()
    {
        PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication);
    }
}
