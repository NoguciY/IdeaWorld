using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    public void Title_Button()
    {
        Cursor.visible = true;
        Time.timeScale = 1;
        SceneManager.LoadScene("TitleScene");
    }
}
