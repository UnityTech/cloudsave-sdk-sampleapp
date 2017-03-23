using UnityEngine;
using UnityEngine.UI;

public class DialogController : MonoBehaviour
{
    [SerializeField] private Text messageLabel;
    [SerializeField] private GameObject blocker;
    [SerializeField] private Transform content;

    void Start()
    {
        Hide();
    }

    public void Show(string message)
    {
        if (blocker.activeSelf)
        {
            messageLabel.text += message + "\n";
        }
        else
        {
            messageLabel.text = message + "\n";
            blocker.SetActive(true);

            content.localScale = Vector3.one;
        }
    }

    private void Hide()
    {
        blocker.SetActive(false);
        content.localScale = Vector3.zero;
    }

    public void OnOKButton()
    {
        Hide();
    }
}
