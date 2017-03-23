using UnityEngine;
using UnityEngine.UI;

public class ConflictController : MonoBehaviour
{
    [SerializeField] private Text messageLabel;
    [SerializeField] private GameObject blocker;
    [SerializeField] private Transform content;

    private InputViewController _inputViewController;

    public string ResolutionPolicy { get; private set; }

    void Start()
    {
        _inputViewController = GetComponent<InputViewController>();
        ResolutionPolicy = "";
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

    public void OnLatestButton()
    {
        ResolutionPolicy = "Latest";
        Hide();
        _inputViewController.Sync();
    }

    public void OnCombineButton()
    {
        ResolutionPolicy = "CombineArray";
        Hide();
        _inputViewController.Sync();
    }

    public void OnMaxButton()
    {
        ResolutionPolicy = "Max";
        Hide();
        _inputViewController.Sync();
    }

    public void OnRemoteButton()
    {
        ResolutionPolicy = "Remote";
        Hide();
        _inputViewController.Sync();
    }

    public void OnLocalButton()
    {
        ResolutionPolicy = "Local";
        Hide();
        _inputViewController.Sync();
    }

    public void OnCancelButton()
    {
        Hide();
    }
}