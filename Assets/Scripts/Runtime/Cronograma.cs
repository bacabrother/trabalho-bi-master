using System;
using System.Collections.Generic;
using System.Linq;
using GeneticSharp;
using UnityEngine;

public class Cronograma : BaseObject, IExcelExportImport
{
    public Sonda[] sondas;
    public Dictionary<string, Atividade> atividades;

    public long custo;
    public long baseCost; //sem multas
    public long multasPredecessor;
    public long multasPerfuracoesQuebradas;
    public int predecessorObrigatorioDescumprido;
    public int folgaDescumprida;
    public int perfuracoesQuebradas;
    public int sondasDiasExtra;
    public int totalDiasExtra;
    public bool perfeito => predecessorObrigatorioDescumprido == 0 && folgaDescumprida == 0 && perfuracoesQuebradas == 0 && sondasDiasExtra == 0;
    public bool valido => predecessorObrigatorioDescumprido == 0;

    public Cronograma()
    {
        this.id = "";
    }
    public long Create(GeneretorMode mode)
    {
        Init();
        switch (mode)
        {
            case GeneretorMode.Random:
                return GenerateRandom();
            case GeneretorMode.Pool_SemPredecessor:
                return GeneratePool_SemPredecessor();
            case GeneretorMode.DoisPools:
                return GenerateDoisPools();
        }
        return long.MaxValue;
    }
    private void Init()
    {
        CreateDictionary();
        ResetMarkers();
    }
    private void ResetMarkers()
    {
        custo = 0;
        baseCost = 0;
        multasPredecessor = 0;
        multasPerfuracoesQuebradas = 0;
        predecessorObrigatorioDescumprido = 0;
        perfuracoesQuebradas = 0;
        folgaDescumprida = 0;
        sondasDiasExtra = 0;
        totalDiasExtra = 0;
    }
    private long GenerateRandom()
    {
        int countAtividades = Manager.inputs.arrayAtividades.Length;
        int countSondas = Manager.inputs.arraySondas.Length;
        int[] Order = Utils.GetUniqueInts(countAtividades, 0, countAtividades);
        int[] SondaSplit = Utils.GetUniqueIntsOrdered(countSondas - 1, 1, countAtividades, true);
        return LoadCronograma(Order, SondaSplit);
    }
    private long GeneratePool_SemPredecessor()
    {
        CreateSondas();
        //Lista de atividades que podem ser alocadas
        Dictionary<string, Atividade> activityPool = new Dictionary<string, Atividade>();
        PreencherListaAtividades(atividades, activityPool, true, false, PreencherListFilter.SemPredecessor);

        for (int i = 0; i < atividades.Count; i++)
        {
            Sonda currentSonda = GetNextSonda(sondas);
            (bool success, Atividade currentAtividade) = GetNextAtividade(currentSonda, activityPool, true);
            currentSonda.AlocarAtividade(currentSonda.nextOrder, currentAtividade, atividades);
            activityPool.Remove(currentAtividade.id);

            foreach (var successor in currentAtividade.inputAtividade.sucessors)
            {
                Atividade SuccessorAtividade = atividades[successor.predecessorOrSucessorId];
                if (!SuccessorAtividade.SemPredecessor) continue;
                if (SuccessorAtividade.alocado) continue;
                if (SuccessorAtividade.noPoolPrincipal) continue;

                activityPool.Add(SuccessorAtividade.id, SuccessorAtividade);
                SuccessorAtividade.noPoolPrincipal = true;
            }
        }
        FinishCreateCronograma();
        return custo;
    }
    /// <summary>
    /// Lista principal apenas com atividades que possuem sucessores + algumas que não possuem predecessora
    /// </summary>
    /// <returns></returns>
    private long GenerateDoisPools()
    {
        CreateSondas();
        //Lista de atividades que podem ser alocadas
        Dictionary<string, Atividade> activityPoolSecondary = new Dictionary<string, Atividade>(); //pool secundário
        PreencherListaAtividades(atividades, activityPoolSecondary, false, false, PreencherListFilter.SemPredecessor_SemSucessor);

        Dictionary<string, Atividade> activityPool = new Dictionary<string, Atividade>(); //pool principal
        PreencherListaAtividades(atividades, activityPool, true, false, PreencherListFilter.SemPredecessor_ComSucessor);

        Atividade currentAtividade = null;
        bool success = false;

        for (int i = 0; i < atividades.Count; i++)
        {
            Sonda currentSonda = GetNextSonda(sondas);
            (success, currentAtividade) = GetNextAtividade(currentSonda, activityPool, true);
            if (!success && activityPoolSecondary.Count > 0)
            {
                (success, currentAtividade) = GetNextAtividade(currentSonda, activityPoolSecondary, true);
                activityPoolSecondary.Remove(currentAtividade.id);
            }
            else
            {
                activityPool.Remove(currentAtividade.id);
            }

            currentSonda.AlocarAtividade(currentSonda.nextOrder, currentAtividade, atividades);

            foreach (var successor in currentAtividade.inputAtividade.sucessors)
            {
                Atividade SuccessorAtividade = atividades[successor.predecessorOrSucessorId];
                if (!SuccessorAtividade.SemPredecessor) continue;
                if (SuccessorAtividade.alocado) continue;
                if (SuccessorAtividade.noPoolPrincipal || SuccessorAtividade.noPoolSecundario) continue;

                bool successorCompletacaoSeguida = SuccessorAtividade.inputPoco.id == currentAtividade.inputPoco.id && SuccessorAtividade.inputAtividade.tipo == TipoAtividade.Completação;
                if (!successorCompletacaoSeguida && SuccessorAtividade.SemSucessor)
                {
                    //Vai para o pool secundário
                    activityPoolSecondary.Add(SuccessorAtividade.id, SuccessorAtividade);
                    SuccessorAtividade.noPoolSecundario = true;
                }
                else
                {
                    //vai para o pool principal
                    activityPool.Add(SuccessorAtividade.id, SuccessorAtividade);
                    SuccessorAtividade.noPoolPrincipal = true;
                }
            }
        }
        FinishCreateCronograma();
        return custo;
    }
    private long LoadCronograma(int[] geneAtividades, int[] geneSondas)
    {
        if (geneAtividades.Length != Manager.inputs.arrayAtividades.Length)
        {
            Debug.Log("Order com tamanho diferente do esperado");
            return long.MaxValue;
        }
        if (geneSondas.Length != Manager.inputs.arraySondas.Length - 1)
        {
            Debug.Log("Sonda com tamanho diferente do esperado");
            return long.MaxValue;
        }

        Init();
        int counter = 0;
        sondas = new Sonda[geneSondas.Length + 1];
        for (int sondaIndex = 0; sondaIndex <= geneSondas.Length; sondaIndex++)
        {
            Sonda newSonda = new Sonda(Manager.inputs.arraySondas[sondaIndex].id);
            sondas[sondaIndex] = newSonda;
            int breaker = sondaIndex == geneSondas.Length ? int.MaxValue : geneSondas[sondaIndex];
            for (int orderIndex = counter; orderIndex < geneAtividades.Length; orderIndex++)
            {
                if (counter == breaker) break;
                int activityIndex = geneAtividades[orderIndex];
                newSonda.AlocarAtividade(orderIndex, atividades[Manager.inputs.arrayAtividades[activityIndex].id], atividades);
                counter++;
            }
        }
        FinishCreateCronograma();
        return custo;
    }

