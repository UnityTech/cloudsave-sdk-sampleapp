using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Tacticsoft;
using UnitySocialCloudSave;
using UnityEngine.SceneManagement;

public class ListViewController : MonoBehaviour, ITableViewDataSource
{
    [SerializeField] private Transform content;
    [SerializeField] private RecordCell m_cellPrefab;
    [SerializeField] private TableView m_tableView;

    private IDataset dataset;
    private Text text;
    public IList<Record> records;
    private int _rowNum;

    private bool m_requiresRefresh;

    void Start()
    {
        gameObject.AddComponent<GameManager>();
        dataset = GameManager.Instance.CurrentDataset;
        records = dataset.GetAllRecords();
        _rowNum = records.Count;

        ((List<Record>) records).Sort((x, y) => x.Key.CompareTo(y.Key));

        m_tableView.dataSource = this;
    }

    public void GoBack()
    {
        SceneManager.LoadScene("InputScene");
    }

    public int GetNumberOfRowsForTableView(TableView tableView)
    {
        return _rowNum;
    }

    //Will be called by the TableView to know what is the height of each row
    public float GetHeightForRowInTableView(TableView tableView, int row)
    {
        return (m_cellPrefab.transform as RectTransform).rect.height;
    }

    //Will be called by the TableView when a cell needs to be created for display
    public TableViewCell GetCellForRowInTableView(TableView tableView, int row)
    {
        RecordCell cell = tableView.GetReusableCell(m_cellPrefab.reuseIdentifier) as RecordCell;
        if (cell == null)
        {
            cell = Instantiate(m_cellPrefab);
        }

        cell.Load(records[row]);
        return cell;
    }
}
