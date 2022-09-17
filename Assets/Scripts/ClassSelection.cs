using RunPG.Multi;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClassSelection : MonoBehaviour
{
    [SerializeField]
    private HeroClass[] classId;

    [SerializeField]
    private TextMeshProUGUI className;

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
        index = modIndex(index - 1, classId.Length);
        classImage.sprite = classId[index].GetSprite();
        className.text = classId[index].GetName();

    }

    public void Right()
    {
        index = (index + 1) % classId.Length;
        classImage.sprite = classId[index].GetSprite();
        className.text = classId[index].GetName();
    }

    public async void Choose()
    {
        NewUserModel newUser = new NewUserModel(PlayerProfile.pseudo, PlayerProfile.guid, PlayerProfile.email, PlayerProfile.serverAuthCode, classId[index].ToString());
        if (await Requests.POSTNewUser(newUser))
        {
            UserModel user = await Requests.GETUserByName(PlayerProfile.pseudo);
            PlayerProfile.id = user.id;
            PlayerProfile.character = await Requests.GETUserCharacter(PlayerProfile.id);
            SceneManager.LoadScene("MapScene");
        }
        else
        {
            Debug.LogError("ahhhhh on est fichu !");
        }
        
    }
}
