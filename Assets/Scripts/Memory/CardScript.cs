using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardScript : MonoBehaviour
{
    public enum FlowerType
    {
        WHITE,
        ORANGE,
        PURPLE,
        YELLOW,
        RED
    }

    [SerializeField]
    GameObject Front;
    [SerializeField]
    GameObject Back;
    [SerializeField]
    SpriteRenderer Flower;

    private FlowerType _flower;
    public FlowerType flowerType
    {
        get { return _flower; }
        set
        {
            _flower = value;
            Flower.sprite = MemoryScript.Instance.GetFlowerSprite(value);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Back.SetActive(false);
        Front.SetActive(true);
    }

    public void SetFrontVisible(bool value)
    {
        Front.SetActive(value);
        Back.SetActive(!value);
    }
}
