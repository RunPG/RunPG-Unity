﻿using Mapbox.Json.Linq;
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
        static readonly string rootUrl = "https://runpg.fooks.fr/";
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
                    var user = JsonConvert.DeserializeObject<UserModel>(request.downloadHandler.text);
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
                var user = JsonConvert.DeserializeObject<UserModel>(request.downloadHandler.text);
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

        public static async Task<NotificationModel> GETNotificationsByTypeBySender(int receiver_id, NotificationType type, int sender_id)
        {
            string url = rootUrl + "user/" + receiver_id + "/notification/" + type + "/" + sender_id;
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {

                request.SendWebRequest();
                while (!request.isDone)
                {
                    await Task.Yield();
                }
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(string.Format("Error in request:{0}\nError Message: {1}\nError 404 can be normal (not an error)", url, request.error));
                }
                else
                {
                    var notifications = JsonConvert.DeserializeObject<NotificationModel>(request.downloadHandler.text);
                    return notifications;
                }
            }
            return null;
        }

        public static async Task<InventoryModel[]> GETUserInventory(int user_id)
        {
            var url = rootUrl + "user/" + user_id + "/inventory";
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

        public static async Task<EquipmentBaseModel[]> GETEquipements()
        {
            var url = rootUrl + "equipmentBase";
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
                    var equipements = JsonConvert.DeserializeObject<EquipmentBaseModel[]>(request.downloadHandler.text);
                    return equipements;
                }
            }
            return null;
        }

        public static async Task<EquipmentModel> GETEquipmentById(int equipment_id)
        {
            var url = rootUrl + "equipment/" + equipment_id;
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
                    var equipement = JsonConvert.DeserializeObject<EquipmentModel>(request.downloadHandler.text);
                    return equipement;
                }
            }
            return null;
        }

        public static async Task<UserCharacterModel> GETUserCharacter(int user_id)
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
                    var character = JsonConvert.DeserializeObject<UserCharacterModel>(request.downloadHandler.text);
                    return character;
                }
            }
            return null;
        }

        public static async Task<FriendModel> GETUserFriend(int user_id, int friend_id)
        {
            var url = rootUrl + "user/" + user_id + "/friend/" + friend_id;
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.SendWebRequest();
                while (!request.isDone)
                {
                    await Task.Yield();
                }
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(string.Format("Error in request:{0}\nError Message: {1}\nError 404 can be normal (not an error)", url, request.error));
                }
                else
                {
                    var friendModel = JsonConvert.DeserializeObject<FriendModel>(request.downloadHandler.text);
                    return friendModel;
                }
            }
            return null;
        }

        public static async Task<GuildListModel[]> GETAllGuilds()
        {
            var url = rootUrl + "guild";
            using UnityWebRequest request = UnityWebRequest.Get(url);
            request.SendWebRequest();
            while (!request.isDone)
            {
                await Task.Yield();
            }
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(string.Format("Error in request:{0}\nError Message: {1}\nError 404 can be normal (not an error)", url, request.error));
            }
            else
            {
                var guildModels = JsonConvert.DeserializeObject<GuildListModel[]>(request.downloadHandler.text);
                return guildModels;
            }
            return null;
        }

        public static async Task<GuildModel> GETGuildById(int guildId)
        {
            var url = rootUrl + "guild/" + guildId;
            using UnityWebRequest request = UnityWebRequest.Get(url);
            request.SendWebRequest();
            while (!request.isDone)
            {
                await Task.Yield();
            }
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(string.Format("Error in request:{0}\nError Message: {1}\nError 404 can be normal (not an error)", url, request.error));
                return null;
            }
            else
            {
                var guildModel = JsonConvert.DeserializeObject<GuildModel>(request.downloadHandler.text);
                return guildModel;
            }
        }

        public static async Task<ActivityModel> GetActivityAvailability(int user_id, long activity_id)
        {
            var url = rootUrl + "user/" + user_id + "/activity/" + activity_id;
            using UnityWebRequest request = UnityWebRequest.Get(url);

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
                var activity = JsonConvert.DeserializeObject<ActivityModel>(request.downloadHandler.text);
                return activity;
            }
            return null;
        }

        public static async Task<ItemModel[]> GetItemById(int? item_id)
        {
            var url = rootUrl + "item/" + (item_id.HasValue ? "?id=" + item_id : "");
            using UnityWebRequest request = UnityWebRequest.Get(url);

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
                var activity = JsonConvert.DeserializeObject<ItemModel[]>(request.downloadHandler.text);
                return activity;
            }
            return null;
        }

        public static async Task<CaloriesModel> GetCaloriesToday(int userId)
        {
            var url = rootUrl + "user/" + userId + "/caloriesToday" ;
            using UnityWebRequest request = UnityWebRequest.Get(url);

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
                var calroies = JsonConvert.DeserializeObject<CaloriesModel>(request.downloadHandler.text);
                return calroies;
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

        public static async Task<bool> POSTAddFriend(int userId, int friend_id)
        {
            var url = rootUrl + "user/" + userId + "/friend/" + friend_id;
            
            using UnityWebRequest request = UnityWebRequest.Post(url, "POST");
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
        
        public static async Task<bool> POSTSendNotification(int senderId, int receiver_id, NotificationType type)
        {
            var url = rootUrl + "user/" + receiver_id + "/notification/" + type + "/" + senderId;
            
            using UnityWebRequest request = UnityWebRequest.Post(url, "POST");
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

        public static async Task<bool> POSTInventoryEquipement(int user_id, NewEquipementModel newEquipment)
        {
            var url = rootUrl + "user/" + user_id + "/inventory/equipement";
            var content = JsonConvert.SerializeObject(newEquipment);

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

        public static async Task<bool> POSTPlayerEquipment(int user_id, PlayerEquipmentModel equipments)
        {
            var url = rootUrl + "user/" + user_id + "/equiped";
            var content = JsonConvert.SerializeObject(equipments);

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

        public static async Task<bool> POSTLevelUp(int user_id, StatisticsModel statistics)
        {
            var url = rootUrl + "user/" + user_id + "/levelup";
            var content = JsonConvert.SerializeObject(statistics);

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

        public static async Task<bool> POSTActivity(int user_id, long activity_id)
        {
            var url = rootUrl + "user/" + user_id + "/activity/" + activity_id;
            using UnityWebRequest request = UnityWebRequest.Post(url, "POST");
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

        public static async Task<bool> POSTJoinGuild(int userId, int guildId)
        {
            var url = rootUrl + "user/" + userId + "/join/" + guildId;

            using UnityWebRequest request = UnityWebRequest.Post(url, "POST");
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

        public static async Task<GuildListModel> POSTGuild(GuildCreateModel guildCreateModel)
        {
            var url = rootUrl + "guild";
            var content = JsonConvert.SerializeObject(guildCreateModel);

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
                return null;
            }
            var guild = JsonConvert.DeserializeObject<GuildListModel>(request.downloadHandler.text);
            return guild;
        }

        public static async Task<bool> PUTGuild(int guildId, GuildPutModel guildModel)
        {
            var url = rootUrl + "guild/" + guildId;
            var content = JsonConvert.SerializeObject(guildModel);

            using UnityWebRequest request = UnityWebRequest.Put(url, "PUT");
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

        public static async Task<bool> PUTXp(int userId)
        {
            var url = rootUrl + "user/" + userId + "/xp";

            using UnityWebRequest request = UnityWebRequest.Put(url, "PUT");
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

        public static async Task<bool> DELETENotification(int receiverId, int senderId, NotificationType type)
        {
            var url = rootUrl + "user/" + receiverId + "/notification/" + type + '/'+ senderId;
            
            using UnityWebRequest request = UnityWebRequest.Delete(url);
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

        public static async Task<bool> DELETELeaveGuild(int userId)
        {
            var url = rootUrl + "user/" + userId + "/guild";

            using UnityWebRequest request = UnityWebRequest.Delete(url);
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
        public static async Task<MarketModel[]> GETallOpenItems()
        {
            var url = rootUrl + "market/";
            using UnityWebRequest request = UnityWebRequest.Get(url);
            request.SendWebRequest();
            while (!request.isDone)
            {
                await Task.Yield();
            }
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(string.Format("Error in request:{0}\nError Message: {1}\nError 404 can be normal (not an error)", url, request.error));
            }
            else
            {
                var marketModels = JsonConvert.DeserializeObject<MarketModel[]>(request.downloadHandler.text);
                return marketModels;
            }
            return null;
        }

        public static async Task<bool> GETItemByID(int itemId)
        {
            var url = rootUrl + "market/" + itemId;

            using UnityWebRequest request = UnityWebRequest.Get(url);
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

        public static async Task<bool> DELETEItem(int itemId)
        {
            var url = rootUrl + "market/" + itemId;

            using UnityWebRequest request = UnityWebRequest.Delete(url);
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
        public static async Task<bool> POSTCreateItem(int inventoryId, int price, int stackSize)
        {
            var url = rootUrl + "market/";

            using UnityWebRequest request = UnityWebRequest.Post(url, "POST");
            request.SetRequestHeader("Content-Type", "application/json");
            NewItemModel newItem = new NewItemModel(inventoryId, price, stackSize);
            var content = JsonConvert.SerializeObject(newItem);

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
        public static async Task<bool> POSTBuyItem(int itemId, int buyerId)
        {
            var url = rootUrl + "market/" + itemId + "/buy";

            using UnityWebRequest request = UnityWebRequest.Post(url, "Post");
            request.SetRequestHeader("Content-Type", "application/json");
            JObject jobject = new JObject(new JProperty("buyerId", buyerId));
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jobject.ToString()));

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
        public static async Task<bool> GETByEquipmentBaseId(int id)
        {
            var url = rootUrl + "market/equipmentBase" + id;

            using UnityWebRequest request = UnityWebRequest.Get(url);
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
        public static async Task<bool> GETByItemId(int id)
        {
            var url = rootUrl + "market/item" + id;

            using UnityWebRequest request = UnityWebRequest.Get(url);
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
}
