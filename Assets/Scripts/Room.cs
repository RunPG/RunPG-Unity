using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Room
{
    public enum RoomType
    {
        Fight,
        Heal,
        Bonus,
        Random,
        Boss,
        Start
    }

    public static Sprite getRoomSprite(RoomType room)
    {
        return room switch
        {
            RoomType.Fight => Resources.Load<Sprite>("Dungeon/FightRoom"),
            RoomType.Heal => Resources.Load<Sprite>("Dungeon/HealRoom"),
            RoomType.Bonus => Resources.Load<Sprite>("Dungeon/BonusRoom"),
            RoomType.Random => Resources.Load<Sprite>("Dungeon/RandomRoom"),
            RoomType.Boss => Resources.Load<Sprite>("Dungeon/BossRoom"),
            RoomType.Start => Resources.Load<Sprite>("Dungeon/StartRoom"),
            _ => null,
        };
    }
}