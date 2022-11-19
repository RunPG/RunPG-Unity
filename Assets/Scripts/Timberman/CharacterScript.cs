using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterScript : MonoBehaviour
{
    [SerializeField]
    private GameObject paladinCut;
    [SerializeField]
    private GameObject paladinIdle;
    [SerializeField]
    private GameObject mageCut;
    [SerializeField]
    private GameObject mageIdle;


    private GameObject characterCut;
    private GameObject characterIdle;
    private bool canCut;

    // Start is called before the first frame update
    void Start()
    {
        canCut = true;

        if (PlayerProfile.characterInfo.heroClass == HeroClass.MAGE)
        {
            characterCut = mageCut;
            characterIdle = mageIdle;
        }
        else
        {
            characterCut = paladinCut;
            characterIdle = paladinIdle;
        }
        
        characterIdle.SetActive(true);
    }

    public bool Cut()
    {
        if (!canCut)
            return false;

        canCut = false;
        characterIdle.SetActive(false);
        characterCut.SetActive(true);
        StartCoroutine(IdleCoroutine());

        return true;
    }

    private IEnumerator IdleCoroutine()
    {
        yield return new WaitForSeconds(0.05f);
        characterIdle.SetActive(true);
        characterCut.SetActive(false);
        canCut = true;
    }
}
