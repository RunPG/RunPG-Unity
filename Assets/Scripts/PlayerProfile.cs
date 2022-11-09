using RunPG.Multi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerProfile
{
    public static string email = null;
    public static string pseudo = null;
    public static string guid = null;
    public static int id;
    public static string serverAuthCode = null;
    public static CharacterInfo characterInfo;
    public static int? guildId;
    public static bool isGuildOwner;
}
