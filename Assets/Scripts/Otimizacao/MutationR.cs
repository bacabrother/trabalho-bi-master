using System.ComponentModel;
using System.Linq;
using GeneticSharp;

namespace GeneticSharp
{
    public class MutationR : MutationBase
    {
        #region Constructors
        public MutationR()
        {
            IsOrdered = true;
        }
        #endregion

        #region Methods
        protected override void PerformMutate(IChromosome chromosome, float probability)
        {
            if (RandomizationProvider.Current.GetDouble() <= probability)
            {
                CromossomoR cromo = chromosome as CromossomoR;
                var genes = chromosome.GetGenes();
                var firstIndexWeighted = RandomizationProvider.Current.GetInt(0, cromo.sumBadPosition); //1º com peso
                var secondIndex = RandomizationProvider.Current.GetInt(0, genes.Length); //2º random
                if (secondIndex == firstIndexWeighted) return;
                (var firstGene, var firstIndex) = genes.GetGeneWeighted(firstIndexWeighted);
                var secondGene = genes[secondIndex];

                chromosome.ReplaceGene(firstIndex, secondGene);
                chromosome.ReplaceGene(secondIndex, firstGene);
            }
        }
        #endregion
    }
}
