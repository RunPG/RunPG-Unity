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
        static readonly string rootUrl = "http://178.62.237.73/";
        public static async Task<User> GETUserByName(string username)
        {
            if (username.Length != 0)
            {
                var url = rootUrl + "user/name/" + username;
                using UnityWebRequest request = UnityWebRequest.Get(url);
                request.SendWebRequest();
                while (!request.isDone)
                {
                    await Task.Yield();
                }

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(string.Format("Error in request:{0}\nError Message: {1}", url, request.error));
                    return null;
                }
                else
                {
                    var user = JsonUtility.FromJson<User>(request.downloadHandler.text);
                    return user;
                }
            }
            return null;
        }

        public static async Task<User> GETUserById(int user_id)
        {
            var url = rootUrl + "user/" + user_id;
            using UnityWebRequest request = UnityWebRequest.Get(url);
            request.SendWebRequest();
            while (!request.isDone)
            {
                await Task.Yield();
            }
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(string.Format("Error in request:{0}\nError Message: {1}", url, request.error));
                return null;
            }
            else
            {
                var user = JsonUtility.FromJson<User>(request.downloadHandler.text);
                return user;
            }
        }
        
        public static async Task<Friendlist> GETAllFriends(int user_id)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(rootUrl + "user/" + user_id + "/friend/"))
            {
                request.SendWebRequest();
                while (!request.isDone)
                {
                    await Task.Yield();
                }
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log("Cannot get friends");
                }
                else
                {
                    Debug.Log(rootUrl + "user/" + user_id + "/friend/");                 
                    var friends = JsonConvert.DeserializeObject<Friendlist>(request.downloadHandler.text); 
                    return friends;
                }
            }         
            return null;
        }

        public static async Task<Notification[]> GETNotificationsByType(int receiver_id, NotificationType type)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(rootUrl + "/user/" + receiver_id + "/notification/" + type))
            {
                
                request.SendWebRequest();
                while (!request.isDone)
                {
                    await Task.Yield();
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

        public static async Task<Inventory[]> GETUserInventory(int user_id)
        {
            var url = rootUrl + "/inventory/user/" + user_id;
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.SendWebRequest();
                while (!request.isDone)
                {
                    await Task.Yield();
                }
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(string.Format("Error in request:{0}\nError Message: {1}", url, request.error));
                }
                else
                {
                    var inventory = JsonConvert.DeserializeObject<Inventory[]>(request.downloadHandler.text);
                    return inventory;
                }
            }
            return null;
        }

        public static async Task<Equipement[]> GETEquipements()
        {
            var url = rootUrl + "/equipementBase";
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.SendWebRequest();
                while (!request.isDone)
                {
                    await Task.Yield();
                }
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(string.Format("Error in request:{0}\nError Message: {1}", url, request.error));
                }
                else
                {
                    var equipements = JsonConvert.DeserializeObject<Equipement[]>(request.downloadHandler.text);
                    return equipements;
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

        public static IEnumerator POSTInventoryEquipement(int user_id, int equipement_id)
        {
            var url = rootUrl + "user/" + user_id + "/inventory/equipement";
            var str = "{\"equipementId\":\"" + equipement_id + "\"}";

            using UnityWebRequest request = UnityWebRequest.Post(url, "POST");
            request.SetRequestHeader("Content-Type", "application/json");
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(str));
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(string.Format("Error in request:{0}\nError Message: {1}", url, request.error));
            }
        }


        public static IEnumerator DELETENotification(int userId, int friend_id, NotificationType type)
        {
            var str = rootUrl + "user/" + userId + "/notification/" + type + '/'+ friend_id;
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
    }
}
