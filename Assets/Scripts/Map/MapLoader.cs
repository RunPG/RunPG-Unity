using Mapbox.Unity.Map;
using Mapbox.Unity.MeshGeneration.Data;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MapLoader : MonoBehaviour
{
  [SerializeField]
  private AbstractMap map;

  [SerializeField]
  private Sprite[] sprites;

  [SerializeField]
  private Image spriteImage;

  [SerializeField]
  private int fps;

  [SerializeField]
  private Animator animator;

  int spriteIndex = 0;

  int finishedTile = 0;
  int totalTiles = 81;

  float step;

  bool isLoaded = false;

  private void Awake()
  {
    map.OnTileFinished += CountTile;
    step = 1.0f / fps;
    StartCoroutine(Anim());
  }

  private void CountTile(UnityTile tile)
  {
    finishedTile++;
    if (finishedTile == totalTiles)
    {
      map.OnTileFinished -= CountTile;
      isLoaded = true;
    }
  }

  private IEnumerator Anim()
  {
    int i = 0;
    while (true)
    {
      if (i == 80)
      {
        animator.SetTrigger("Start");
      }
      yield return new WaitForSeconds(step);
      spriteIndex = (spriteIndex + 1) % sprites.Length;
      spriteImage.sprite = sprites[spriteIndex];
      i++;
    }
  }

  private void FinishLoad()
  {
    Destroy(gameObject);
  }
}
