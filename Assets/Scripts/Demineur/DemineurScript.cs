using System;
using System.Collections;
using System.Collections.Generic;
using RunPG.Multi;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DemineurScript : MonoBehaviour
{
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



  private Coroutine timerCoroutine;
  private Coroutine resultsCoroutine;
  private bool hasLaunched = false;
  public bool hasStarted = false;
  public bool hasEnded = false;


  [SerializeField]
  private GameObject blockPrefab;
  [SerializeField]
  private Transform gridTransform;
  [SerializeField]
  private TMP_Text remainingMinesIndicator;
  private List<List<BlockModel>> blocks;
  private int remainingMines;
  private int remainingBlocks;
  private int spottedMines;
  private int score;

  void Start()
  {
    resultButton.onClick.AddListener(() => SkipResults());


    InitBlocks();
    InitMines();
  }

  public void StartCountdown()
  {
    if (!hasLaunched)
    {
      startButton.gameObject.SetActive(false);
      StartCoroutine(CountdownCoroutine());
    }
  }

  public void SkipResults()
  {
    StopCoroutine(resultsCoroutine);
    resultsScore.text = score.ToString();
    List<int> quantities = GetQuantities();
    firstQuantity.text = "x " + quantities[0].ToString();
    secondQuantity.text = "x " + quantities[1].ToString();
    thirdQuantity.text = "x " + quantities[2].ToString();

    resultButton.onClick.RemoveAllListeners();
    resultButton.onClick.AddListener(() => Leave());
  }

  private List<int> GetQuantities()
  {
    List<int> quantities = new List<int>();
    quantities.Add(0);
    quantities.Add(0);
    quantities.Add(0);
    quantities[0] = Mathf.RoundToInt(score);
    quantities[1] = Mathf.RoundToInt(quantities[0] * 0.5f) / 5;
    quantities[0] -= quantities[1] * 5;
    quantities[2] = Mathf.RoundToInt(quantities[1] * 0.5f) / 5;
    quantities[1] -= quantities[2] * 5;
    return quantities;
  }

  public async void Leave()
  {
    List<int> quantities = GetQuantities();
    if (quantities[0] > 0)
      await Requests.POSTInventoryItem(PlayerProfile.id, new PostItemModel(5, quantities[0]));
    if (quantities[1] > 0)
      await Requests.POSTInventoryItem(PlayerProfile.id, new PostItemModel(8, quantities[1]));
    if (quantities[2] > 0)
      await Requests.POSTInventoryItem(PlayerProfile.id, new PostItemModel(11, quantities[2]));
    SceneManager.LoadScene("MapScene");
  }

  private IEnumerator CountdownCoroutine()
  {
    hasLaunched = true;
    veil.gameObject.SetActive(false);
    tuto.gameObject.SetActive(false);
    gameInfo.SetActive(true);
    countdown.gameObject.SetActive(true);
    countdown.text = "3";
    countdown.color = new Color32(255, 0, 0, 255);

    yield return new WaitForSeconds(1f);

    countdown.text = "2";
    countdown.color = new Color32(0, 123, 0, 255);

    yield return new WaitForSeconds(1f);

    countdown.text = "1";
    countdown.color = new Color32(0, 0, 255, 255);

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
    score += spottedMines * 15;
    StopCoroutine(timerCoroutine);
    resultsCoroutine = StartCoroutine(ResultCoroutine());
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
      resultsScore.text = Mathf.RoundToInt(Mathf.Lerp(0, score, EasingFunc.EaseOutQuad(elapsedTime / 0.5f))).ToString();
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



  void InitBlocks()
  {
    blocks = new List<List<BlockModel>>();
    for (int i = 0; i < 6; i++)
    {
      blocks.Add(new List<BlockModel>());
      for (int j = 0; j < 6; j++)
      {
        GameObject blockObject = Instantiate(blockPrefab, gridTransform);
        BlockModel blockModel = blockObject.GetComponent<BlockModel>();
        blockModel.Init(this, i, j);
        blocks[i].Add(blockModel);
      }
    }
  }

  void InitMines()
  {
    remainingMines = 5;
    remainingBlocks = 31;
    spottedMines = 0;

    for (int k = 0; k < 5; k++)
    {
      int i = UnityEngine.Random.Range(0, 6);
      int j = UnityEngine.Random.Range(0, 6);
      if (blocks[i][j].isMine)
      {
        k--;
      }
      else
      {
        blocks[i][j].isMine = true;
        for (int l = i - 1; l <= i + 1; l++)
        {
          for (int m = j - 1; m <= j + 1; m++)
          {
            if (l >= 0 && l < 6 && m >= 0 && m < 6)
            {
              blocks[l][m].mineCount++;
            }
          }
        }
      }
    }
  }

  void ResetBlocks()
  {
    for (int i = 0; i < 6; i++)
    {
      for (int j = 0; j < 6; j++)
      {
        blocks[i][j].Reset();
      }
    }
  }

  public void MineralDestroyed()
  {
    remainingMines--;
    remainingMinesIndicator.text = remainingMines.ToString();
    if (remainingBlocks == 0 && remainingMines == 0)
    {
      Restart();
    }
  }

  public void Restart()
  {
    score += spottedMines * 15;
    scoreIndicator.text = score.ToString();
    ResetBlocks();
    InitMines();
    remainingMinesIndicator.text = remainingMines.ToString();
  }

  public void BlockRevealed(int positionX, int positionY)
  {
    remainingBlocks--;
    if (remainingBlocks == 0 && remainingMines == 0)
    {
      Restart();
      return;
    }
    if (blocks[positionX][positionY].mineCount == 0)
    {
      for (int i = positionX - 1; i <= positionX + 1; i++)
      {
        for (int j = positionY - 1; j <= positionY + 1; j++)
        {
          if (i >= 0 && i < 6 && j >= 0 && j < 6 && !blocks[i][j].isRevealed)
          {
            blocks[i][j].RevealBlock();
          }
        }
      }
    }
  }

  public void BlockFlagged(bool isMine)
  {
    remainingMines--;
    remainingMinesIndicator.text = remainingMines.ToString();
    if (isMine)
    {
      spottedMines++;
      if (remainingBlocks == 0 && remainingMines == 0)
      {
        Restart();
      }
    }
  }

  public void BlockUnflagged(bool isMine)
  {
    if (isMine)
    {
      spottedMines--;
    }
    remainingMines++;
  }
}