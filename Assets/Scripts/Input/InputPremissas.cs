using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public enum TipoPremissa
{
    Reentrada,
    Abandono,
    Dual,
    THD,
    PrimeiroSonda,
}
//TODO
[System.Serializable]
public class InputPremissas : BaseObject, IExcelExportImport
{
    public TipoPremissa tipo;
    public int dias;

    #region Import/Export Excel
    public override string ToString()
    {
        StringBuilder newText = new StringBuilder();
        newText.Append("Id: ").Append(id).Append(" | ");
        newText.Append("Tipo: ").Append(tipo).Append(" | ");
        newText.Append("Dias: ").AppendLine(dias.ToString());
        return newText.ToString();
    }
    public string[] GetHeaderColumnNames()
    {
        return new string[3] {
            "Id",
            "Tipo",
            "Dias",
  };
    }
    public string GetFieldValue(string fieldName)
    {
        switch (fieldName)
        {
            case "Id": return id.ToString();
            case "Tipo": return tipo.ToString();
            case "Dias": return dias.ToString();
        }
        return "";
    }
    public void SetFieldValue(string fieldName, string value)
    {
        Debug.Log(fieldName);
        switch (fieldName)
        {
            case "Id": id = value; break;
            case "Tipo": tipo.SetValueByString(value); break;
            case "Dias": dias.SetValueByString(value); break;
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
