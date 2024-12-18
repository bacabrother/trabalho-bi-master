using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
public enum LogicalRelationType
{
    Sucessor,
    Predecessor
}
public class LogicalRelation
{
    public string pocoId;
    public string predecessorOrSucessorId;
    public int folga;
    public bool obrigatorio;
    public LogicalRelationType relation;

    //public AtividadeOld Poco { get { return Manager.inputs.atividades[pocoId]; } }
    public InputAtividade PredecessorOrSucessor { get { return Manager.inputs.inputAtividades[predecessorOrSucessorId]; } }
    public LogicalRelation(string pocoId, string predecessorId, int folga, LogicalRelationType relation, bool obrigatorio = false)
    {
        this.pocoId = pocoId;
        this.predecessorOrSucessorId = predecessorId;
        this.folga = folga;
        this.obrigatorio = obrigatorio;
        this.relation = relation;
    }
    public LogicalRelation ShallowCopy()
    {
        return (LogicalRelation)this.MemberwiseClone();
    }
    public override string ToString()
    {
        StringBuilder newText = new StringBuilder();
        //newText.Append("Poco ").Append(Poco.Poco.displayName).Append(" | ");
        //newText.Append("PredecessorOrSucessor ").Append(PredecessorOrSucessor.Poco.displayName).Append(" | ");
        newText.Append("relation ").Append(relation).Append(" | ");
        newText.Append("folga ").Append(folga).Append(" | ");
        return newText.ToString();
    }

}
