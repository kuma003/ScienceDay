using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NextScene : MonoBehaviour
{
    public void Update()
    {
        if (DataManager.Instance != null && DataManager.Instance.trajectory.time.Count > 0)
        {
            gameObject.GetComponent<Button>().interactable = true;
        }
        else
        {
            gameObject.GetComponent<Button>().interactable= false;
        }
    }

    // �{�^���������ꂽ�ꍇ
    public void onClick()
    {
        SceneManager.LoadScene("Launch");

    }
}
