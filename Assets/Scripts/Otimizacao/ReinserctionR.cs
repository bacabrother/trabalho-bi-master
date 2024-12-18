using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GeneticSharp;
using UnityEngine;

/// <summary>
/// Baseado no Elitist, porém mantém os melhores do parent e cria novos
/// </summary>
public class ReinserctionR : ReinsertionBase
{
    #region Constructors
    readonly int _numberNewCromossomo;
    readonly int _numberKeepBestFitness;

    public ReinserctionR(int numberNewCromossomo, int numberKeepBestFitness) : base(true, true)
    {
        _numberKeepBestFitness = numberKeepBestFitness;
        _numberNewCromossomo = numberNewCromossomo;
    }
    #endregion

    #region Methods
    protected override IList<IChromosome> PerformSelectChromosomes(IPopulation population, IList<IChromosome> offspring, IList<IChromosome> parents)
    {
        for (int i = 0; i < _numberNewCromossomo; i++)
        {
            offspring.Add(offspring[0].CreateNew());
        }

        if (_numberKeepBestFitness > 0)
        {
            var bestParents = parents.OrderByDescending(p => p.Fitness).Take(_numberKeepBestFitness).ToList();

            for (int i = 0; i < bestParents.Count; i++)
            {
                offspring.Add(bestParents[i]);
            }
        }

        int diff = population.MinSize - offspring.Count;
        for (int i = 0; i < diff; i++)
        {
            offspring.Add(offspring[0].CreateNew());
        }

        return offspring;
        #endregion
    }

}