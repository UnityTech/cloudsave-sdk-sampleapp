using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnitySocialCloudSave;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnitySocialCloudSave.Impl;
using UnitySocialCloudSave.Utils;
using Debug = UnityEngine.Debug;

public class InputViewController : MonoBehaviour
{
    [SerializeField] public Text datasetNameLabel;
    [SerializeField] public Text syncCountLabel;
    [SerializeField] private Dropdown _dropdown;

    private string keyText;
    private string valueText;

    private static DialogController _dialogController;
    private static SwitchDatasetDialogController _switchDatasetDialogController;
    private static ConflictController _conflictController;

    private string _endpoint = "eu";

    public IDataset dataset { get; set; }

    void Awake()
    {
        CloudSaveInitializer.AttachToGameObject(gameObject);
        gameObject.AddComponent<GameManager>();
        dataset = GameManager.Instance.CurrentDataset;
        datasetNameLabel.text = dataset.Name;
        syncCountLabel.text = SdkUtils.ConvertSyncRevisionToString(dataset.GetLastSyncRevision());

        _dialogController = GetComponent<DialogController>();
        _conflictController = GetComponent<ConflictController>();

        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        foreach (KeyValuePair<string, string> entry in GameManager.Instance.EndPoints)
        {
            options.Add(new Dropdown.OptionData(entry.Key));
        }

        _dropdown.AddOptions(options);
        
        for (int i = 0; i < options.Count; i++)
        {
            var option = options[i];
            if (option.text.Equals(GameManager.Instance._endpoint))
            {
                _dropdown.value = i;
                break;
            }
        }
    }

    public void SwitchDataset()
    {
        dataset = GameManager.Instance.CurrentDataset;
        datasetNameLabel.text = dataset.Name;
        syncCountLabel.text = SdkUtils.ConvertSyncRevisionToString(dataset.GetLastSyncRevision());
    }

    public void KeyInput(string input)
    {
        keyText = input;
    }

    public void ValueInput(string input)
    {
        valueText = input;
    }

    public void OnEndPointSwitched(Text endPoint)
    {
        _endpoint = endPoint.text;
    }

    public void SaveToLocal()
    {
        if (string.IsNullOrEmpty(keyText))
        {
            _dialogController.Show("Key should not be null");
            return;
        }
        try
        {
            dataset.Put(keyText, valueText);
            syncCountLabel.text = SdkUtils.ConvertSyncRevisionToString(dataset.GetLastSyncRevision());
        }
        catch (Exception e)
        {
            _dialogController.Show("Failed to save data to Local: " + e.Message);
            return;
        }
        _dialogController.Show("Success to save data to Local");
    }

    public void Sync()
    {
        dataset.SynchronizeAsync(new DefaultSyncCallback
        {
            syncCountLabel = syncCountLabel
        });
    }

    public void SyncOnConnectivity()
    {
        dataset.SynchronizeOnConnectivityAsync(new DefaultSyncCallback
        {
            syncCountLabel = syncCountLabel
        });
    }

    public void SyncOnWIFI()
    {
        dataset.SynchronizeOnWifiOnlyAsync(new DefaultSyncCallback
        {
            syncCountLabel = syncCountLabel
        });
    }

    public void WipeLocal()
    {
        CloudSave.WipeOut();
    }

    public void ShowRecords()
    {
        SceneManager.LoadScene("ListRecordsScene");
    }

    public void SwitchEndPoint()
    {
       var endPointField = typeof(Endpoints).GetField("CloudSaveEndpoint", BindingFlags.Static | BindingFlags.Public);
       GameManager.Instance._endpoint = _endpoint;
       endPointField.SetValue(null, GameManager.Instance.EndPoints[GameManager.Instance._endpoint]);
       Debug.LogFormat("EndPoint switch to {0}", endPointField.GetValue(null));
    }

    public class DefaultSyncCallback : ISyncCallback
    {
        public Text syncCountLabel { get; set; }

        public bool OnConflict(IDataset dataset, IList<SyncConflict> conflicts)
        {
            var resolvedRecords = new List<Record>();
            if (_conflictController.ResolutionPolicy.Equals("Latest"))
            {
                foreach (var conflict in conflicts)
                {
                    var remote = conflict.RemoteRecord;
                    var local = conflict.LocalRecord;

                    var newValue = DateTime.Compare(
                                       remote.LastModifiedDate.GetValueOrDefault(
                                           DateTime.MinValue),
                                       local.DeviceLastModifiedDate
                                           .GetValueOrDefault(DateTime.MinValue)) >=
                                   0
                        ? remote.Value
                        : local.Value;
                    resolvedRecords.Add(conflict.ResolveWithValue(newValue));
                }
            }
            else if (_conflictController.ResolutionPolicy.Equals("CombineArray"))
            {
                foreach (var conflict in conflicts)
                {
                    var remote = conflict.RemoteRecord;
                    var local = conflict.LocalRecord;

                    var newValue = string.Concat(remote.Value, local.Value);
                    resolvedRecords.Add(conflict.ResolveWithValue(newValue));
                }
            }
            else if (_conflictController.ResolutionPolicy.Equals("Max"))
            {
                foreach (var conflict in conflicts)
                {
                    var remote = conflict.RemoteRecord;
                    var local = conflict.LocalRecord;
                    var newValue = string.CompareOrdinal(remote.Value, local.Value) >= 0
                        ? remote.Value
                        : local.Value;

                    resolvedRecords.Add(conflict.ResolveWithValue(newValue));
                }
            }
            else if (_conflictController.ResolutionPolicy.Equals("Remote"))
            {
                foreach (var conflict in conflicts)
                {
                    resolvedRecords.Add(conflict.ResolveWithRemoteRecord());
                }
            }
            else if (_conflictController.ResolutionPolicy.Equals("Local"))
            {
                foreach (var conflict in conflicts)
                {
                    resolvedRecords.Add(conflict.ResolveWithLocalRecord());
                }
            }
            else
            {
                string conflictDesc = "";

                foreach (var conflict in conflicts)
                {
                    conflictDesc += string.Format("Key <{0}> (Local: <{1}>, Remote: <{2}> is in conflict.\n)",
                        conflict.RemoteRecord.Key,
                        conflict.LocalRecord.Value,
                        conflict.RemoteRecord.Value
                    );

                }
                _conflictController.Show("Conflict detected and policy not found:\n" + conflictDesc);
                return false;
            }

            dataset.ResolveConflicts(resolvedRecords);
            return true;
            
        }

        public void OnError(IDataset dataset, DatasetSyncException syncEx)
        {
            // _conflictController.Show("Sync failed: " + syncEx.Message);
            _dialogController.Show("Sync failed: " + syncEx.Message);
        }

        public void OnSuccess(IDataset dataset)
        {
            _dialogController.Show("Sync success");
            syncCountLabel.text = SdkUtils.ConvertSyncRevisionToString(dataset.GetLastSyncRevision());
        }
    }
}


