using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class FlexibleGridLayout : LayoutGroup
{
    [SerializeField]
    private GameObject linePrefab;

    [SerializeField]
    private ScrollRect scrollRect;

    [SerializeField]
    private GameObject resultText;

    private float cellSide = 0;

    private bool defeat;

    public void generateMap(bool? victory)
    {
        List<List<GameObject>> transforms = new List<List<GameObject>>();

        List<int> path = DungeonManager.instance.path;
        List<List<Room.RoomType>> map = DungeonManager.instance.map;

        defeat = (victory != null && victory == false);

        float parentWidth = rectTransform.rect.width;
        float parentHeight = rectTransform.rect.height;

        cellSide = parentWidth * 0.20f;

        Vector2 cellSize = new Vector2(cellSide, cellSide);

        var spacing = cellSize.y * 1.5f;
        var newParentHeight = cellSide * map.Count * 2.5f;
        if (newParentHeight > parentHeight)
        {
            parentHeight = cellSide * map.Count * 2.5f;
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, parentHeight);
        }
        else
        {
            spacing = (parentHeight - (cellSide * (map.Count - 1) + padding.bottom + padding.top)) / map.Count;
        }

        for (int i = 0; i < map.Count; i++)
        {
            transforms.Add(new List<GameObject>());
            var row = map[i];
            var dist = parentWidth / row.Count;

            for (int j = 0; j < row.Count; j++)
            {
                int rowCount = i;
                int columnCount = j;

                GameObject gameObject = new GameObject();
                var item = Instantiate(gameObject, this.transform);
                DestroyImmediate(gameObject);

                var itemImage = item.AddComponent<Image>();
                itemImage.sprite = Room.getRoomSprite(row[j]);

                var xPos = (dist / 2) - (cellSize.x / 2) + (dist * columnCount);
                var yPos = parentHeight - ((cellSize.y * (rowCount + 1) + (spacing * rowCount) + padding.bottom));

                var itemRect = item.GetComponent<RectTransform>();
                SetChildAlongAxis(itemRect, 0, xPos, cellSize.x);
                SetChildAlongAxis(itemRect, 1, yPos, cellSize.y);

                if (path.Count -1 == i && path[i] == j)
                {
                    GameObject selectionGameObject = new GameObject();
                    var selectionItem = Instantiate(selectionGameObject, this.transform);
                    DestroyImmediate(selectionGameObject);

                    var selectionImage = selectionItem.AddComponent<Image>();
                    selectionImage.sprite = Resources.Load<Sprite>("Dungeon/Selected");

                    selectionImage.color = defeat ? new Color32(204, 40, 30, 255) : new Color32(63, 72, 204, 255);
                    itemImage.color = defeat ? new Color32(204, 40, 30, 255) : new Color32(63, 72, 204, 255);


                    var selectionItemRect = selectionItem.GetComponent<RectTransform>();
                    SetChildAlongAxis(selectionItemRect, 0, xPos - (cellSize.x/2), cellSize.x * 2);
                    SetChildAlongAxis(selectionItemRect, 1, yPos - (cellSize.x / 2), cellSize.y * 2);
                }
                else if (i < path.Count)
                {
                    if (i < path.Count && j == path[i])
                        itemImage.color = defeat ? new Color32(204, 40, 30, 122) : new Color32(63, 72, 204, 122);
                    else
                        itemImage.color = new Color32(0, 0, 0, 122);
                }
                else
                {
                    itemImage.color = defeat ? new Color32(0, 0, 0, 122) : new Color32(0, 0, 0, 255);
                }
                transforms[i].Add(item);
            }
        }
        for (int i = 0; i < transforms.Count - 1; i++)
        {
            var line = transforms[i];
            for (int j = 0; j < line.Count; j++)
            {
                if (line.Count == 1)
                {
                    for (int k = 0; k < transforms[i+1].Count; k++)
                    {
                        createLine(line[j], transforms[i + 1][k], getLineStatus(j, k, i, transforms[i + 1][k]));
                    }
                }
                else if (line.Count == 2)
                {
                    if (transforms[i + 1].Count <= 2)
                    {
                        for (int k = 0; k < transforms[i + 1].Count; k++)
                        {
                            createLine(line[j], transforms[i + 1][k], getLineStatus(j, k, i, transforms[i + 1][k]));
                        }
                    }
                    else
                    {
                        createLine(line[j], transforms[i + 1][j], getLineStatus(j, j, i, transforms[i + 1][j]));
                        createLine(line[j], transforms[i + 1][j + 1], getLineStatus(j, j + 1, i, transforms[i + 1][j + 1]));
                    }
                }
                else
                {
                    if (transforms[i + 1].Count == 1 || j == 1)
                    {
                        for (int k = 0; k < transforms[i + 1].Count; k++)
                        {
                            createLine(line[j], transforms[i + 1][k], getLineStatus(j, k, i, transforms[i + 1][k]));
                        }
                    }
                    else if (transforms[i + 1].Count == 2)
                    {
                        var target = j == 0 ? 0 : 1;
                        createLine(line[j], transforms[i + 1][target], getLineStatus(j, target, i, transforms[i + 1][target]));
                    }
                    else
                    {
                        var target = j == 0 ? 0 : 1;
                        createLine(line[j], transforms[i + 1][target], getLineStatus(j, target, i, transforms[i + 1][target]));
                        createLine(line[j], transforms[i + 1][target + 1], getLineStatus(j, target + 1, i, transforms[i + 1][target + 1]));
                    }
                }
            }
        }
        StartCoroutine(AutoScroll(2f, false));
    }

    private int getLineStatus(int fromIndex, int toIndex, int currentLine, GameObject target)
    {
        List<int> path = DungeonManager.instance.path;
        if (currentLine + 1 < path.Count)
        {
            return fromIndex == path[currentLine] && toIndex == path[currentLine + 1] ? defeat ? 4 : 3 : 1;
        }
        else if (currentLine < path.Count)
        {
            if (fromIndex == path[currentLine] && !defeat)
            {
                var itemButton = target.AddComponent<Button>();
                itemButton.onClick.AddListener(delegate {
                    DungeonMap.LoadLevel(DungeonManager.instance.currentFloor + 1);
                    DungeonManager.instance.path.Add(toIndex);
                });
            }
            return fromIndex == path[currentLine] && !defeat ? 2 : 1;
        }
        return defeat ? 1 : 0;
    }

    private void createLine(GameObject start, GameObject end, int status)
    {
        GameObject lineGameObject = Instantiate(linePrefab, this.transform);
        lineGameObject.transform.SetAsFirstSibling();
        UILineRendererList line = lineGameObject.GetComponent<UILineRendererList>();

        Vector2 startPosition = new Vector2(start.transform.localPosition.x, start.transform.localPosition.y + (cellSide / 2));
        Vector2 endPosition = new Vector2(end.transform.localPosition.x, end.transform.localPosition.y - (cellSide / 2));
        line.AddPoint(startPosition);
        line.AddPoint(endPosition);
        line.Resolution = Vector2.Distance(startPosition, endPosition) / 100;

        switch (status)
        {
            case 0:
                line.color = new Color32(0, 0, 0, 255);
                break;
            case 1:
                line.color = new Color32(0, 0, 0, 122);
                break;
            case 2:
                line.color = new Color32(0, 13, 200, 255);
                break;
            case 3:
                line.color = new Color32(63, 72, 204, 122);
                break;
            case 4:
                line.color = new Color32(204, 40, 30, 122);
                break;
            default:
                break;
        }
    }

    public override void CalculateLayoutInputVertical()
    {
    }

    public override void SetLayoutHorizontal()
    {
    }

    public override void SetLayoutVertical()
    {
    }

    public IEnumerator AutoScroll(float duration, bool direction = true)
    {
        float t0 = 0.0f;
        while (t0 < 1.0f)
        {
            t0 += Time.deltaTime / duration;
            if (direction)
                scrollRect.verticalNormalizedPosition = Mathf.Lerp(0f, 1f, t0);
            else
                scrollRect.verticalNormalizedPosition = Mathf.Lerp(1f, 0f, t0);
            yield return null;
        }
    }

    public void createVictoryText()
    {
        GameObject tmp = new GameObject();
        var tmpItem = Instantiate(tmp, this.transform);
        var text = tmpItem.AddComponent<TextMeshProUGUI>();
        text.text = "Victory";
        text.color = new Color32(0, 13, 200, 255);
        text.fontSize = 200;
        text.alignment = TextAlignmentOptions.Center;
        DestroyImmediate(tmp);
        var textRect = tmpItem.GetComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(rectTransform.rect.width, 200);
        SetChildAlongAxis(textRect, 1, 50);
        SetChildAlongAxis(textRect, 0, 0);
    }

    public void createDefeatText()
    {
        GameObject tmp = new GameObject();
        var tmpItem = Instantiate(tmp, this.transform);
        var text = tmpItem.AddComponent<TextMeshProUGUI>();
        text.text = "Defeat";
        text.color = new Color32(204, 40, 30, 255);
        text.fontSize = 200;
        text.alignment = TextAlignmentOptions.Center;
        DestroyImmediate(tmp);
        var textRect = tmpItem.GetComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(rectTransform.rect.width, 200);
        SetChildAlongAxis(textRect, 1, 50);
        SetChildAlongAxis(textRect, 0, 0);
    }
}
