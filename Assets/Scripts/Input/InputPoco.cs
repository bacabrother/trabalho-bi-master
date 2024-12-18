using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class InputPoco : BaseObject, IExcelExportImport
{
    public InputProjeto inputProjeto => Manager.inputs.inputProjetos[projectId];

    public string projectId;
    public bool mpd;
    private int prioridade; //quanto menor, maior a prioridade, sendo que 0 é sem prioridade
    public int perfuracaoDuration;
    public int completacaoDuration;
    public int workoverDuration;

    public List<InputAtividade> atividades;
    public float prioridadeFinal; //quanto maior, maior a prioridade

    public void Init()
    {
        atividades = new List<InputAtividade>();
        if (perfuracaoDuration > 0) atividades.Add(new InputAtividade(id, TipoAtividade.Perfuração, perfuracaoDuration));
        if (completacaoDuration > 0) atividades.Add(new InputAtividade(id, TipoAtividade.Completação, completacaoDuration));
        if (workoverDuration > 0) atividades.Add(new InputAtividade(id, TipoAtividade.Workover, workoverDuration));
        prioridadeFinal = prioridade == 0 ? prioridade : (Premissas.Instance.menorPrioridade - prioridade) * Premissas.Instance.pesoPrioridadePoco + (Premissas.Instance.menorPrioridade - inputProjeto.prioridade) * Premissas.Instance.pesoPrioridadeProjeto;
    }
    public void SetNaturalPredecessors()
    {
        if (perfuracaoDuration > 0 && completacaoDuration > 0)
        {
            //Completação sempre depois de perfuração
            InputPredecessor newPredecessor = new InputPredecessor(id, id, TipoAtividade.Completação, TipoAtividade.Perfuração, 0, true);
            Manager.inputs.inputPredecessor.Add(newPredecessor.id, newPredecessor);
        }
        if (workoverDuration > 0 && completacaoDuration > 0)
        {
            //Workover apenas se tiver completação, e só apos 180 dias
            InputPredecessor newPredecessor = new InputPredecessor(id, id, TipoAtividade.Workover, TipoAtividade.Completação, 0, true);
            Manager.inputs.inputPredecessor.Add(newPredecessor.id, newPredecessor);
        }
    }
    public InputAtividade GetActivity(TipoAtividade tipo) => atividades.Find(x => x.tipo == tipo);

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
            "Projeto",
            "Prioridade",
            "MPD",
            "Perfuração",
            "Completação",
            "Workover",
         };
    }
    public string GetFieldValue(string fieldName)
    {
        switch (fieldName)
        {
            case "Id": return id.ToString();
            case "Projeto": return projectId.ToString();
            case "Prioridade": return prioridade.ToString();
            case "MPD": return mpd.ToString();
            case "Perfuração": return perfuracaoDuration.ToString();
            case "Completação": return completacaoDuration.ToString();
            case "Workover": return workoverDuration.ToString();
        }
        return "";
    }
    public void SetFieldValue(string fieldName, string value)
    {
        switch (fieldName)
        {
            case "Id": id = value; displayName = value; break;
            case "Projeto": projectId = value; break;
            case "Prioridade": prioridade.SetValueByString(value); break;
            case "MPD": mpd.SetValueByString(value); break;
            case "Perfuração": perfuracaoDuration.SetValueByString(value); break;
            case "Completação": completacaoDuration.SetValueByString(value); break;
            case "Workover": workoverDuration.SetValueByString(value); break;
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
