using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class CombatManager : MonoBehaviour
{
    private ILogger logger = Debug.unityLogger;

    public static CombatManager Instance { get; private set; }

    private Dictionary<string, Func<CombatAction>> combatActions = new Dictionary<string, Func<CombatAction>>();

    [SerializeField]
    private List<Character> characters = new List<Character>();

    private List<CombatAction> queue = new List<CombatAction>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            Instance = this;
        }
        RegisterCombatAction<LightAttack>("Light Attack");
        RegisterCombatAction<HeavyAttack>("Heavy Attack");
        RegisterCombatAction<HealthPotion>("Health Potion");
        RegisterCombatAction<Bomb>("Bomb");
    }
    private void Start()
    {
        StartCoroutine(Combat());
    }

    public void AddAction(CombatAction action)
    {
        queue.Add(action);
    }

    /*
    public List<Character> GetMyAllies(Character myCharacter)
    {
        List<Character> allies = new List<Character>();
        foreach (var character in characters)
        {
            if (character.CompareTag(myCharacter.tag) && character.isAlive())
                allies.Add(character);
        }
        return allies;
    }

    public List<Character> GetMyEnemies(Character myCharacter)
    {
        List<Character> enemies = new List<Character>();
        foreach (var character in characters)
        {
            if (!character.CompareTag(myCharacter.tag) && character.isAlive())
                enemies.Add(character);
        }
        return enemies;
    }*/

    public List<Character> GetPossibleTargets(Character caster, CombatAction.PossibleTarget possibleTarget)
    {
        List<Character> targets = new List<Character>();
        switch (possibleTarget)
        {
            case CombatAction.PossibleTarget.Self:
                targets.Add(caster);
                return targets;
            case CombatAction.PossibleTarget.Enemy:
                foreach (var character in characters)
                {
                    if (!character.CompareTag(caster.tag) && character.isAlive())
                        targets.Add(character);
                }
                return targets;
            case CombatAction.PossibleTarget.Ally:
                foreach (var character in characters)
                {
                    if (character != caster && character.CompareTag(caster.tag) && character.isAlive())
                        targets.Add(character);
                }
                return targets;
            case CombatAction.PossibleTarget.AllyOrSelf:
                foreach (var character in characters)
                {
                    if (character.CompareTag(caster.tag) && character.isAlive())
                        targets.Add(character);
                }
                return targets;
            case CombatAction.PossibleTarget.All:
                foreach (var character in characters)
                    targets.Add(character);
                return targets;
            default:
                throw new Exception("Error on possible target");
        }
    }

    public CombatAction GetCombatAction(string name)
    {
        return combatActions[name]();
    }

    private IEnumerator Combat()
    {
        while (true)
        {
            queue.Clear();
            for (int i = 0; i < characters.Count; i++)
            {
                logger.Log("asking character " + i);
                characters[i].AskForAction();
                while (queue.Count < i + 1)
                    yield return null;
            }
            foreach (var action in queue)
            {
                // Later some spell may target dead characters (revive)
                if (action == null || !action.caster.isAlive() || !action.target.isAlive())
                    continue;

                if (!VerifyTarget(action))
                    throw new Exception("Error wrong target");

                logger.Log("resolving action " + action.name);
                if (action as Consumable != null)
                    action.caster.UseConsumable(action.name);
                action.doAction();
                yield return new WaitForSeconds(1);
            }
            if (characters.Where(c => c.isAlive() && c.CompareTag("Team1")).Count() == 0
                || characters.Where(c => c.isAlive() && c.CompareTag("Team2")).Count() == 0)
            {
                break;
            }
        }
        logger.Log("end");
    }

    public bool VerifyTarget(CombatAction action)
    {
        return GetPossibleTargets(action.caster, action.possibleTarget).Find(character => character == action.target) != null;
    }

    public void ShowPossibleTarget(Character caster, CombatAction.PossibleTarget possibleTarget)
    {
        GetPossibleTargets(caster, possibleTarget).ForEach(target => target.ShowSelector(true));
    }

    public void HidePossibleTarget()
    {
        characters.ForEach(target => target.ShowSelector(false));
    }

    private void RegisterCombatAction<T>(string name) where T : CombatAction, new()
    {
        combatActions.Add(name, () =>
        {
            return new T();
        });
    }
}
