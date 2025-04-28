using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI; 

public class MenuController : MonoBehaviour 
{
    //���������� ����� �� ����� � ����� ������� ���� ������ � ������
    public GameObject inputJoin;
    public GameObject inputCreate;
    public GameObject playButton;
    public GameObject settingsButton;
    public GameObject shopButton;
    public GameObject buttonCreate; 
    public GameObject buttonJoin;
    
    private void Start()
    {
        //���� ������� ��������� � ���� ��������� ��� ��� ����
        buttonCreate.SetActive(false);
        buttonJoin.SetActive(false);
        inputJoin.SetActive(false);
        inputCreate.SetActive(false);

    }
    public void PlayButtonClicked()
    {
        // ��������� ������ ������
        playButton.SetActive(false);
        settingsButton.SetActive(false);
        shopButton.SetActive(false);

        // �������� ����� ������
        buttonCreate.SetActive(true);
        buttonJoin.SetActive(true);
        inputJoin.SetActive(true);
        inputCreate.SetActive(true);
    }
}