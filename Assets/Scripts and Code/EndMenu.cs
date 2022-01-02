using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndMenu : MonoBehaviour
{
    [SerializeField] Text buildVersionText;

    // Start is called before the first frame update
    void Start()
    {
        buildVersionText.text = Application.version;
        Cursor.visible = true;

        // show restart button in menu now
        PlayerPrefs.SetInt("Restart", 1);
    }

    public void Quit()
    {
        Debug.Log("You quit the game!");
        Application.Quit();
    }

    public void Menu()
    {
        StartCoroutine(LoadLevel.instance.LoadLevelIndex(0));
    }

    public void MouseHover()
    {
        AudioManager.instance.Play("MouseOver");
    }

    public void MouseClick()
    {
        AudioManager.instance.Play("MouseClick");
    }
}
