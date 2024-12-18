using GeneticSharp;
using UnityEngine;

public class FitnessR : IFitness
{
    public FitnessR()
    {
    }
    public double Evaluate(IChromosome chromosome)
    {
        var chromo = chromosome as CromossomoR;
        return chromo.EvaluateFitness();
    }
}
