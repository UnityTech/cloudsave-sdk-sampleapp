using UnityEngine;
using UnitySocialCloudSave;

public class SwitchDatasetDialogController : MonoBehaviour
{
    [SerializeField] private GameObject _blocker;
    [SerializeField] private Transform _dialog;

    private string _datasetName;

    public void Show()
    {
        _blocker.SetActive(true);
        _dialog.localScale = Vector3.one;
    }

    private void Hide()
    {
        _blocker.SetActive(false);
        _dialog.localScale = Vector3.zero;
    }

    public void DatasetInput(string datasetName)
    {
        _datasetName = datasetName;
    }

    public void OnOKBtn()
    {
        GameManager.Instance.CurrentDataset = CloudSave.OpenOrCreateDataset(_datasetName);
        GetComponent<InputViewController>().SwitchDataset();
        Hide();
    }

    public void OnCancelBtn()
    {
        Hide();
    }

    public void Start()
    {
        gameObject.AddComponent<GameManager>();
        Hide();
    }
}