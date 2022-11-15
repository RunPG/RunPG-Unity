using RunPG.Multi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInfos
{
    public int id;
    public string name;
    public HeroClass heroClass;
    public int level;

    public UserInfos(UserModel userModel, UserCharacterModel characterModel)
    {
        id = userModel.id;
        name = userModel.name;
        heroClass = characterModel.character.heroClass;
        level = characterModel.statistics.level;
    }

}
