using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework.Constraints;
using TMPro;
using UnityEngine;
public class Manager : Singleton<Manager>
{
    public TMP_Text pathInputLabel, errorLabel, statusLabel;
    public MenuBt newPathBt, gerarCronogramaBt, exportBt, logBt, gerarAlgoritmoGeneticoBt;

    public List<Cronograma> cronogramas;
    public static InputDados inputs;
    public static long aproxCost;

    public bool pathValidado;
    public bool inputProcessado;
    public bool cronogramaGerado;

    void Start()
    {
        inputs = new InputDados();
        IOManager.Init();
        OnPathSelected();
        cronogramas = new List<Cronograma>();
        SetButtons();
    }

    #region Animate/Label/Status
    private float timeStatusLabel = 0.2f;
    private float nextTimeStatusLabel = 0;
    private int statusCount = 0;
    private string originalStatusText;
    private void FixedUpdate()
    {
        if (Time.time > nextTimeStatusLabel)
        {
            AnimateStatusLabel();
        }
    }
    private void AnimateStatusLabel()
    {
        nextTimeStatusLabel = Time.time + timeStatusLabel;
        statusCount++;
        if (statusCount >= 4) statusCount = 0;
        switch (statusCount)
        {
            case 0:
                statusLabel.text = originalStatusText;
                break;
            case 1:
                statusLabel.text = originalStatusText + ".";
                break;
            case 2:
                statusLabel.text = originalStatusText + "..";
                break;
            case 3:
                statusLabel.text = originalStatusText + "...";
                break;
            case 4:
                statusLabel.text = originalStatusText + "....";
                break;
        }
    }
    public void SetStatus()
    {
        if (!pathValidado) SetStatusLabel("Selecione um novo input");
        else if (pathValidado && !inputProcessado) SetStatusLabel("Selecione um novo Input!");
        else if (pathValidado && inputProcessado) SetStatusLabel("Aguardando geração de um novo cronograma");
        else if (cronogramaGerado) SetStatusLabel("Cronograma tá pronto! Exporte ou simule novamente!");
    }
    public void SetStatusLabel(string text)
    {
        originalStatusText = text;
        statusLabel.text = text;
    }
    public void SetPathLabel()
    {
        pathInputLabel.text = $"Input: {IOManager.pathExcelInput}";
    }
    public void SetError(string error)
    {
        if (string.IsNullOrWhiteSpace(error))
        {
            errorLabel.text = "";
            return;
        }
        string errorText = DateTime.Now.ToString() + ": " + error;
        errorLabel.text = errorText;
        Debug.Log(errorText);
    }
    #endregion

    public void SetButtons()
    {
        newPathBt.button.onClick.AddListener(() =>
        {
            IOManager.GetNewExcelInput(OnPathSelected);
        });
        gerarAlgoritmoGeneticoBt.button.onClick.AddListener(() =>
        {
            GeracaoAlgoritmoGenetico();
        });
        gerarCronogramaBt.button.onClick.AddListener(() =>
        {
            GeracaoCronogramaAleatorio();
        });
        logBt.button.onClick.AddListener(() =>
        {
            LogCronograma();
        });
        exportBt.button.onClick.AddListener(() =>
        {
            if (cronogramas == null || cronogramas.Count == 0)
            {
                Debug.Log("Sem cronogramas");
                return;
            }
            var cron = cronogramas.OrderBy(x => x.custo).ToList();
            for (int i = 0; i < Premissas.Instance.cronogramasExportados; i++)
            {
                if (i >= cron.Count) return;
                ExportCronograma(cron[i]);
            }
        });
    }
    public void OnPathSelected()
    {
        ValidarPath();
        SetStatus();
        SetPathLabel();
        ProcessarInputs();
    }
    public void ValidarPath()
    {
        pathValidado = false;
        if (string.IsNullOrWhiteSpace(IOManager.pathExcelInput))
        {
            return;
        }
        if (!File.Exists(IOManager.pathExcelInput))
        {
            SetError("O excel não existe! Selecione um novo input!!");
            return;
        }
        string validacao = ExcelUtils.ValidarExcel(IOManager.pathExcelInput);
        if (!string.IsNullOrWhiteSpace(validacao))
        {
            SetError(validacao);
            return;
        }
        SetError("");
        pathValidado = true;
    }
    public void ProcessarInputs()
    {
        ValidarPath();
        if (pathValidado)
        {
            inputs.ProcessInput(IOManager.pathExcelInput, () =>
            {
                inputProcessado = true;
            });
        }
        SetStatus();
    }
    public void GeracaoCronogramaAleatorio()
    {
        cronogramas = new List<Cronograma>();
        float timer = Time.realtimeSinceStartup;
        Debug.Log("Init Gerar Cronograma Aleatorio");
        for (int i = 0; i < Premissas.Instance.cronogramasGerados; i++)
        {
            Cronograma cronograma = new Cronograma();
            cronograma.Create(Premissas.Instance.generatorMode);
            if (cronograma.valido)
                cronogramas.Add(cronograma);
        }
        Debug.Log($"Finish.. Cronogramas validos: {cronogramas.Count} -- Total time: {Time.realtimeSinceStartup - timer}");
        LogCronograma();
        cronogramaGerado = true;
        SetStatus();
    }
    public void GeracaoAlgoritmoGenetico()
    {
        aproxCost = inputs.arrayAtividades.Sum(x => x.duration) * inputs.arraySondas.Sum(x => x.valorDiaria) / inputs.arraySondas.Length;
        Debug.Log($"Base cost {aproxCost}");
        IAManager newM = new IAManager();
        cronogramas = newM.RunIA();
        cronogramaGerado = true;
        SetStatus();
    }
    public void LogCronograma()
    {
        if (cronogramas == null || cronogramas.Count == 0)
        {
            Debug.Log("Sem cronogramas");
            return;
        }
        cronogramas.OrderBy(x => x.custo).ToList()[0].LogCronograma();
    }
    public void ExportCronograma(Cronograma crono)
    {
        List<InputCrono> inputs = new List<InputCrono>();
        foreach (var atv in crono.atividades.Values)
        {
            InputCrono inputcrono = new InputCrono();
            inputcrono.FromAtividade(atv);
            inputs.Add(inputcrono);
        }
        List<Cronograma> newList = new List<Cronograma>();
        newList.Add(crono);
        ExcelUtils.ExportToExcel2(inputs, "Atividades", newList, "Cronograma", Premissas.Instance.exportName);
    }

}
