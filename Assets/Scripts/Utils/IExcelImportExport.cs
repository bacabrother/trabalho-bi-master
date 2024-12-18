using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IExcelExportImport
{
    /// <summary>
    /// <br>PRIMEIRO HEADER NECESSÁRIAMENTE É O PrefabId</br>
    /// </summary>
    /// <returns></returns>
    public string[] GetHeaderColumnNames();
    public string GetFieldValue(string fieldName);
    public void SetFieldValue(string fieldName, string value);
    public void ResetFields();
    public void FinishImport();
}
