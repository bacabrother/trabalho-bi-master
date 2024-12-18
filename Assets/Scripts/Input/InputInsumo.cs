using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
//TODO
[System.Serializable]
public class InputInsumo : BaseObject, IExcelExportImport
{
    //id = Atividade|Tipo
    public TipoAtividade atividade;
    public TipoInsumo tipo;
    public int quantidade;
    public int tempoNecessidade;
    public bool consumivel;

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
        return new string[6] {
        "Id",
        "Atividade",
        "Tipo",
        "Quantidade",
        "Tempo Relativo",
        "Consumível",
        };
    }
    public string GetFieldValue(string fieldName)
    {
        switch (fieldName)
        {
            case "Id": return id.ToString();
            case "Atividade": return atividade.ToString();
            case "Tipo": return tipo.ToString().Replace("_", " ");
            case "Quantidade": return quantidade.ToString();
            case "Tempo Relativo": return tempoNecessidade.ToString();
            case "Consumível": return consumivel.ToString();
        }
        return "";
    }
    public void SetFieldValue(string fieldName, string value)
    {
        switch (fieldName)
        {
            case "Id": id = value; break;
            case "Atividade": atividade.SetValueByString(value); break;
            case "Tipo": tipo.SetValueByString(value); break;
            case "Quantidade": quantidade.SetValueByString(value); break;
            case "Tempo Relativo": tempoNecessidade.SetValueByString(value); break;
            case "Consumível": consumivel.SetValueByString(value); break;
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
