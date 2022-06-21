using Newtonsoft.Json;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
namespace RunPG.Multi
{
    public static class Requests
    {
        static String rootUrl = "http://178.62.237.73/";
        public static int? GETPlayerID(String username, GameObject _errorMessage = null)
        {
            if (username.Length != 0)
            {
                using (UnityWebRequest request = UnityWebRequest.Get(rootUrl + "user/name/" + username))
                {
                    request.SendWebRequest();
                    while (!request.isDone)
                    {
                        //TODO change 
                        //waiting for request to be done
                    }
                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        Debug.Log(rootUrl + "user/name/" + username);
                        _errorMessage.SetActive(true);
                        _errorMessage.GetComponent<Text>().text = "User does not exist !";

                        return null;
                    }
                    else
                    {
                        var user = JsonUtility.FromJson<User>(request.downloadHandler.text);
                        return user.id;
                    }
                }
            }
            return null;
        }
        public static String GETPlayerName(int user_id, GameObject _errorMessage = null)
        {
            
                using (UnityWebRequest request = UnityWebRequest.Get(rootUrl + "user/" + user_id))
                {
                    request.SendWebRequest();
                    while (!request.isDone)
                    {
                        //TODO change 
                        //waiting for request to be done
                    }
                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        Debug.Log(request.result + ":" + user_id);
                            _errorMessage.SetActive(true);
                        if (_errorMessage)
                            _errorMessage.GetComponent<Text>().text = "User does not exist !";
                        else
                            Debug.Log("Error:" + rootUrl + "user/" + user_id);
                        return "";
                    }
                    else
                    {
                    var user = JsonUtility.FromJson<User>(request.downloadHandler.text);
                        return user.name;
                    }
                }        
        }
        public static Friend[] GETAllFriends(int user_id)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(rootUrl + "user/" + user_id + "/friend/"))
            {
                request.SendWebRequest();
                while (!request.isDone)
                {

                    //TODO change 
                    //waiting for request to be done
                }
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log("Cannot get friends");
                }
                else
                {
                    Debug.Log(rootUrl + "user/" + user_id + "/friend/");                 
                    var test = JsonConvert.DeserializeObject<Friendlist>(request.downloadHandler.text); 
                    return test.friends;
                }
            }         
            return null;
        }
        public static IEnumerator POSTNewUser(InputField username, GameObject _errorMessage = null)
        {
            Debug.Log(username.text);
            if (username.text.Length != 0)
            {
                var str = "{\"name\":\"" + username.text + "\"}";
                Debug.Log(str);
                using (UnityWebRequest request = UnityWebRequest.Post(rootUrl +"user/", "POST"))
                {
                    request.SetRequestHeader("Content-Type", "application/json");
                    request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(str)) as UploadHandler;
                    yield return request.SendWebRequest();
                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        _errorMessage.SetActive(true);
                        _errorMessage.GetComponent<Text>().text = "User already exist!";
                    }
                }
            }
        }
        public static IEnumerator POSTAddFriend(int userId, int friend_id)
        {
            // var str = "http://178.62.237.73/user/" + PhotonNetwork.NickName + "/friend/7";
            var str = rootUrl + "user/" + userId + "/friend/" + friend_id;
            Debug.Log(str);
            using (UnityWebRequest request = UnityWebRequest.Post(str, "POST"))
            {
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(request.error);
                    // _errorMessage.SetActive(true);
                    // _errorMessage.GetComponent<Text>().text = "User already exist!";
                }
            }
        }
        public static IEnumerator POSTSendNotification(int senderId, int receiver_id, NotificationType type)
        {
            var str = rootUrl + "user/" + receiver_id + "/notification/" + type + "/" + senderId;
            Debug.Log(str);
            using (UnityWebRequest request = UnityWebRequest.Post(str, "POST"))
            {
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(request.error);
                    // _errorMessage.SetActive(true);
                    // _errorMessage.GetComponent<Text>().text = "User already exist!";
                }
            }
        }
        public static IEnumerator DELETENotification(int userId, int friend_id)
        {
            // var str = "http://178.62.237.73/user/" + PhotonNetwork.NickName + "/friend/7";
            var str = rootUrl + "user/" + userId + "/friend/" + friend_id;
            Debug.Log(str);
            using (UnityWebRequest request = UnityWebRequest.Delete(str))
            {
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(request.error);
                }
            }
        }
        public static Notification[] GETNotificationsByType(int receiver_id, NotificationType type)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(rootUrl + "user/" + receiver_id + "/notification/" + type + "/"))
            {
                request.SendWebRequest();
                while (!request.isDone)
                {

                    //TODO change 
                    //waiting for request to be done
                }
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log("Cannot get notifications of type: " + type.ToString());
                }
                else
                {
                    var notifications = JsonConvert.DeserializeObject<Notification[]>(request.downloadHandler.text);
                    return notifications;
                }
            }
            return null;
        }
    }
}
