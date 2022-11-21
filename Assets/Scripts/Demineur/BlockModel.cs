using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

class BlockModel: MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler {
  [SerializeField]
  private GameObject notRevealedObject;
  [SerializeField]
  private GameObject flagIconObject;
  [SerializeField]
  private GameObject mineIconObject;
  [SerializeField]
  private TMP_Text mineCountObject;



  private DemineurScript demineurScript;
  public bool isMine;
  public int mineCount;
  public bool isRevealed;
  public bool isFlagged;
  public int positionX;
  public int positionY;

  private bool isPointerDown;
  private bool isPointerOnBlock;

  public void Init(DemineurScript demineurScript, int positionX, int positionY) {
    this.demineurScript = demineurScript;
    this.positionX = positionX;
    this.positionY = positionY;
    this.isRevealed = false;
  }

  IEnumerator ClickCoroutine()
  {
    float elapsedTime = 0f;
    float duration = 0.3f;
    while (elapsedTime < duration)
    {
      elapsedTime += Time.deltaTime;
      if (!isPointerDown)
      {
        break;
      }
      yield return null;
    }
    if (!isPointerOnBlock)
    {
      yield break;
    }
    if (isPointerDown)
    {
      FlagBlock();
    }
    else if (!isFlagged)
    {
      RevealBlock();
    }
    isPointerOnBlock = false;
    isPointerDown = false;
  }

  public void OnPointerDown(PointerEventData eventData)
  {
    if (!demineurScript.hasStarted || demineurScript.hasEnded)
    {
      return;
    }
    if (isRevealed)
    {
      return;
    }
    isPointerDown = true;
    isPointerOnBlock = true;
    StartCoroutine(ClickCoroutine());
  }

  public void OnPointerUp(PointerEventData eventData)
  {
    isPointerDown = false;
  }

  public void OnPointerExit(PointerEventData eventData)
  {
    if (isPointerDown)
    {
      isPointerOnBlock = false;
    }
  }

  void FlagBlock()
  {
    isFlagged = !isFlagged;
    this.flagIconObject.SetActive(isFlagged);
    if (isFlagged)
      demineurScript.BlockFlagged(isMine);
    else
      demineurScript.BlockUnflagged(isMine);
  }

  public void RevealBlock()
  {
    isRevealed = true;
    if (isMine) {
      notRevealedObject.SetActive(false);
      mineIconObject.SetActive(true);
      demineurScript.MineralDestroyed();
    } else {
      this.notRevealedObject.SetActive(false);
      if (mineCount > 0) {
        this.mineCountObject.text = mineCount.ToString();
      }
      demineurScript.BlockRevealed(positionX, positionY);
    }
  }

  public void Reset()
  {
    isMine = false;
    isRevealed = false;
    isFlagged = false;
    mineCount = 0;
    notRevealedObject.SetActive(true);
    flagIconObject.SetActive(false);
    mineIconObject.SetActive(false);
    mineCountObject.text = "";
  }
}