    #region Generate Helper
    /// <param name="check"> Indica se vai verificar se pode alocar a atividade pela data mínima</param>
    /// <returns>First -> success (atendeu ao critério de data mínima)...</returns>
    private (bool, Atividade) GetNextAtividade(Sonda sonda, Dictionary<string, Atividade> list, bool check)
    {
        if (list == null || list.Count == 0) return (false, null);
        List<Atividade> newList = Enumerable.ToList(list.Values);
        int atividadeIndex = -1;
        for (int i = 0; i < list.Count; i++)
        {
            atividadeIndex = Utils.GetRandomIndex(null, 0, newList.Count);
            if (!check) return (false, newList[atividadeIndex]);

            if (sonda.nextDate >= newList[atividadeIndex].dataMinima)
            {
                return (true, newList[atividadeIndex]);
            }
            else
            {
                newList.RemoveAt(atividadeIndex);
            }
        }

        if (!check) return (false, null);
        return GetNextAtividade(sonda, list, false);
    }
    private int GetNextSondaIndex(Sonda[] sondas)
    {
        DateTime minTime = DateTime.MaxValue;
        int returnValue = -1;
        for (int i = 0; i < sondas.Length; i++)
        {
            if (sondas[i].nextDate < minTime && sondas[i].nextDate < sondas[i].inputSonda.desmobilizacao)
            {
                minTime = sondas[i].nextDate;
                returnValue = i;
            }
        }
        //Não achou sonda... vamos sem restrição de data, no random!
        if (returnValue == -1) returnValue = Utils.GetRandomValue(0, sondas.Length);
        if (returnValue == -1)
        {
            Debug.Log("Não encontrou sonda");
        }
        return returnValue;
    }
    private Sonda GetNextSonda(Sonda[] sondas)
    {
        return sondas[GetNextSondaIndex(sondas)];
    }
    private int GetNextSonda(Sonda[] sondas, Atividade atividade)
    {
        DateTime minTime = DateTime.MaxValue;
        int returnValue = -1;
        for (int i = 0; i < sondas.Length; i++)
        {
            if (sondas[i].nextDate < minTime && sondas[i].PodeAlocar(atividade))
            {
                minTime = sondas[i].nextDate;
                returnValue = i;
            }
        }
        //Não achou sonda... vamos sem restrição de data, no random!
        if (returnValue == -1) returnValue = Utils.GetRandomValue(0, sondas.Length);
        return returnValue;
    }
    #endregion

