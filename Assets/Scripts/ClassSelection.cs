using RunPG.Multi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClassSelection : MonoBehaviour
{
    [SerializeField]
    private Sprite[] sprites;

    [SerializeField]
    private HeroClass[] classId;

    [SerializeField]
    private Image classImage;

    private int index = 0;

    // modulo function that works with negative number
    internal int modIndex(int x, int m)
    {
        return ((x % m) + m) % m;
    }

    public void Left()
    {
        index = modIndex(index - 1, sprites.Length);
        classImage.sprite = sprites[index];

    }

    public void Right()
    {
        index = (index + 1) % sprites.Length;
        classImage.sprite = sprites[index];
    }

    public async void Choose()
    {
        NewUserModel newUser = new NewUserModel(PlayerProfile.pseudo, PlayerProfile.guid, classId[index].ToString());
        if (await Requests.POSTNewUser(newUser))
        {
            UserModel user = await Requests.GETUserByName(PlayerProfile.pseudo);
            PlayerProfile.id = user.id;
            SceneManager.LoadScene("MapScene");
        }
        else
        {
            Debug.LogError("ahhhhh on est fichu !");
        }
        
    }
}
