using UnityEngine;
public enum TerminationType
{
    TerminationR,
    GenerationsCount,
}
public enum CrossoverType
{
    AlternatingPositionCrossover,
    CycleCrossover,
    OrderBasedCrossover,
    OrderedCrossover,
    PartiallyMappedCrossover,
    PositionBasedCrossover,
}
public enum MutationType
{
    MutationR,
    TworsMutation,
    DisplacementMutation,
    InsertionMutation,
    PartialShuffleMutation,
    ReverseSequenceMutation,
}
/// <summary>
///<br>SemPredecessor_SemSucessor</br>
///<br>ComPredecessor_ComSucessor</br>
///<br>SemPredecessor_ComSucessor</br>
///<br>ComPredecessor_SemSucessor</br>
///<br>SemPredecessor</br>
///<br>SemSucessor</br>
///<br>ComPredecessor</br>
///<br>ComSucessor</br>
///<br>All</br>
/// </summary>
public enum PreencherListFilter
{
    SemPredecessor_SemSucessor,
    ComPredecessor_ComSucessor,
    SemPredecessor_ComSucessor,
    ComPredecessor_SemSucessor,
    SemPredecessor,
    SemSucessor,
    ComPredecessor,
    ComSucessor,
    All,
}
public enum GeneretorMode
{
    Random,
    Pool_SemPredecessor,
    DoisPools,
}
public enum StatusPoco
{
    Nao_Inciado,
    Perfurado,
    Completado,
    Perfurado_THD,
    Completado_Inferior,
}
public enum TipoAtividade
{
    Perfuração,
    Completação,
    Workover,
    //Perfuração_BHD,
    //Perfuração_THD,
    //Completação_inferior,
    //Completação_superior,
    //Abandono_Definitivo,
    //Parada_Programada,
    //Avaliação,
    //Mobilizacao,
    //Ag_Programação,
    //Preparativos,
    //Recebimento,
}
public enum ActualStatus
{
    Iniciado,
    Concluido,
    NaoIniciado
}
public enum CheckPredecessorType
{
    Obrigatorias,
    NaoObrigatorias,
    Todas
}