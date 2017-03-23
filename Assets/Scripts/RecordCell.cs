using Tacticsoft;
using UnityEngine;
using UnityEngine.UI;
using UnitySocialCloudSave;

public class RecordCell : TableViewCell
{
    [SerializeField] private Text keyLabel;
    [SerializeField] private Text valueLabel;
    [SerializeField] private Text SyncRevision;
    [SerializeField] private Text dirtyLabel;

    public void Load(Record record)
    {
        keyLabel.text = record.Key;
        valueLabel.text = record.Value;
        SyncRevision.text = record.SyncRegion + ":" + record.SyncCount;
        dirtyLabel.text = record.IsDirty.ToString();
    }
}