    #region Helper
    public Atividade GetActivity(int index)
    {
        int lastRemaining = index;
        int remaning = index;
        foreach (var sonda in sondas)
        {
            remaning -= sonda.eventos.Count;
            if (remaning < 0)
            {
                return sonda.eventos[lastRemaining];
            }
            lastRemaining = remaning;
        }
        return null;
    }
    private void CreateSondas()
    {
        int sondasCount = Manager.inputs.arraySondas.Length;
        sondas = new Sonda[sondasCount];
        for (int i = 0; i < sondasCount; i++)
        {
            Sonda newSonda = new Sonda(Manager.inputs.arraySondas[i].id);
            sondas[i] = newSonda;
        }
    }
    private void PreencherListaAtividades(Dictionary<string, Atividade> baseAct, Dictionary<string, Atividade> dict, bool poolPrincipal, bool neg, PreencherListFilter filter)
    {
        foreach (var item in baseAct)
        {
            if (!CheckFilter(neg, filter, item.Value)) continue;
            dict.Add(item.Value.id, item.Value);
            //list.Add(item.Value);
            if (poolPrincipal) item.Value.noPoolPrincipal = true;
            else item.Value.noPoolSecundario = true;
        }
    }
    /// <summary>
    /// True -> canPass, False -> GoToNewLoop
    /// </summary>
    /// <returns></returns>
    private bool CheckFilter(bool neg, PreencherListFilter filter, Atividade atv)
    {
        bool firstCheck = false;
        switch (filter)
        {
            case PreencherListFilter.SemPredecessor_SemSucessor:
                firstCheck = atv.SemPredecessor && atv.SemSucessor;
                break;
            case PreencherListFilter.ComPredecessor_ComSucessor:
                firstCheck = !atv.SemPredecessor && !atv.SemSucessor;
                break;
            case PreencherListFilter.SemPredecessor_ComSucessor:
                firstCheck = atv.SemPredecessor && !atv.SemSucessor;
                break;
            case PreencherListFilter.ComPredecessor_SemSucessor:
                firstCheck = !atv.SemPredecessor && atv.SemSucessor;
                break;
            case PreencherListFilter.SemPredecessor:
                firstCheck = atv.SemPredecessor;
                break;
            case PreencherListFilter.SemSucessor:
                firstCheck = atv.SemSucessor;
                break;
            case PreencherListFilter.ComPredecessor:
                firstCheck = !atv.SemPredecessor;
                break;
            case PreencherListFilter.ComSucessor:
                firstCheck = !atv.SemSucessor;
                break;
            case PreencherListFilter.All:
                return true;
        }
        if (!neg && firstCheck) return true;
        else if (neg && !firstCheck) return true;
        return false;
    }
    private void CreateDictionary()
    {
        atividades = new Dictionary<string, Atividade>();
        foreach (var item in Manager.inputs.inputAtividades)
        {
            Atividade newAct = new Atividade(item.Value.id);
            atividades.Add(item.Value.id, newAct);
        }
    }
    public void LogCronograma()
    {
        Debug.Log(this.ToString());
    }
    public override string ToString()
    {
        return $"Custo: {custo} || PerfQuebrada: {perfuracoesQuebradas} || !PredecessorObrigatorio: {predecessorObrigatorioDescumprido} || FolgaDescumprida: {folgaDescumprida} || SondasExtra: {sondasDiasExtra} || TotalExtra: {totalDiasExtra}";
    }
    #endregion

