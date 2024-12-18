using System;
using System.Text;
using UnityEngine;
//TODO
public class InputEstoque : BaseObject, IExcelExportImport
{
    public TipoInsumo tipo;
    public int quantidade;
    public DateTime data;

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
        return new string[4] {
            "Id",
            "Tipo",
            "Quantidade",
            "Data",
   };
    }
    public string GetFieldValue(string fieldName)
    {
        switch (fieldName)
        {
            case "Id": return id.ToString();
            case "Tipo": return tipo.ToString().Replace("_", " ");
            case "Quantidade": return quantidade.ToString();
            case "Data": return data.ToString();
        }
        return "";
    }
    public void SetFieldValue(string fieldName, string value)
    {
        switch (fieldName)
        {
            case "Id": id = value; break;
            case "Tipo": tipo.SetValueByString(value); break;
            case "Quantidade": quantidade.SetValueByString(value); break;
            case "Data": data.SetValueByString(value); break;
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

