using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePortal : MonoBehaviour
{
    [Header("Scene")]
    public string sceneToLoad = "Level_2";

    [Header("Interaction")]
    public KeyCode interactKey = KeyCode.E;

    [Header("Loadout Selection")]
    public bool showLoadoutSelection = true;

    private bool playerInRange = false;

    private void Update()
    {
        if (Time.timeScale == 0f) return;

        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            if (showLoadoutSelection && LoadoutSelectionUI.Instance != null)
            {
                LoadoutSelectionUI.Instance.Open(sceneToLoad);
            }
            else
            {
                LoadScene();
            }
        }
    }

    private void LoadScene()
    {
        Debug.Log("Loading scene: " + sceneToLoad);
        SceneManager.LoadScene(sceneToLoad);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        playerInRange = true;

        if (UIMessage.Instance != null)
        {
            UIMessage.Instance.ShowMessage("Нажми E, чтобы перейти дальше");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        playerInRange = false;
    }
}