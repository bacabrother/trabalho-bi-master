using System;
using System.Collections.Generic;
using System.Windows.Forms.VisualStyles;
using GeneticSharp;
using NUnit.Framework;
using UnityEngine;

public class CromossomoR : IChromosome, IComparable<IChromosome>
{
    #region Base
    public Cronograma cronograma;
    private Gene[] gene_atividades;
    private Gene[] gene_sondas;
    private float mutationCounter;
    private int m_length;
    public int sumBadPosition;

    public double? Fitness { get; set; }
    public int Length => m_length;
    public CromossomoR(int length, bool createEmpty)
    {
        length = Manager.inputs.arrayAtividades.Length;
        ValidateLength(length);
        m_length = length;
        cronograma = new Cronograma();
        gene_atividades = new Gene[length];
        sumBadPosition = length;
    }
    public CromossomoR(int length)
    {
        length = Manager.inputs.arrayAtividades.Length;
        ValidateLength(length);
        m_length = length;
        cronograma = new Cronograma();
        SetFitness(cronograma.Create(GeneretorMode.Pool_SemPredecessor));
        CreateGenes();
        SetBadPosition();
    }
    public IChromosome CreateNew()
    {
        if (AGManager.log) Debug.Log("Create New");
        CromossomoR newCromo = new CromossomoR(Manager.inputs.arrayAtividades.Length);
        return newCromo;
    }
    protected virtual void CreateGenes()
    {
        if (AGManager.log) Debug.Log("Create Genes");
        (gene_atividades, gene_sondas) = cronograma.ConvertToGene();
    }
    public Gene GenerateGene(int geneIndex)
    {
        if (AGManager.log) Debug.Log("Generate Gene");
        return new Gene(geneIndex);
    }
    public void ReplaceGene(int index, Gene gene)
    {
        if (AGManager.log) Debug.Log("Replace Gene");
        if (index < 0 || index >= m_length)
        {
            throw new ArgumentOutOfRangeException("index", "There is no Gene on index {0} to be replaced.".With(index));
        }
        mutationCounter += 0.5f;
        gene_atividades[index] = gene;
        Fitness = null;
    }
    public void ReplaceGenes(int startIndex, Gene[] genes)
    {
        if (AGManager.log) Debug.Log("Replace Genes");
        ExceptionHelper.ThrowIfNull("genes", genes);
        if (genes.Length != 0)
        {
            if (startIndex < 0 || startIndex >= m_length)
            {
                throw new ArgumentOutOfRangeException("startIndex", "There is no Gene on index {0} to be replaced.".With(startIndex));
            }

            int num = genes.Length;
            int num2 = m_length - startIndex;
            if (num > num2)
            {
                throw new ArgumentException("Gene", "The number of genes to be replaced is greater than available space, there is {0} genes between the index {1} and the end of chromosome, but there is {2} genes to be replaced.".With(num2, startIndex, num));
            }

            Array.Copy(genes, 0, gene_atividades, startIndex, genes.Length);
            Fitness = null;
        }
    }
    public Gene GetGene(int index)
    {
        if (AGManager.log) Debug.Log("Get Gene");
        return gene_atividades[index];
    }
    public Gene[] GetGenes()
    {
        if (AGManager.log) Debug.Log("Get Genes");
        return gene_atividades;
    }
    public Gene[] GetGenesSonda()
    {
        if (AGManager.log) Debug.Log("Get Genes");
        return gene_sondas;
    }
    void IChromosome.MarkGeneration(int generation, float time)
    {
        cronograma.SetGeneration(generation, Mathf.RoundToInt(mutationCounter), time);
    }
    #endregion

    #region Utils
    public override string ToString()
    {
        return cronograma.ToString();
    }
    public bool CanStop()
    {
        return cronograma.perfeito;
    }
    public double EvaluateFitness()
    {
        long custo = cronograma.FromGene(gene_atividades, gene_sondas);
        SetBadPosition();
        Fitness = (Manager.aproxCost) / (double)custo;// - cronograma.predecessorObrigatorioDescumprido - cronograma.perfuracoesQuebradas / 10f - cronograma.folgaDescumprida / 25f - cronograma.sondasDiasExtra - cronograma.sondasDiasExtra / 100f;
        return Fitness.Value;
    }
    private void SetBadPosition()
    {
        sumBadPosition = 0;
        for (int i = 0; i < gene_atividades.Length; i++)
        {
            gene_atividades[i].badPosition = cronograma.GetActivity(i).badPosition;
            sumBadPosition += gene_atividades[i].badPosition;
        }
    }
    public double SetFitness(long custo)
    {
        Fitness = Manager.aproxCost / (double)custo - cronograma.predecessorObrigatorioDescumprido - cronograma.perfuracoesQuebradas / 10f;
        return Fitness.Value;
    }
    public virtual IChromosome Clone()
    {
        if (AGManager.log) Debug.Log("Clone");
        CromossomoR cromossomoR = new CromossomoR(Manager.inputs.arrayAtividades.Length, true);
        cromossomoR.SetFitness(cromossomoR.cronograma.FromGene(gene_atividades, gene_sondas));
        cromossomoR.CreateGenes();
        cromossomoR.SetBadPosition();
        cromossomoR.mutationCounter = mutationCounter;
        cromossomoR.cronograma.SetGeneration(cronograma.generationCreated, cronograma.mutations, cronograma.timeToCreate);
        return cromossomoR;
    }
    public int CompareTo(IChromosome other)
    {
        if (AGManager.log) Debug.Log("Compare To");
        if (other == null)
        {
            return -1;
        }

        double? fitness = other.Fitness;
        if (Fitness == fitness)
        {
            return 0;
        }

        if (!(Fitness > fitness))
        {
            return -1;
        }

        return 1;
    }
    public override bool Equals(object obj)
    {
        if (AGManager.log) Debug.Log("Equals");
        if (!(obj is IChromosome other))
        {
            return false;
        }
        return CompareTo(other) == 0;
    }
    public override int GetHashCode()
    {
        if (AGManager.log) Debug.Log("Hascode");
        return Fitness.GetHashCode();
    }
    public void Resize(int newLength)
    {
        if (AGManager.log) Debug.Log("Resize");
        ValidateLength(newLength);
        Array.Resize(ref gene_atividades, newLength);
        m_length = newLength;
    }
    private static void ValidateLength(int length)
    {
        if (length < 2)
        {
            throw new ArgumentException("The minimum length for a chromosome is 2 genes.", "length");
        }
    }



    public static bool operator ==(CromossomoR first, CromossomoR second)
    {
        if ((object)first == second)
        {
            return true;
        }

        if ((object)first == null || (object)second == null)
        {
            return false;
        }

        return first.CompareTo(second) == 0;
    }
    public static bool operator !=(CromossomoR first, CromossomoR second)
    {
        return !(first == second);
    }
    public static bool operator <(CromossomoR first, CromossomoR second)
    {
        if ((object)first == second)
        {
            return false;
        }

        if ((object)first == null)
        {
            return true;
        }

        if ((object)second == null)
        {
            return false;
        }

        return first.CompareTo(second) < 0;
    }
    public static bool operator >(CromossomoR first, CromossomoR second)
    {
        if (!(first == second))
        {
            return !(first < second);
        }

        return false;
    }

    #endregion
}
