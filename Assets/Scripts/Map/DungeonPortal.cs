using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonPortal : MonoBehaviour
{
    [SerializeField]
    private GameObject mesh;
    [SerializeField]
    private Material originMaterial;

    private long id = -1;

    private void Start()
    {
        id = long.Parse(transform.name);
        Material material = new Material(originMaterial);
        switch (id % 4)
        {
            case 0:
                material.color = Color.red;
                break;
            case 1:
                material.color = Color.grey;
                break;
            case 2:
                material.color = Color.blue;
                break;
            case 3:
                material.color = Color.green;
                break;
        }
        mesh.GetComponent<MeshRenderer>().material = material;
    }

    public bool EnterDungeon()
    {
        GameObject player = GameObject.Find("LocationBasedGame/Character");
        if (Vector3.Distance(player.transform.position, transform.position) < 50)
        {
            SceneManager.LoadScene("DungeonScene");
            return true;
        }
        else
        {
            return false;
        }
        
    }

    public void ShowInfo()
    {
        GameObject description = GameObject.Find("DungeonDescription");
        description.GetComponent<CanvasGroup>().interactable = true;
        description.GetComponent<CanvasGroup>().blocksRaycasts = true;
        description.GetComponent<CanvasGroup>().alpha = 1;
        description.GetComponent<DungeonDescription>().dungeon = gameObject;
        string title = "";
        switch (id % 4)
        {
            case 0:
                title = "Antre de Daarun";
                break;
            case 1:
                title = "La tour ténébreuse";
                break;
            case 2:
                title = "Le donjon des oubliés";
                break;
            case 3:
                title = "La forêt ensanglantée";
                break;
        }
        description.transform.Find("Background/Title").GetComponent<TextMeshProUGUI>().text = title;
    }
}
