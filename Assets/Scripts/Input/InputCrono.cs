
using System;
using System.Text;
using UnityEngine;
//TODO
public class InputCrono : BaseObject, IExcelExportImport
{
    public string poco;
    public TipoAtividade atividade;
    public string sonda;
    public DateTime inicio;
    public int duration;
    public DateTime termino;

    public void FromAtividade(Atividade atv)
    {
        poco = atv.inputPoco.id;
        atividade = atv.inputAtividade.tipo;
        sonda = atv.sondaId;
        inicio = atv.start;
        duration = atv.duracaoReal;
        termino = atv.finish;
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
        return new string[7] {
            "Id",
            "Poço",
            "Atividade",
            "Sonda",
            "Data Início",
            "Duração",
            "Data Término",
    };
    }
    public string GetFieldValue(string fieldName)
    {
        switch (fieldName)
        {
            case "Id": return id.ToString();
            case "Poço": return poco.ToString();
            case "Atividade": return atividade.ToString();
            case "Sonda": return sonda.ToString();
            case "Data Início": return inicio.ToOADate().ToString();
            case "Duração": return duration.ToString();
            case "Data Término": return termino.ToOADate().ToString();
        }
        return "";
    }
    public void SetFieldValue(string fieldName, string value)
    {
        switch (fieldName)
        {
            case "Id": id = value; break;
            case "Poço": poco = value; break;
            case "Atividade": atividade.SetValueByString(value); break;
            case "Sonda": sonda = value; break;
            case "Data Início": inicio.SetValueByString(value); break;
            case "Duração": duration.SetValueByString(value); break;
            case "Data Término": termino.SetValueByString(value); break;
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
