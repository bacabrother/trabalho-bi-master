using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
public class Sonda : BaseObject
{
    public InputSonda inputSonda => Manager.inputs.inputSondas[id];
    public List<Atividade> eventos;

    public DateTime nextDate;
    public int duration;
    public long Custo => GetCost();
    public int nextOrder => eventos.Count;

    public int diasExtra;
    public Sonda(string id)
    {
        this.id = id;
        eventos = new List<Atividade>();
        nextDate = inputSonda.mobilizacao;
        duration = 0;
    }
    public bool PodeAlocar(Atividade atividade) => nextDate.AddDays(atividade.duracaoReal) < inputSonda.desmobilizacao;
    public void AlocarAtividade(int ordemNaSonda, Atividade atividade, Dictionary<string, Atividade> activityList)
    {
        eventos.Add(atividade);
        nextDate = atividade.AlocarAtividade(id, ordemNaSonda, nextDate, activityList);
        duration += atividade.duracaoReal;
    }
    /// <summary>
    /// Feito no final do cronograma, para atualizar data devido a mudança de duração
    /// </summary>
    public void ProcessarNovasDuracoes()
    {
        nextDate = inputSonda.mobilizacao;
        duration = 0;
        foreach (var item in eventos)
        {
            nextDate = item.ReprocessarData(nextDate);
            duration += item.duracaoReal;
        }
    }
    private long GetCost()
    {
        long cost = 0;
        cost += CustoDiario();
        cost += CustoDataAlemDesmobilizacao();
        return cost;
    }
    private long CustoDiario()
    {
        return duration * inputSonda.valorDiaria;
    }
    private long CustoDataAlemDesmobilizacao()
    {
        if (nextDate <= inputSonda.desmobilizacao) return 0;
        eventos.Last().badPosition += Premissas.Instance.badPositionAtividadeExtraSonda;
        int diasExtras = (int)(nextDate - inputSonda.desmobilizacao).TotalDays;
        this.diasExtra = diasExtras;
        long custo = (long)((1 + diasExtras * Premissas.Instance.XDiariaPorDiaExtra) * inputSonda.valorDiaria * diasExtras);
        long multa = (long)(Premissas.Instance.XMultaDiariaExtra * inputSonda.valorDiaria);
        //Debug.Log($"Next: {nextDate} Desmob: {inputSonda.desmobilizacao} DiasExtra: {diasExtras} Cost: {custo} Multa: {multa}");
        return custo + multa;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("Sonda: ").AppendLine(id);
        sb.Append("Atividades: ").AppendLine(eventos.Count.ToString());
        sb.Append("End Date: ").AppendLine(nextDate.ToString());
        foreach (var activity in eventos)
        {
            sb.AppendLine(activity.ToString());
        }
        return sb.ToString();
    }
}
