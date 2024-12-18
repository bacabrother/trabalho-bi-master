using System.Text;
using UnityEngine;

public class InputPredecessor : BaseObject, IExcelExportImport
{
    //id = idPoçoSucessor|idPredecessor|TipoSucessor|TipoPredecessor
    public string idPocoSucessor;
    public string idPocoPredecessor;
    public TipoAtividade tipoSucessor;
    public TipoAtividade tipoPredecessor;
    public int folga;
    public bool obrigatorio;
    public InputPredecessor() { }
    public InputPredecessor(string idPocoSuc, string idPocoPred, TipoAtividade tipoSucessor, TipoAtividade tipoPredecessor, int folga, bool obrigatorio)
    {
        this.idPocoPredecessor = idPocoSuc;
        this.idPocoSucessor = idPocoSuc;
        this.tipoPredecessor = tipoPredecessor;
        this.tipoSucessor = tipoSucessor;
        this.obrigatorio = obrigatorio;
        this.folga = folga;
        this.id = idPocoPredecessor + "|" + idPocoSucessor + "|" + tipoSucessor + "|" + tipoPredecessor;
    }
    public void Init()
    {
        InputAtividade pocoSucessor = Manager.inputs.inputPoco[idPocoSucessor].GetActivity(tipoSucessor);
        InputAtividade pocoPredecessor = Manager.inputs.inputPoco[idPocoPredecessor].GetActivity(tipoPredecessor);
        pocoSucessor.SetPredecessor(pocoPredecessor, folga, obrigatorio);
        pocoPredecessor.SetSucessor(pocoSucessor, folga, obrigatorio);
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
            "Predecessor",
            "Tipo Sucessor",
            "Tipo Predecessor",
            "Folga",
            "Obrigatorio"
   };
    }
    public string GetFieldValue(string fieldName)
    {
        switch (fieldName)
        {
            case "Id": return id.ToString();
            case "Poço": return idPocoSucessor.ToString();
            case "Predecessor": return idPocoPredecessor.ToString();
            case "Tipo Sucessor": return tipoSucessor.ToString().Replace("_", " ");
            case "Tipo Predecessor": return tipoPredecessor.ToString().Replace("_", " ");
            case "Folga": return folga.ToString();
            case "Obrigatorio": return obrigatorio.ToString();
        }
        return "";
    }
    public void SetFieldValue(string fieldName, string value)
    {
        switch (fieldName)
        {
            case "Id": id = value; break;
            case "Poço": idPocoSucessor = value; break;
            case "Predecessor": idPocoPredecessor = value; break;
            case "Tipo Sucessor": tipoSucessor.SetValueByString(value); break;
            case "Tipo Predecessor": tipoPredecessor.SetValueByString(value); break;
            case "Folga": folga.SetValueByString(value); break;
            case "Obrigatorio": obrigatorio.SetValueByString(value); break;
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