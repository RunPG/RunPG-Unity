using GooglePlayGames;
using GooglePlayGames.BasicApi;
using RunPG.Multi;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        PlayGamesPlatform.Activate();
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthenticationAsync);
    }

    internal async void ProcessAuthenticationAsync(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            Social.ReportProgress(GPGSIds.achievement_bienvenue__kheg, 100.0f, null);

            var username = Social.localUser.userName;
            PlayerProfile.pseudo = username;
            var user = await Requests.GETUserByName(username);
            if (user != null)
            {
                PlayerProfile.id = user.id;
            }

            SceneManager.LoadScene("MapScene");
        }
        else
        {
            connectionButton.SetActive(true);
        }
    }

    public void Connect()
    {
        PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthenticationAsync);
    }
}
