using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI; 

public class MenuController : MonoBehaviour 
{
    //переменные чтобы их юзать и потом бросать типо поюзал и бросил
    public GameObject inputJoin;
    public GameObject inputCreate;
    public GameObject playButton;
    public GameObject settingsButton;
    public GameObject shopButton;
    public GameObject buttonCreate; 
    public GameObject buttonJoin;
    
    private void Start()
    {
        //типа сначала отключены и ждут активаци€ как мой мозг
        buttonCreate.SetActive(false);
        buttonJoin.SetActive(false);
        inputJoin.SetActive(false);
        inputCreate.SetActive(false);

    }
    public void PlayButtonClicked()
    {
        // ќтключаем старые кнопки
        playButton.SetActive(false);
        settingsButton.SetActive(false);
        shopButton.SetActive(false);

        // ¬ключаем новые кнопки
        buttonCreate.SetActive(true);
        buttonJoin.SetActive(true);
        inputJoin.SetActive(true);
        inputCreate.SetActive(true);
    }
}