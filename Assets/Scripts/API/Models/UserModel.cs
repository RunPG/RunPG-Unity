using System;

[Serializable]
public class UserModel
{
  int id;
  string name;
  int? guildId;
  int? characterId;
  DateTime lastCaloriesUpdate;
}
