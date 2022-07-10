using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConnectionManager : MonoBehaviour
{
    [SerializeField]
    private GameObject connectionButton;

    public void Start()
    {
        PlayGamesPlatform.Activate();
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    internal void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            Social.ReportProgress(GPGSIds.achievement_bienvenue__kheg, 100.0f, null);

            PlayerProfile.pseudo = Social.localUser.userName;

            if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                var callback = new PermissionCallbacks();
                callback.PermissionDenied += PermissionDenied;
                callback.PermissionGranted += PermissionGranted;
                Permission.RequestUserPermission(Permission.FineLocation, callback);
            }
            else
            {
                SceneManager.LoadScene("MapScene");
            }
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


    internal void PermissionDenied(string permissionName)
    {
        Application.Quit();
    }

    internal void PermissionGranted(string permissionName)
    {
        SceneManager.LoadScene("MapScene");
    }

    internal void PermissionDeniedAndDontAskAgain(string permissionName)
    {
        Application.Quit();
    }
}
