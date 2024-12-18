using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

//TODO
/// <summary>
/// Aba Projetos
/// </summary>
[System.Serializable]
public class InputProjeto : BaseObject, IExcelExportImport
{
    public int prioridade; //1 a xxx -> 1 é a maior
    public DateTime dataMaxima; //TODO maior data que um poço do projeto pode terminar

    public InputProjeto()
    {
    }
    #region Import/Export Excel
    public override string ToString()
    {
        StringBuilder newText = new StringBuilder();
        string[] header = GetHeaderColumnNames();
        for (int i = 0; i < header.Length; i++)
        {
            newText.Append($"{header[i]}: ").Append(GetFieldValue(header[i]));
            if (i != header.Length - 1) newText.Append(" | ");
            else newText.AppendLine();
        }
        return newText.ToString();
    }
    public string[] GetHeaderColumnNames()
    {
        return new string[3] {
            "Id",
            "Data Máxima",
            "Prioridade"
        };
    }
    public string GetFieldValue(string fieldName)
    {
        switch (fieldName)
        {
            case "Id": return id.ToString();
            case "Data Máxima": return dataMaxima.ToString();
            case "Prioridade": return prioridade.ToString();
        }
        return "";
    }
    public void SetFieldValue(string fieldName, string value)
    {
        switch (fieldName)
        {
            case "Id": id = value; displayName = value; break;
            case "Data Máxima": dataMaxima.SetValueByString(value); break;
            case "Prioridade": prioridade.SetValueByString(value); break;
        }
    }
    public void ResetFields()
    {

    }
    public void FinishImport()
    {
    }
    #endregion

}