    #region Finish Create
    private void FinishCreateCronograma()
    {
        ResetMarkers();
        ResetActivities();
        VerificarCompletacaoSeparada();
        ProcessarNovasDuracoes();
        CalcularCustoInterno();
    }
    private void ResetActivities()
    {
        foreach (var atv in atividades)
        {
            atv.Value.ResetActivity();
        }
    }
    private void VerificarCompletacaoSeparada()
    {
        foreach (var atv in atividades)
        {
            if (atv.Value.VerificarCompletacaoSeparada(atividades)) perfuracoesQuebradas++;
        }
    }
    private void ProcessarNovasDuracoes()
    {
        foreach (var atv in sondas)
        {
            atv.ProcessarNovasDuracoes();
        }
    }
    private void CalcularCustoInterno()
    {
        foreach (var sonda in sondas)
        {
            baseCost += sonda.Custo;
            if (sonda.diasExtra > 0)
            {
                totalDiasExtra += sonda.diasExtra;
                sondasDiasExtra++;
            }
        }
        multasPredecessor = CustoPredecessores();
        multasPerfuracoesQuebradas = perfuracoesQuebradas * Premissas.Instance.multaPerfuraçãoQuebrada;
        custo = baseCost + multasPredecessor + multasPerfuracoesQuebradas;
    }
    private long CustoPredecessores()
    {
        long cost = 0;
        foreach (var atv in atividades)
        {
            (long multa, int obrig, int folga) = atv.Value.CheckPredecessorsCost(atividades);
            predecessorObrigatorioDescumprido += obrig;
            folgaDescumprida += folga;
            cost += multa;
        }
        return cost;
    }
    #endregion

    #region Otimizar
    public long CalcularCusto()
    {
        if (IAManager.log) Debug.Log("Calculando Custo");
        FinishCreateCronograma();
        return custo;
    }
    /// <returns>First -> genes... Second -> sondas </returns>
    public (int[], int[]) ConvertToGeneInt()
    {
        int[] genes = new int[atividades.Count];
        int[] sondasGene = new int[sondas.Length - 1];
        int atvCounter = 0;
        for (int i = 0; i < sondas.Length; i++)
        {
            foreach (var atv in sondas[i].eventos)
            {
                genes[atvCounter] = atv.intId;
                atvCounter++;
            }
            if (i != sondas.Length - 1) sondasGene[i] = atvCounter;
        }
        return (genes, sondasGene);
    }
    /// <returns>First -> genes... Second -> sondas </returns>
    public (Gene[], Gene[]) ConvertToGene()
    {
        Gene[] genes = new Gene[atividades.Count];
        Gene[] sondasGene = new Gene[sondas.Length - 1];
        int atvCounter = 0;
        for (int i = 0; i < sondas.Length; i++)
        {
            foreach (var atv in sondas[i].eventos)
            {
                genes[atvCounter] = new Gene(atv.intId);
                atvCounter++;
            }
            if (i != sondas.Length - 1) sondasGene[i] = new Gene(atvCounter);
        }
        return (genes, sondasGene);
    }
    public long FromGene(Gene[] geneAtividades, Gene[] geneSondas)
    {
        int[] genes = geneAtividades.ToInt();
        int[] sondasGene = geneSondas.ToInt();
        return LoadCronograma(genes, sondasGene);
    }
    #endregion

    #region Import/Export Excel
    public string[] GetHeaderColumnNames()
    {
        return new string[12] {
            "Id",
            "Duracao",
            "Custo",
            "Valido",
            "Base Cost",
            "Multas Predecessor",
            "Multas PerfQuebrada",
            "!Predecessor Obrigatorio",
            "Perfuracoes Quebradas",
            "Folgas",
            "SondasDiasExtra",
            "TotalDiasExtra"
     };
    }
    public string GetFieldValue(string fieldName)
    {
        switch (fieldName)
        {
            case "Id": return id.ToString();
            case "Duracao": return sondas.Sum(x => x.duration).ToString();
            case "Custo": return custo.ToString();
            case "Valido": return valido.ToString();
            case "Base Cost": return baseCost.ToString();
            case "Multas Predecessor": return multasPredecessor.ToString();
            case "Multas PerfQuebrada": return multasPerfuracoesQuebradas.ToString();
            case "!Predecessor Obrigatorio": return predecessorObrigatorioDescumprido.ToString();
            case "Perfuracoes Quebradas": return perfuracoesQuebradas.ToString();
            case "Folgas": return folgaDescumprida.ToString();
            case "SondasDiasExtra": return sondasDiasExtra.ToString();
            case "TotalDiasExtra": return totalDiasExtra.ToString();
        }
        return "";
    }
    public void SetFieldValue(string fieldName, string value)
    {
        switch (fieldName)
        {
            case "Id": id = value; break;
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
