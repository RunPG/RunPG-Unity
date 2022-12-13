using System;
using System.Collections;
using System.Collections.Generic;
using RunPG.Multi;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MemoryScript : MonoBehaviour
{
  const float QUANTITY_MULTIPLIER = 5;

  [SerializeField]
  private List<CardScript> cards;

  [SerializeField]
  private GameObject gameInfo;
  [SerializeField]
  private TextMeshProUGUI scoreIndicator;
  [SerializeField]
  private TextMeshProUGUI timer;
  [SerializeField]
  private TextMeshProUGUI tuto;
  [SerializeField]
  private Image veil;
  [SerializeField]
  private TextMeshProUGUI countdown;
  [SerializeField]
  private Button startButton;

  [SerializeField]
  private GameObject results;
  [SerializeField]
  private TextMeshProUGUI resultsScore;
  [SerializeField]
  private TextMeshProUGUI firstQuantity;
  [SerializeField]
  private TextMeshProUGUI secondQuantity;
  [SerializeField]
  private TextMeshProUGUI thirdQuantity;
  [SerializeField]
  private Button resultButton;

  [SerializeField]
  Sprite WhiteFlowerImage;
  [SerializeField]
  Sprite OrangeFlowerImage;
  [SerializeField]
  Sprite PurpleFlowerImage;
  [SerializeField]
  Sprite YellowFlowerImage;
  [SerializeField]
  Sprite RedFlowerImage;

  private CardScript firstCard = null;
  private CardScript secondCard = null;

  private bool[] touchDidMove = new bool[10];

  private int remainingPairs;

  private bool canFlip = true;

  private int score = 0;
  private int multiplier = 1;

  private Coroutine timerCoroutine;
  private Coroutine resultsCoroutine;

  private bool hasLaunched = false;
  private bool hasStarted = false;
  private bool hasEnded = false;

  public static MemoryScript Instance;

  private void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
    }
    else
    {
      Destroy(this);
    }
  }


  // Start is called before the first frame update
  void Start()
  {
    resultButton.onClick.AddListener(() => SkipResults());
    ShuffleCards();
    remainingPairs = cards.Count / 2;
  }

  private void ShuffleCards()
  {
    var colors = new List<CardScript.FlowerType>()
        {
            CardScript.FlowerType.WHITE, CardScript.FlowerType.WHITE, CardScript.FlowerType.ORANGE, CardScript.FlowerType.ORANGE, CardScript.FlowerType.PURPLE,
            CardScript.FlowerType.PURPLE, CardScript.FlowerType.YELLOW, CardScript.FlowerType.YELLOW, CardScript.FlowerType.RED, CardScript.FlowerType.RED
        };
    foreach (var card in cards)
    {
      int x = UnityEngine.Random.Range(0, colors.Count);
      card.flowerType = colors[x];
      colors.RemoveAt(x);
    }
  }

  private void Update()
  {
    foreach (var touch in Input.touches)
    {
      if (touch.phase == TouchPhase.Began)
      {
        touchDidMove[touch.fingerId] = false;
      }
      else if (touch.phase == TouchPhase.Moved)
      {
        touchDidMove[touch.fingerId] = true;
      }
      else if (canFlip && hasStarted && !hasEnded && touch.phase == TouchPhase.Ended && !touchDidMove[touch.fingerId])
      {
        Ray ray = Camera.main.ScreenPointToRay(touch.position);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
          CardScript card = hit.transform.gameObject.GetComponent<CardScript>();
          if (card != null && !card.isFlipped)
          {
            canFlip = false;
            StartCoroutine(FlipCoroutine(card));
          }
        }
      }
    }
  }

  public void SetFlippedCard(CardScript card)
  {
    if (firstCard == null)
    {
      firstCard = card;
      canFlip = true;
    }
    else
    {
      secondCard = card;
      if (firstCard.flowerType == secondCard.flowerType)
      {
        firstCard = null;
        secondCard = null;
        remainingPairs--;
        score++;
        if (remainingPairs <= 0)
        {
          multiplier++;
          StartCoroutine(ResetCardsCoroutine());
        }
        else
        {
          canFlip = true;
        }
        scoreIndicator.text = (score * multiplier).ToString();
      }
      else
      {
        StartCoroutine(WrongPairCoroutine(firstCard, secondCard));
        firstCard = null;
        secondCard = null;
      }
    }
  }

  IEnumerator FlipCoroutine(CardScript card)
  {
    card.isFlipped = true;
    float elapsedTime = 0f;
    float duration = 0.05f;
    while (elapsedTime < duration)
    {
      elapsedTime += Time.deltaTime;
      card.transform.rotation = Quaternion.Euler(0, Mathf.Lerp(0, 90, elapsedTime / duration), 0);
      yield return null;
    }
    card.SetFrontVisible(false);
    elapsedTime = 0f;
    while (elapsedTime < duration)
    {
      elapsedTime += Time.deltaTime;
      card.transform.rotation = Quaternion.Euler(0, Mathf.Lerp(90, 180, elapsedTime / duration), 0);
      yield return null;
    }
    SetFlippedCard(card);
  }

  IEnumerator FlipBackCoroutine(List<CardScript> cards)
  {
    float elapsedTime = 0f;
    float duration = 0.1f;
    while (elapsedTime < duration)
    {
      elapsedTime += Time.deltaTime;
      foreach (var card in cards)
      {
        card.transform.rotation = Quaternion.Euler(0, Mathf.Lerp(180, 90, elapsedTime / duration), 0);
      }
      yield return null;
    }
    foreach (var card in cards)
    {
      card.SetFrontVisible(true);
    }
    elapsedTime = 0f;
    while (elapsedTime < duration)
    {
      elapsedTime += Time.deltaTime;
      foreach (var card in cards)
      {
        card.transform.rotation = Quaternion.Euler(0, Mathf.Lerp(90, 0, elapsedTime / duration), 0);
      }
      yield return null;
    }
    foreach (var card in cards)
    {
      card.isFlipped = false;
    }
    canFlip = true;
  }

  IEnumerator WrongPairCoroutine(CardScript first, CardScript second)
  {
    yield return new WaitForSeconds(0.1f);
    yield return StartCoroutine(FlipBackCoroutine(new List<CardScript>() { first, second }));
  }

  IEnumerator ResetCardsCoroutine()
  {
    yield return StartCoroutine(FlipBackCoroutine(cards));
    remainingPairs = cards.Count / 2;
    ShuffleCards();
  }


  public void SkipResults()
  {
    StopCoroutine(resultsCoroutine);
    resultsScore.text = (score * multiplier).ToString();
    List<int> quantities = GetQuantities();
    firstQuantity.text = "x " + quantities[0].ToString();
    secondQuantity.text = "x " + quantities[1].ToString();
    thirdQuantity.text = "x " + quantities[2].ToString();

    resultButton.onClick.RemoveAllListeners();
    resultButton.onClick.AddListener(() => Leave());
  }

  public async void Leave()
  {
    List<int> quantities = GetQuantities();
    if (quantities[0] > 0)
      await Requests.POSTInventoryItem(PlayerProfile.id, new PostItemModel(4, quantities[0]));
    if (quantities[1] > 0)
      await Requests.POSTInventoryItem(PlayerProfile.id, new PostItemModel(7, quantities[1]));
    if (quantities[2] > 0)
      await Requests.POSTInventoryItem(PlayerProfile.id, new PostItemModel(10, quantities[2]));
    SceneManager.LoadScene("MapScene");
  }

  public void StartCountdown()
  {
    Debug.Log("test");
    if (!hasLaunched)
    {
      startButton.gameObject.SetActive(false);
      StartCoroutine(CountdownCoroutine());
    }
  }

  private IEnumerator CountdownCoroutine()
  {
    hasLaunched = true;
    startButton.gameObject.SetActive(false);
    veil.gameObject.SetActive(false);
    tuto.gameObject.SetActive(false);
    gameInfo.SetActive(true);
    countdown.gameObject.SetActive(true);
    countdown.text = "3";

    yield return new WaitForSeconds(1f);

    countdown.text = "2";

    yield return new WaitForSeconds(1f);

    countdown.text = "1";

    yield return new WaitForSeconds(1f);

    countdown.gameObject.SetActive(false);
    timerCoroutine = StartCoroutine(TimerCoroutine());
  }

  private IEnumerator TimerCoroutine()
  {
    hasStarted = true;
    float elapsedTime = 0f;
    while (elapsedTime < 30f)
    {
      elapsedTime += Time.deltaTime;
      TimeSpan remainingTime = new TimeSpan(0, 0, Mathf.RoundToInt(30f - elapsedTime));
      timer.text = remainingTime.ToString(@"m\:ss");
      yield return null;
    }
    End();
  }

  private void End()
  {
    hasEnded = true;
    StopCoroutine(timerCoroutine);
    resultsCoroutine = StartCoroutine(ResultCoroutine());
  }

  private List<int> GetQuantities()
  {
    List<int> quantities = new List<int>();
    quantities.Add(0);
    quantities.Add(0);
    quantities.Add(0);
    quantities[0] = Mathf.RoundToInt(score * multiplier * QUANTITY_MULTIPLIER);
    quantities[1] = Mathf.RoundToInt(quantities[0] * 0.5f) / 5;
    quantities[0] -= quantities[1] * 5;
    quantities[2] = Mathf.RoundToInt(quantities[1] * 0.5f) / 5;
    quantities[1] -= quantities[2] * 5;
    return quantities;
  }

  private IEnumerator ResultCoroutine()
  {
    veil.gameObject.SetActive(true);
    gameInfo.SetActive(false);
    veil.color = new Color(0, 0, 0, 0);

    float elapsedTime = 0f;
    while (elapsedTime < 0.5f)
    {
      elapsedTime += Time.deltaTime;
      veil.color = new Color(0, 0, 0, Mathf.Lerp(0f, 0.95f, elapsedTime / 0.5f));
      yield return null;
    }

    RectTransform rect = results.GetComponent<RectTransform>();
    elapsedTime = 0f;
    while (elapsedTime < 0.5f)
    {
      elapsedTime += Time.deltaTime;
      rect.pivot = new Vector2(0.5f, Mathf.Lerp(0, 1, EasingFunc.EaseOutBounce(elapsedTime / 0.5f)));
      yield return null;
    }

    resultButton.gameObject.SetActive(true);

    yield return new WaitForSeconds(0.1f);

    elapsedTime = 0f;
    while (elapsedTime < 0.5f)
    {
      elapsedTime += Time.deltaTime;
      resultsScore.text = Mathf.RoundToInt(Mathf.Lerp(0, score * multiplier, EasingFunc.EaseOutQuad(elapsedTime / 0.5f))).ToString();
      yield return null;
    }

    yield return new WaitForSeconds(0.1f);

    List<int> quantities = GetQuantities();

    elapsedTime = 0f;
    while (elapsedTime < 0.5f)
    {
      elapsedTime += Time.deltaTime;
      firstQuantity.text = "x " + Mathf.RoundToInt(Mathf.Lerp(0, quantities[0], EasingFunc.EaseOutQuad(elapsedTime / 0.5f))).ToString();
      yield return null;
    }

    yield return new WaitForSeconds(0.1f);

    elapsedTime = 0f;
    while (elapsedTime < 0.5f)
    {
      elapsedTime += Time.deltaTime;
      secondQuantity.text = "x " + Mathf.RoundToInt(Mathf.Lerp(0, quantities[1], EasingFunc.EaseOutQuad(elapsedTime / 0.5f))).ToString();
      yield return null;
    }

    yield return new WaitForSeconds(0.1f);

    elapsedTime = 0f;
    while (elapsedTime < 0.5f)
    {
      elapsedTime += Time.deltaTime;
      thirdQuantity.text = "x " + Mathf.RoundToInt(Mathf.Lerp(0, quantities[2], EasingFunc.EaseOutQuad(elapsedTime / 0.5f))).ToString();
      yield return null;
    }

    resultButton.onClick.RemoveAllListeners();
    resultButton.onClick.AddListener(() => Leave());
  }

  public Sprite GetFlowerSprite(CardScript.FlowerType type)
  {
    switch (type)
    {
      case CardScript.FlowerType.WHITE:
        return WhiteFlowerImage;
      case CardScript.FlowerType.ORANGE:
        return OrangeFlowerImage;
      case CardScript.FlowerType.PURPLE:
        return PurpleFlowerImage;
      case CardScript.FlowerType.YELLOW:
        return YellowFlowerImage;
      case CardScript.FlowerType.RED:
        return RedFlowerImage;
      default:
        return WhiteFlowerImage;
    }
  }
}
