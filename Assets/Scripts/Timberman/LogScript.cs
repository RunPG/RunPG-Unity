using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogScript : MonoBehaviour
{
    //[SerializeField]
    //private Sprite sprite;
    [SerializeField]
    private GameObject branchSprite;

    public enum Branch
    {
        None,
        Left,
        Right
    }

    const int LOG_SIZE = 1;

    private Branch branch;

    private float startPos;
    private float endPos;
    private float elapsedTime = 1f;

    private void Update()
    {
        if (elapsedTime < 0.1f)
        {
            elapsedTime += Time.deltaTime;
            float height = Mathf.Lerp(startPos, endPos, elapsedTime / 0.1f);
            transform.localPosition = new Vector3(transform.localPosition.x, height, transform.localPosition.z);
        }
    }

    public void UpdatePos(int pos)
    {
        elapsedTime = 0f;
        startPos = transform.localPosition.y;
        endPos = pos * LOG_SIZE;
    }

    public void SetPos(int pos)
    {
        elapsedTime = 1f;
        transform.localPosition = new Vector3(transform.localPosition.x, pos * LOG_SIZE, transform.localPosition.z);
    }

    public void InitBranch(bool random)
    {
        if (random)
        {
            int x = Random.Range(0, 3);
            if (x < 1)
            {
                branch = Branch.Left;
                branchSprite.SetActive(true);
                branchSprite.transform.localPosition = new Vector3(-1, 0, 0);
                branchSprite.transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (x > 1)
            {
                branch = Branch.Right;
                branchSprite.SetActive(true);
                branchSprite.transform.localPosition = new Vector3(1, 0, 0);
                branchSprite.transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                branch = Branch.None;
                branchSprite.SetActive(false);
            }
        }
        else
        {
            branch = Branch.None;
            branchSprite.SetActive(false);
        }  
    }

    public bool HasLeftBranch()
    {
        return branch == Branch.Left;
    }

    public bool HasRightBranch()
    {
        return branch == Branch.Right;
    }
}
