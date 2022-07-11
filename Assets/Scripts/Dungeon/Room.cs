using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class Room
{
    public abstract void onClickAction();

    public abstract Sprite getRoomSprite();
}

public class StartRoom : Room
{
    public override void onClickAction()
    {
        throw new NotImplementedException();
    }

    public override Sprite getRoomSprite()
    {
        return Resources.Load<Sprite>("Dungeon/StartRoom");
    }
}

public class FightRoom : Room
{
    private DungeonManager.DungeonMonsterInfo[] monsters;

    public FightRoom(DungeonManager.DungeonMonsterInfo[] monsters)
    {
        this.monsters = monsters;
    }

    public override void onClickAction()
    {
        DungeonManager.instance.StartBattle(monsters);
    }
    public override Sprite getRoomSprite()
    {
        return Resources.Load<Sprite>("Dungeon/FightRoom");
    }
}

public class BossRoom : Room
{
    private DungeonManager.DungeonMonsterInfo[] monsters;

    public BossRoom(DungeonManager.DungeonMonsterInfo[] monsters)
    {
        this.monsters = monsters;
    }

    public override void onClickAction()
    {
        DungeonManager.instance.StartBattle(monsters);
    }
    public override Sprite getRoomSprite()
    {
        return Resources.Load<Sprite>("Dungeon/BossRoom");
    }
}

public class HealRoom : Room
{
    public override void onClickAction()
    {
        DungeonManager.instance.HealParty();
        DungeonMap.RefreshMap();
    }

    public override Sprite getRoomSprite()
    {
        return Resources.Load<Sprite>("Dungeon/HealRoom");
    }
}

public class BonusRoom : Room 
{
    public override void onClickAction()
    {
        DungeonManager.instance.GiveParty();
        DungeonMap.RefreshMap();
    }

    public override Sprite getRoomSprite()
    {
        return Resources.Load<Sprite>("Dungeon/BonusRoom");
    }
}