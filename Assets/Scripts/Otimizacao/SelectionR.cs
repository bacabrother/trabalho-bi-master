using System.Collections.Generic;
using System.Linq;
using GeneticSharp;
using UnityEngine;
/// <summary>
/// Mesclado entre o EliteSelection e o RouletteWheelSelection
/// </summary>
public sealed class SelectionR : SelectionBase
{
    readonly int _selectionRandom;
    //readonly int _selectionNewChromosomes;
    List<IChromosome> _cromosommos;

    public SelectionR(int selectionRandom) : base(2)
    {
        _selectionRandom = selectionRandom;
        //_selectionNewChromosomes = selectionNewChromosomes;
    }

    #region ISelection implementation
    protected override IList<IChromosome> PerformSelectChromosomes(int number, Generation generation)
    {
        if (generation.Number == 1) _cromosommos = new List<IChromosome>();

        _cromosommos.AddRange(generation.Chromosomes);

        var ordered = _cromosommos.OrderByDescending(c => c.Fitness).ToList();

        int selectionNumber = number - _selectionRandom;
        var result = ordered.Take(selectionNumber).ToList();

        //NEW CHROMOSOME
        //for (int i = 0; i < _selectionNewChromosomes; i++)
        //{
        //    result.Add(generation.BestChromosome.CreateNew());
        //}

        //RANDOM SELECTION
        int[] random = Utils.GetUniqueInts(_selectionRandom, selectionNumber, ordered.Count);
        if (random != null)
        {
            for (int i = 0; i < _selectionRandom; i++)
            {
                result.Add(ordered[random[i]]);
            }
        }

        _cromosommos.Clear();

        return result;
    }

    #endregion
}
