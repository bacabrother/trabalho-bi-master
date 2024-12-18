using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GeneticSharp;
using UnityEngine;


public class AGManager
{
    public static bool log = false;
    private Premissas premissas => Premissas.Instance;
    private Thread m_gaThread;
    public List<Cronograma> CronogramaAlgoritmoGenetico()
    {
        Manager.Instance.SetStatus("Criando cronograma pelo algoritmo genético");
        float timer = Time.realtimeSinceStartup;
        List<Cronograma> cronogramas = new List<Cronograma>();
        double latestFitness = 0.0;

        var chromosome = new CromossomoR(Manager.inputs.arrayAtividades.Length);
        var population = new Population(premissas.populationMinSize, premissas.populationMaxSize, chromosome);
        var fitness = new FitnessR();
        var selection = new SelectionR(premissas.selectionNumberRandom);
        var reinsertion = new ReinserctionR(premissas.reinsertionNumberNewCromossomo, premissas.keepBestFromLastGeneration);

        CrossoverBase crossover = premissas.crossoverType switch
        {
            CrossoverType.AlternatingPositionCrossover => new AlternatingPositionCrossover(),
            CrossoverType.CycleCrossover => new CycleCrossover(),
            CrossoverType.OrderBasedCrossover => new OrderBasedCrossover(),
            CrossoverType.OrderedCrossover => new OrderedCrossover(),
            CrossoverType.PartiallyMappedCrossover => new PartiallyMappedCrossover(),
            CrossoverType.PositionBasedCrossover => new PositionBasedCrossover(),
            _ => new AlternatingPositionCrossover()
        };

        MutationBase mutation = premissas.mutationType switch
        {
            MutationType.MutationR => new MutationR(),
            MutationType.TworsMutation => new TworsMutation(),
            MutationType.DisplacementMutation => new DisplacementMutation(),
            MutationType.InsertionMutation => new InsertionMutation(),
            MutationType.PartialShuffleMutation => new PartialShuffleMutation(),
            MutationType.ReverseSequenceMutation => new ReverseSequenceMutation(),
            _ => new TworsMutation(),
        };

        TerminationBase termination = premissas.terminationType switch
        {
            TerminationType.TerminationR => new TerminationR(premissas.generations),
            TerminationType.GenerationsCount => new GenerationNumberTermination(premissas.generations),
            _ => new GenerationNumberTermination(premissas.generations)
        };

        var operatorStrategy = new DefaultOperatorsStrategy();
        //var taskExecutor = new ParallelTaskExecutor() { MinThreads = 100, MaxThreads = 100 };
        var taskExecutor = new LinearTaskExecutor();

        var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
        ga.Reinsertion = reinsertion;
        ga.OperatorsStrategy = operatorStrategy;
        ga.Termination = termination;
        ga.TaskExecutor = taskExecutor;

        if (premissas.crossoverProbability > 1) premissas.crossoverProbability = 1;
        if (premissas.mutationProbability > 1) premissas.mutationProbability = 1;

        ga.CrossoverProbability = premissas.crossoverProbability;
        ga.MutationProbability = premissas.mutationProbability;

        ga.GenerationRan += (sender, e) =>
        {
            var bestChromosome = ga.BestChromosome as CromossomoR;
            var bestFitness = bestChromosome.Fitness.Value;
            if (bestFitness != latestFitness)
            {
                latestFitness = bestFitness;
                Manager.Instance.SetStatus($"BEST FITNESS ->> Generation {ga.GenerationsNumber} -> Fitness: {bestFitness}");
                Manager.Instance.SetStatus(bestChromosome.ToString());
                cronogramas.Add(bestChromosome.cronograma);
            }
        };

        if (premissas.useThread)
        {
            m_gaThread = new Thread(() =>
            {
                try
                {
                    Thread.Sleep(1);
                    ga.Start();
                }
                catch (Exception ex)
                {
                    Manager.Instance.SetStatus($"GA thread error: {ex.Message}");
                }
            });
            m_gaThread.Start();
        }
        else
        {
            ga.Start();
        }
        cronogramas = cronogramas.OrderBy(x => x.custo).ToList();
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine($"Cronogramas gerados!  Tempo total: {Time.realtimeSinceStartup - timer} Geração: {ga.GenerationsNumber - 1}");
        if (cronogramas.Count > 0) stringBuilder.Append(cronogramas[0].ToString());
        Manager.Instance.SetStatus(stringBuilder.ToString());

        return cronogramas;
    }
}
