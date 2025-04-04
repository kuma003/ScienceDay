using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Windows.Forms;
using UnityEngine.SceneManagement;

public class KeyEventHandler : MonoBehaviour
{
    private string currentSceneName;

    void Start()
    {
        // 現在のシーン名を初期化
        currentSceneName = SceneManager.GetActiveScene().name;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DialogResult result = MessageBox.Show(
                "終了しますか? / Are you sure to quit?",
                "終了 / Quit",
                MessageBoxButtons.YesNo
            );
            if (result == DialogResult.Yes)
            {
                UnityEngine.Application.Quit();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }
        }

        if (currentSceneName == "Launch" && Input.GetKeyDown(KeyCode.Q))
        {
            SceneManager.LoadScene("Top");
        }


    }
}