using GooglePlayGames;
using GooglePlayGames.BasicApi;
using RunPG.Multi;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConnectionManager : MonoBehaviour
{
    [SerializeField]
    private GameObject signupButton;

    [SerializeField]
    private CanvasGroup connectionGroup;
    [SerializeField]
    private CanvasGroup classGroup;

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

            PlayerProfile.pseudo = Social.localUser.userName;
            PlayerProfile.guid = Social.localUser.id;

            if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                var callback = new PermissionCallbacks();
                callback.PermissionDenied += PermissionDenied;
                callback.PermissionGranted += PermissionGranted;
                Permission.RequestUserPermission(Permission.FineLocation, callback);
            }
            else
            {
                var user = await Requests.GETUserByName(PlayerProfile.pseudo);
                if (user != null)
                {
                    PlayerProfile.id = user.id;
                    PlayerProfile.character = await Requests.GETUserCharacter(PlayerProfile.id);
                    SceneManager.LoadScene("MapScene");
                }
                else
                {
                    signupButton.SetActive(true);
                }
            }
        }
        else
        {
            if (Application.isEditor)
            {
                PlayerProfile.pseudo = "UnityEditor";
                PlayerProfile.guid = "UnityEditor";
                var user = await Requests.GETUserByName("UnityEditor");
                if (user != null)
                {
                    PlayerProfile.id = user.id;
                    PlayerProfile.character = await Requests.GETUserCharacter(PlayerProfile.id);
                    SceneManager.LoadScene("MapScene");
                }
                else
                {
                    signupButton.SetActive(true);
                }
            }
            else
            {
                Debug.LogError("Can't connect to Google Play Games");
            }
        }
    }

    public void SignUp()
    {
        connectionGroup.alpha = 0;
        connectionGroup.interactable = false;
        connectionGroup.blocksRaycasts = false;
        classGroup.alpha = 1;
        classGroup.interactable = true;
        classGroup.blocksRaycasts = true;
    }


    internal void PermissionDenied(string permissionName)
    {
        Application.Quit();
    }

    internal async void PermissionGranted(string permissionName)
    {
        var user = await Requests.GETUserByName(PlayerProfile.pseudo);
        if (user != null)
        {
            PlayerProfile.id = user.id;
            PlayerProfile.character = await Requests.GETUserCharacter(PlayerProfile.id);
            SceneManager.LoadScene("MapScene");
        }
        else
        {
            signupButton.SetActive(true);
        }
    }

    internal void PermissionDeniedAndDontAskAgain(string permissionName)
    {
        Application.Quit();
    }
}
