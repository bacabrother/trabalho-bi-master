using System.Collections.Generic;
using UnityEngine;

//Fica dentro do InputPoco
public class InputAtividade : BaseObject
{
    public static int counter = 0;

    public InputPoco inputPoco => Manager.inputs.inputPoco[pocoId];

    public string pocoId;
    public TipoAtividade tipo;
    public int duration;
    public List<LogicalRelation> predecessors;
    public List<LogicalRelation> sucessors;

    public InputAtividade(string pocoId, TipoAtividade tipo, int duration)
    {
        this.id = counter.ToString();
        counter++;
        this.pocoId = pocoId;
        this.tipo = tipo;
        this.duration = duration;
        predecessors = new List<LogicalRelation>();
        sucessors = new List<LogicalRelation>();
    }
    public void SetSucessor(InputAtividade poco, int folga, bool obrigatorio)
    {
        sucessors.Add(new LogicalRelation(inputPoco.id, poco.id, folga, LogicalRelationType.Sucessor, obrigatorio));
    }
    public void SetPredecessor(InputAtividade poco, int folga, bool obrigatorio)
    {
        predecessors.Add(new LogicalRelation(inputPoco.id, poco.id, folga, LogicalRelationType.Predecessor, obrigatorio));
    }

}
