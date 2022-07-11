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
        public static async Task<UserModel> GETUserByName(string username)
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
                    Debug.LogWarning(string.Format("Error in request:{0}\nError Message: {1}", url, request.error));
                    return null;
                }
                else
                {
                    var user = JsonUtility.FromJson<UserModel>(request.downloadHandler.text);
                    return user;
                }
            }
            return null;
        }

        public static async Task<UserModel> GETUserById(int user_id)
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
                var user = JsonUtility.FromJson<UserModel>(request.downloadHandler.text);
                return user;
            }
        }
        
        public static async Task<FriendlistModel> GETAllFriends(int user_id)
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
                    var friends = JsonConvert.DeserializeObject<FriendlistModel>(request.downloadHandler.text); 
                    return friends;
                }
            }         
            return null;
        }

        public static async Task<NotificationModel[]> GETNotificationsByType(int receiver_id, NotificationType type)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(rootUrl + "user/" + receiver_id + "/notification/" + type))
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
                    var notifications = JsonConvert.DeserializeObject<NotificationModel[]>(request.downloadHandler.text);
                    return notifications;
                }
            }
            return null;
        }

        public static async Task<InventoryModel[]> GETUserInventory(int user_id)
        {
            var url = rootUrl + "inventory/user/" + user_id;
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
                    var inventory = JsonConvert.DeserializeObject<InventoryModel[]>(request.downloadHandler.text);
                    return inventory;
                }
            }
            return null;
        }

        public static async Task<EquipementBaseModel[]> GETEquipements()
        {
            var url = rootUrl + "equipementBase";
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
                    var equipements = JsonConvert.DeserializeObject<EquipementBaseModel[]>(request.downloadHandler.text);
                    return equipements;
                }
            }
            return null;
        }

        public static async Task<EquipementModel> GETEquipementById(int equipement_id)
        {
            var url = rootUrl + "equipement/" + equipement_id;
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
                    var equipement = JsonConvert.DeserializeObject<EquipementModel>(request.downloadHandler.text);
                    return equipement;
                }
            }
            return null;
        }

        public static async Task<CharacterModel> GETUserCharacter(int user_id)
        {
            var url = rootUrl + "user/" + user_id + "/character";
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
                    var character = JsonConvert.DeserializeObject<CharacterModel>(request.downloadHandler.text);
                    return character;
                }
            }
            return null;
        }

        public static async Task<bool> POSTNewUser(NewUserModel newUser)
        {
            var url = rootUrl + "user";
            var content = JsonConvert.SerializeObject(newUser);
            Debug.Log(content);
            using (UnityWebRequest request = UnityWebRequest.Post(url, "POST"))
            {
                request.SetRequestHeader("Content-Type", "application/json");
                request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(content)) as UploadHandler;
                request.SendWebRequest();
                while (!request.isDone)
                {
                    await Task.Yield();
                }
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(string.Format("Error in request:{0}\nError Message: {1}", url, request.error));
                    return false;
                }
                return true;
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

        public static async Task<bool> POSTInventoryEquipement(int user_id, NewEquipementModel newUser)
        {
            var url = rootUrl + "user/" + user_id + "/inventory/equipement";
            var content = JsonConvert.SerializeObject(newUser);

            using UnityWebRequest request = UnityWebRequest.Post(url, "POST");
            request.SetRequestHeader("Content-Type", "application/json");
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(content));
            request.SendWebRequest();
            while (!request.isDone)
            {
                await Task.Yield();
            }
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(string.Format("Error in request:{0}\nError Message: {1}", url, request.error));
                return false;
            }
            return true;
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
