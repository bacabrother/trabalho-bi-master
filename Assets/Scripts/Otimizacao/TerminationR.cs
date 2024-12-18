using GeneticSharp;
using UnityEngine;
public class TerminationR : TerminationBase
{
    readonly int maxGenerations;
    public TerminationR(int maxGenerations)
    {
        this.maxGenerations = maxGenerations;
    }
    protected override bool PerformHasReached(IGeneticAlgorithm geneticAlgorithm)
    {
        CromossomoR cromo = geneticAlgorithm.BestChromosome as CromossomoR;
        return cromo.CanStop() || geneticAlgorithm.GenerationsNumber > maxGenerations;
    }
}
