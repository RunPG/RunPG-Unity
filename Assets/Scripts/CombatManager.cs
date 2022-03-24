using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class CombatManager : MonoBehaviour
{
    private ILogger logger = Debug.unityLogger;

    [SerializeField]
    private GameObject Allies;
    [SerializeField]
    private GameObject Ennemies;
    [SerializeField]
    private GameObject ActionSelection;
    [SerializeField]
    private GameObject AttackSelection;

    private int damages = -1;
    private Button attackButton;
    private Queue<GameObject> turnQueue = new Queue<GameObject>();
    private GameObject entityInAction;


    private List<GameObject> AlliesList = new List<GameObject>();
    private List<GameObject> EnnemiesList = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        if (Allies != null)
        {
            for (int i = 0; i < Allies.transform.childCount; i++)
                AlliesList.Add(Allies.transform.GetChild(i).gameObject);
        }
        if (Ennemies != null)
        {
            for (int i = 0; i < Ennemies.transform.childCount; i++)
                EnnemiesList.Add(Ennemies.transform.GetChild(i).gameObject);
        }
    }

    private void Update()
    {
        if (entityInAction == null)
        {
            setEntityInAction();
        }

        if (entityInAction.CompareTag("Ennemy"))
        {
            EnnemieAttack();
        }

        else if (damages > 0 && (Input.touchCount == 1) && (Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Ray raycast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit raycastHit;
            if (Physics.Raycast(raycast, out raycastHit))
            {
                if (raycastHit.collider.CompareTag("Ennemy"))
                {
                    Attack(raycastHit.collider.gameObject, damages);
                    resetEntityInAction();
                }
            }
            resetUserSelection();
        }
    }

    private void setEntityInAction()
    {
        if (turnQueue.Count == 0)
            resetQueue();
        entityInAction = turnQueue.Dequeue();
        if (entityInAction.TryGetComponent(out EntityManager entity))
        {
            entity.setSelected(true);
        }
    }

    private void resetUserSelection()
    {
        damages = -1;
        attackButton.interactable = true;
        attackButton = null;
    }

    private void resetEntityInAction()
    {
        if (entityInAction && entityInAction.TryGetComponent(out EntityManager entity))
        {
            entity.setSelected(false);
        }
        entityInAction = null;
    }

    private void resetQueue()
    {
        AlliesList.ForEach(x => turnQueue.Enqueue(x));
        EnnemiesList.ForEach(x => turnQueue.Enqueue(x));
    }
    public void SetAttackButton(Button button)
    {
        attackButton = button;
    }

    public void SetDamages(int damages)
    {
        this.damages = damages;
    }

    public List<GameObject> GetAllies()
    {
        return AlliesList;
    }

    public List<GameObject> GetEnnemies()
    {
        return EnnemiesList;
    }

    public void EnnemieAttack()
    {
        Random rand = new Random();
        GameObject target = AlliesList[rand.Next(AlliesList.Count)];
        Attack(target, 10);
        resetEntityInAction();
    }

    public void Attack(GameObject target, int damages)
    {
        if (target == null)
        {
            logger.LogError("Attack", "Target is null");
        } else if (target.TryGetComponent(out EntityManager entity))
        {
            entity.TakeDamage(damages);
            AttackSelection.SetActive(false);
            ActionSelection.SetActive(true);
        } else
        {
            logger.LogError("Attack", "Cannot get EntityManager from target", target);
        }
    }
}
