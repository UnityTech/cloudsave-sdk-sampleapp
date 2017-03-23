using System.Collections.Generic;
using UnityEngine;
using UnitySocialCloudSave;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private IDataset _currentDataset;
    private InputViewController _inputViewController;
    public string _endpoint;
    private IDictionary<string, string> _endPoints = new Dictionary<string, string>();

    public IDataset CurrentDataset
    {
        get
        {
            if (_currentDataset == null)
            {
                _currentDataset = CloudSave.OpenOrCreateDataset("DefaultDataset");
            }
            return _currentDataset;
        }
        set { _currentDataset = value; }
    }

    public IDictionary<string, string> EndPoints
    {
        get { return _endPoints; }
    }

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            _endPoints.Add("eu", "http://52.68.156.199:8080");
            _endPoints.Add("us", "http://34.195.3.253:8080");
            _inputViewController = GetComponent<InputViewController>();
            _inputViewController.SwitchEndPoint();

            DontDestroyOnLoad(this);
        }
        else
        {
            if (this != Instance)
            {
                Destroy(this);
            }
        }
    }
}