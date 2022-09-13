using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class EndMenuController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //quit game
    public void QuitGame()
{
    Application.Quit();
}

    public void goToMainMenu(){
        SceneManager.LoadScene("MainMenu");
    }

}
