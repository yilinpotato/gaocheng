using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ClickBotton : MonoBehaviour
{
   public void StartBottonClick()
    {
        SceneManager.LoadScene("��ͼ");
    }
   public void EndBottonClick()
    {
        SceneManager.LoadScene("�������");
    } 
}
