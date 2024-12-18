using NaughtyAttributes;
using UnityEngine;

public class Premissas : Singleton<Premissas>
{
    public string exportName = "Export";
    public int cronogramasExportados = 5;
    [Tooltip("Sem uso do algoritmo genético")] public int cronogramasGerados = 100; //Para uso sem IA
    public GeneretorMode generatorMode = GeneretorMode.Pool_SemPredecessor;

    //private bool showTaxaPoolExtra => generatorMode == GeneretorMode.DoisPools;
    //[ShowIf("showTaxaPoolExtra")]
    //public int taxaPoolExtra = 20; //20%

    [HorizontalLine(1, EColor.Red)]
    public bool useThread = false;
    public int generations = 1000;
    public TerminationType terminationType;
    public CrossoverType crossoverType;
    public MutationType mutationType;

    public int populationMinSize = 50;
    public int populationMaxSize = 100;

    public int keepBestFromLastGeneration = 10; //Tanto no mutation quanto no reinserction
    public int selectionNumberRandom = 3;
    public int reinsertionNumberNewCromossomo = 10; //Adicionado no reinsertion, não sofre crossover e mutação

    [Range(0, 1)] public float crossoverProbability = 0.75f; //chance de entrar para o crossover
    [Range(0, 1)] public float mutationProbability = 0.1f;

    public int badPositionPredObrigatorio = 100;
    public int badPositionCompletacaoQuebrada = 60;
    public int badPositionFolgaDescumprida = 30;
    public int badPositionAtividadeExtraSonda = 50;

    [HorizontalLine(1, EColor.Red)]
    public int MultaAbsolutaPadraoDiaria = 250000;

    #region Predecessor
    [Foldout("Predecessor")] public int XMultaPredecessorObrigatorio = 999;
    [Foldout("Predecessor")] public int XMultaPredecessor = 15;
    public long multaPredecessorObrigatorio => MultaAbsolutaPadraoDiaria * XMultaPredecessorObrigatorio;
    public long multaBasePredecessor => MultaAbsolutaPadraoDiaria * XMultaPredecessor;
    public long MultaPredecessor(int folga, int folgaRecomendada)
    {
        if (folga < 0) return multaBasePredecessor;
        else if (folga >= folgaRecomendada) return 0;
        float percentual = (float)folga / (float)folgaRecomendada;
        return (long)((1 - Mathf.Pow(percentual, 1.2f)) * multaBasePredecessor);
    }
    #endregion

    #region Desmobilização
    [Foldout("Desmobilização")] public int XMultaDiariaExtra = 15; //15 diárias
    [Foldout("Desmobilização")] public float XDiariaPorDiaExtra = 0.01f; //multiplica pelo número de dias para obter a diária final
    #endregion

    #region Perfuração Quebrada
    public long multaPerfuraçãoQuebrada => MultaAbsolutaPadraoDiaria * XMultaPerfuraçãoQuebrada;
    [Foldout("Perfuração Quebrada")] public int XMultaPerfuraçãoQuebrada = 5;
    [Foldout("Perfuração Quebrada")] public int diasAbandono = 7;
    [Foldout("Perfuração Quebrada")] public int diaReentrada = 8;

    #endregion

    #region Peso
    //TODO
    [Foldout("Peso")] public float pesoPrioridadeProjeto = 3f;
    [Foldout("Peso")] public float pesoPrioridadePoco = 6f;
    [Foldout("Peso")] public int menorPrioridade = 20;
    #endregion

}
