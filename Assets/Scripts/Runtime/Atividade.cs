using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Atividade : BaseObject
{
    public InputAtividade inputAtividade => Manager.inputs.inputAtividades[id];
    public InputPoco inputPoco => inputAtividade.inputPoco;
    public int intId;
    public bool alocado;
    public string sondaId;
    public int ordemNaSonda;
    public DateTime start;
    public DateTime finish;
    public int duracaoReal;
    public int badPosition = 1; //quanto mais próximo de 1, melhor

    private List<LogicalRelation> predecessorsNaoAlocados; //remover a medida que for alocando
    private List<LogicalRelation> sucessoresNaoAlocados; //remover a medida que for alocando

    public DateTime dataMinima; //representa a data mínima que o poço pode ser feito, considera a folga das restrições
    public bool SemPredecessor => predecessorsNaoAlocados.Count == 0;
    public bool SemSucessor => sucessoresNaoAlocados.Count == 0;

    public bool noPoolPrincipal; //apenas runtime
    public bool noPoolSecundario;

    public Atividade() { }
    public Atividade(string id)
    {
        intId = int.Parse(id);
        this.id = id;
        predecessorsNaoAlocados = inputAtividade.predecessors.ToList();
        sucessoresNaoAlocados = inputAtividade.sucessors.ToList();
        duracaoReal = inputAtividade.duration;
        if (predecessorsNaoAlocados.Count == 0) dataMinima = DateTime.MinValue;
        else dataMinima = DateTime.MaxValue;
    }
    public DateTime AlocarAtividade(string sondaId, int ordemNaSonda, DateTime start, Dictionary<string, Atividade> activityList)
    {
        alocado = true;
        this.sondaId = sondaId;
        this.ordemNaSonda = ordemNaSonda;
        this.start = start;
        this.finish = start.AddDays(duracaoReal);
        //alocar na sonda agora
        foreach (var successor in inputAtividade.sucessors)
        {
            activityList[successor.PredecessorOrSucessor.id].SetPredecessorAlocado(this);
        }
        foreach (var predecessor in inputAtividade.predecessors)
        {
            activityList[predecessor.PredecessorOrSucessor.id].SetSucessorAlocado(this);
        }
        return this.finish.AddDays(1);
    }
    public DateTime ReprocessarData(DateTime start)
    {
        this.start = start;
        this.finish = start.AddDays(duracaoReal);
        return this.finish.AddDays(1);
    }
    /// <summary>
    /// Chamar apenas após colocar a data de finish
    /// </summary>
    public void SetPredecessorAlocado(Atividade predecessor)
    {
        int index = predecessorsNaoAlocados.FindIndex(x => x.predecessorOrSucessorId == predecessor.id);
        if (index == -1) return;
        DateTime novaDataMinima = predecessor.finish.AddDays(predecessorsNaoAlocados[index].folga);
        if (dataMinima > novaDataMinima) dataMinima = novaDataMinima;
        predecessorsNaoAlocados.RemoveAt(index);
    }
    public void SetSucessorAlocado(Atividade predecessor)
    {
        int index = sucessoresNaoAlocados.FindIndex(x => x.predecessorOrSucessorId == predecessor.id);
        if (index == -1) return;
        sucessoresNaoAlocados.RemoveAt(index);
    }
    public void ResetActivity()
    {
        badPosition = 1;
    }
    /// <returns>First -> cost... Second -> obrigatorioDescumprido... Third -> folgaDescumprida</returns>
    public (long, int, int) CheckPredecessorsCost(Dictionary<string, Atividade> atvList)
    {
        long cost = 0;
        int obrigatorioDescumprido = 0;
        int folgaDescumprida = 0;
        foreach (var item in inputAtividade.predecessors)
        {
            Atividade predecessor = atvList[item.predecessorOrSucessorId];
            int folga = (int)(start - predecessor.finish).TotalDays;
            int folgaDaFolga = folga - item.folga;
            long predecessorCost = 0;
            if (folgaDaFolga < 0)
            {
                if (item.obrigatorio)
                {
                    predecessorCost += Premissas.Instance.multaPredecessorObrigatorio;
                    obrigatorioDescumprido++;
                    badPosition += Premissas.Instance.badPositionPredObrigatorio;
                    predecessor.badPosition += Premissas.Instance.badPositionPredObrigatorio;
                }
                else
                {
                    predecessorCost += Premissas.Instance.MultaPredecessor(folga, item.folga);
                    folgaDescumprida++;
                    badPosition += Premissas.Instance.badPositionFolgaDescumprida;
                    predecessor.badPosition += Premissas.Instance.badPositionFolgaDescumprida;
                }
            }
            cost += predecessorCost;
        }
        return (cost, obrigatorioDescumprido, folgaDescumprida);
    }
    public bool VerificarCompletacaoSeparada(Dictionary<string, Atividade> atvList)
    {
        //A propria completação já vai alterar a duração da perfuração
        if (inputAtividade.tipo != TipoAtividade.Completação) return false;
        if (inputPoco.perfuracaoDuration <= 0) return false;
        //Id da perfuração sempre é -1 da completação
        string perfId = (intId - 1).ToString();
        Atividade perf = atvList[perfId];
        if (perf.sondaId == this.sondaId && perf.ordemNaSonda == this.ordemNaSonda - 1) return false;
        badPosition += Premissas.Instance.badPositionCompletacaoQuebrada;
        perf.badPosition += Premissas.Instance.badPositionCompletacaoQuebrada;
        perf.duracaoReal = perf.inputAtividade.duration + Premissas.Instance.diasAbandono;
        this.duracaoReal = inputAtividade.duration + Premissas.Instance.diaReentrada;
        return true;
    }
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("Poço: ").Append(inputPoco.id);
        sb.Append(" Tipo: ").Append(inputAtividade.tipo);
        sb.Append(" Duration: ").Append(inputAtividade.duration);
        sb.Append(" Start: ").Append(start);
        sb.Append(" Finish: ").Append(finish);
        sb.Append(" Bad: ").Append(badPosition);
        return sb.ToString();
    }
}
