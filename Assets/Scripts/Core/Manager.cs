using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework.Constraints;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Manager : Singleton<Manager>
{
    public TMP_Text pathInputLabel, pathOutputLabel, errorLabel, statusLabel;
    public MenuBt newPathBt, gerarCronogramaBt, exportPathBt, exportBt, logBt, gerarAlgoritmoGeneticoBt, salvarBt;

    public List<Cronograma> cronogramas;
    public static InputDados inputs;
    public static long aproxCost;

    public bool pathValidado;
    public bool pathOutputValidado;
    public bool inputProcessado;
    public bool cronogramaGerado;

    public Transform panel;
    public MenuInputField decimalField;
    public MenuInputField integerField;
    public MenuInputField textField;
    public MenuDropdown dropdownField;

    private List<MenuInputField> inputFieldList;
    private List<MenuDropdown> dropdownList;
    void Start()
    {
        inputs = new InputDados();
        IOManager.Init();
        CreatePremissas();
        OnPathSelected();
        OnPathOutputSelected();
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
    public void SetStatus(string text)
    {
        SetStatusLabel(text);
        Debug.Log(text);
        //if (!pathValidado) SetStatusLabel("Selecione um novo input");
        //else if (cronogramaGerado) SetStatusLabel("Cronograma tá pronto! Exporte ou simule novamente!");
        //else if (pathValidado && !inputProcessado) SetStatusLabel("Selecione um novo Input!");
        //else if (pathValidado && inputProcessado) SetStatusLabel("Aguardando geração de um novo cronograma");
    }
    public void SetStatusLabel(string text)
    {
        originalStatusText = text;
        statusLabel.text = text;
    }
    public void SetPathInputLabel()
    {
        pathInputLabel.text = $"Input: {IOManager.pathExcelInput}";
    }
    public void SetPathOutputLabel()
    {
        pathOutputLabel.text = $"Output: {IOManager.pathOutput}";
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

    #region Premissas

    #region Fields
    private MenuInputField exportName; //text
    private MenuInputField cronogramasExportados;
    private MenuInputField cronogramasGerados;
    private MenuInputField generations;
    private MenuDropdown generatorMode;
    private MenuDropdown terminationType;
    private MenuDropdown crossoverType;
    private MenuDropdown mutationType;
    private MenuInputField populationMinSize;
    private MenuInputField populationMaxSize;
    private MenuInputField keepBestFromLastGeneration;
    private MenuInputField selectionNumberRandom;
    private MenuInputField reinsertionNumberNewCromossomo;
    private MenuInputField crossoverProbability; //float
    private MenuInputField mutationProbability; //float
    private MenuInputField badPositionPredObrigatorio;
    private MenuInputField badPositionCompletacaoQuebrada;
    private MenuInputField badPositionFolgaDescumprida;
    private MenuInputField badPositionAtividadeExtraSonda;
    private MenuInputField MultaAbsolutaPadraoDiaria;
    private MenuInputField XMultaPredecessorObrigatorio;
    private MenuInputField XMultaPredecessor;
    private MenuInputField XMultaDiariaExtra; //float
    private MenuInputField XDiariaPorDiaExtra; //float
    private MenuInputField XMultaPerfuraçãoQuebrada;
    private MenuInputField diasAbandono;
    private MenuInputField diaReentrada;
    private MenuInputField pesoPrioridadeProjeto; //float
    private MenuInputField pesoPrioridadePoco; //float
    private MenuInputField menorPrioridade;
    #endregion

    public void SalvarPremissas()
    {
        foreach (var item in inputFieldList)
        {
            FieldInfo field = Utils.GetField(typeof(Premissas), item.name);
            Utils.GetField(typeof(Premissas), item.name).SetValue(Premissas.Instance, Convert.ChangeType(item.field.text, field.FieldType));
            switch (field.FieldType.ToString())
            {
                case "System.String":
                    PlayerPrefs.SetString(item.name, item.field.text);
                    break;
                case "System.Int32":
                    PlayerPrefs.SetInt(item.name, int.Parse(item.field.text));
                    break;
                case "System.Single":
                    PlayerPrefs.SetFloat(item.name, float.Parse(item.field.text));
                    break;
            }
        }
        foreach (var item in dropdownList)
        {
            Utils.GetField(typeof(Premissas), item.name).SetValue(Premissas.Instance, item.field.value);
            PlayerPrefs.SetInt(item.name, item.field.value);
        }
        SetStatus("Premissas Salvas!");
    }
    private void CreatePremissas()
    {
        inputFieldList = new List<MenuInputField>();
        dropdownList = new List<MenuDropdown>();

        exportName = CreatePrefabString(textField, panel, inputFieldList, "exportName");
        cronogramasExportados = CreatePrefabInt(integerField, panel, inputFieldList, "cronogramasExportados");
        cronogramasGerados = CreatePrefabInt(integerField, panel, inputFieldList, "cronogramasGerados");
        generations = CreatePrefabInt(integerField, panel, inputFieldList, "generations");
        generatorMode = CreatePrefabDropdown(dropdownField, panel, dropdownList, "generatorMode", GeneretorMode.Random);
        //crossoverType = CreatePrefabDropdown(dropdownField, panel, dropdownList, "crossoverType", CrossoverType.CycleCrossover);
        mutationType = CreatePrefabDropdown(dropdownField, panel, dropdownList, "mutationType", MutationType.MutationR);
        terminationType = CreatePrefabDropdown(dropdownField, panel, dropdownList, "terminationType", TerminationType.TerminationR);
        populationMinSize = CreatePrefabInt(integerField, panel, inputFieldList, "populationMinSize");
        populationMaxSize = CreatePrefabInt(integerField, panel, inputFieldList, "populationMaxSize");
        keepBestFromLastGeneration = CreatePrefabInt(integerField, panel, inputFieldList, "keepBestFromLastGeneration");
        selectionNumberRandom = CreatePrefabInt(integerField, panel, inputFieldList, "selectionNumberRandom");
        reinsertionNumberNewCromossomo = CreatePrefabInt(integerField, panel, inputFieldList, "reinsertionNumberNewCromossomo");
        crossoverProbability = CreatePrefabDecimal(decimalField, panel, inputFieldList, "crossoverProbability");
        mutationProbability = CreatePrefabDecimal(decimalField, panel, inputFieldList, "mutationProbability");
        badPositionPredObrigatorio = CreatePrefabInt(integerField, panel, inputFieldList, "badPositionPredObrigatorio");
        badPositionCompletacaoQuebrada = CreatePrefabInt(integerField, panel, inputFieldList, "badPositionCompletacaoQuebrada");
        badPositionFolgaDescumprida = CreatePrefabInt(integerField, panel, inputFieldList, "badPositionFolgaDescumprida");
        badPositionAtividadeExtraSonda = CreatePrefabInt(integerField, panel, inputFieldList, "badPositionAtividadeExtraSonda");
        MultaAbsolutaPadraoDiaria = CreatePrefabInt(integerField, panel, inputFieldList, "MultaAbsolutaPadraoDiaria");
        XMultaPredecessorObrigatorio = CreatePrefabInt(integerField, panel, inputFieldList, "XMultaPredecessorObrigatorio");
        XMultaPredecessor = CreatePrefabInt(integerField, panel, inputFieldList, "XMultaPredecessor");
        XMultaDiariaExtra = CreatePrefabInt(integerField, panel, inputFieldList, "XMultaDiariaExtra");
        XDiariaPorDiaExtra = CreatePrefabDecimal(decimalField, panel, inputFieldList, "XDiariaPorDiaExtra");
        XMultaPerfuraçãoQuebrada = CreatePrefabInt(integerField, panel, inputFieldList, "XMultaPerfuraçãoQuebrada");
        diasAbandono = CreatePrefabInt(integerField, panel, inputFieldList, "diasAbandono");
        diaReentrada = CreatePrefabInt(integerField, panel, inputFieldList, "diaReentrada");
        SalvarPremissas();
    }
    private MenuInputField CreatePrefabString(MenuInputField obj, Transform transform, List<MenuInputField> list, string name)
    {
        MenuInputField prefab = MonoBehaviour.Instantiate(obj, transform);
        string save = PlayerPrefs.GetString(name, "");
        if (save == "")
        {
            save = Utils.GetField(typeof(Premissas), name).GetValue(Premissas.Instance) as string;
        }
        prefab.field.text = save;
        prefab.name = name;
        prefab.label.text = Utils.ParseSpaceWhenUpper(name.FirstCharacterToUpper());
        list.Add(prefab);
        return prefab;
    }
    private MenuInputField CreatePrefabInt(MenuInputField obj, Transform transform, List<MenuInputField> list, string name)
    {
        MenuInputField prefab = MonoBehaviour.Instantiate(obj, transform);
        int save = PlayerPrefs.GetInt(name, int.MinValue);
        if (save == int.MinValue)
        {
            save = Convert.ToInt32(Utils.GetField(typeof(Premissas), name).GetValue(Premissas.Instance));
        }
        prefab.field.text = save.ToString();
        prefab.name = name;
        prefab.label.text = Utils.ParseSpaceWhenUpper(name.FirstCharacterToUpper());
        list.Add(prefab);
        return prefab;
    }
    private MenuInputField CreatePrefabDecimal(MenuInputField obj, Transform transform, List<MenuInputField> list, string name)
    {
        MenuInputField prefab = MonoBehaviour.Instantiate(obj, transform);
        float save = PlayerPrefs.GetFloat(name, float.MinValue);
        if (save == float.MinValue)
        {
            save = Convert.ToSingle(Utils.GetField(typeof(Premissas), name).GetValue(Premissas.Instance));
        }
        prefab.field.text = save.ToString();
        prefab.name = name;
        prefab.label.text = Utils.ParseSpaceWhenUpper(name.FirstCharacterToUpper());
        list.Add(prefab);
        return prefab;
    }
    private MenuDropdown CreatePrefabDropdown<T>(MenuDropdown obj, Transform transform, List<MenuDropdown> list, string name, T enumm) where T : Enum
    {
        MenuDropdown prefab = MonoBehaviour.Instantiate(obj, transform);

        int save = PlayerPrefs.GetInt(name, int.MinValue);
        if (save == int.MinValue)
        {
            save = Convert.ToInt32(Utils.GetField(typeof(Premissas), name).GetValue(Premissas.Instance));
        }

        var values = new List<TMP_Dropdown.OptionData>();
        foreach (T state in Enum.GetValues(typeof(T)))
        {
            TMP_Dropdown.OptionData data = new TMP_Dropdown.OptionData();
            data.text = state.ToString();
            values.Add(data);
        }
        prefab.field.options = values;
        prefab.field.value = save;
        prefab.name = name;
        prefab.label.text = Utils.ParseSpaceWhenUpper(name.FirstCharacterToUpper());
        list.Add(prefab);
        return prefab;
    }

    #endregion

    public void SetButtons()
    {
        salvarBt.button.onClick.AddListener(() =>
        {
            SalvarPremissas();
        });
        newPathBt.button.onClick.AddListener(() =>
        {
            IOManager.GetNewExcelInput(OnPathSelected);
        });
        exportPathBt.button.onClick.AddListener(() =>
        {
            IOManager.GetExportFolderPath(OnPathOutputSelected);
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
            SalvarPremissas();
            if (IOManager.pathOutput == null || IOManager.pathOutput == "")
            {
                IOManager.GetExportFolderPath(OnPathOutputSelected);
                SetStatus("Após escolher a pasta, clique em exportar novamente!");
                return;
            }
            if (cronogramas == null || cronogramas.Count == 0)
            {
                SetStatus("Sem cronogramas para exportar");
                return;
            }
            var cron = cronogramas.OrderBy(x => x.custo).ToList();
            for (int i = 0; i < Premissas.Instance.cronogramasExportados; i++)
            {
                if (i >= cron.Count) return;
                ExportCronograma(cron[i]);
            }
            SetStatus($"{Premissas.Instance.cronogramasExportados} cronogramas exportados!");
        });
    }
    public void OnPathOutputSelected()
    {
        pathOutputValidado = true;
        SetPathOutputLabel();
    }
    public void OnPathSelected()
    {
        ValidarPath();
        SetPathInputLabel();
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
            SetStatus("O excel não existe! Selecione um novo input!!");
            return;
        }
        string validacao = ExcelUtils.ValidarExcel(IOManager.pathExcelInput);
        if (!string.IsNullOrWhiteSpace(validacao))
        {
            SetStatus(validacao);
            return;
        }
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
                SetStatus("Input processado! Agora crie um novo cronograma.");
            });
        }
    }
    public void GeracaoCronogramaAleatorio()
    {
        SalvarPremissas();
        cronogramas = new List<Cronograma>();
        float timer = Time.realtimeSinceStartup;
        SetStatus($"Gerando {Premissas.Instance.cronogramasGerados} cronogramas aleatórios");
        for (int i = 0; i < Premissas.Instance.cronogramasGerados; i++)
        {
            Cronograma cronograma = new Cronograma();
            cronograma.Create(Premissas.Instance.generatorMode);
            cronograma.SetGeneration(i, 0, Time.realtimeSinceStartup - timer);
            if (cronograma.valido)
                cronogramas.Add(cronograma);
        }
        cronogramas = cronogramas.OrderBy(x => x.custo).ToList();
        StringBuilder statusText = new StringBuilder();
        statusText.Append($"Cronogramas gerados! Validos: {cronogramas.Count} - Time: {Time.realtimeSinceStartup - timer} sec.");
        if (cronogramas.Count > 0) statusText.AppendLine().Append("Melhor: ").Append(cronogramas[0].ToString());
        SetStatus(statusText.ToString());
    }
    public void GeracaoAlgoritmoGenetico()
    {
        SalvarPremissas();
        aproxCost = inputs.arrayAtividades.Sum(x => x.duration) * inputs.arraySondas.Sum(x => x.valorDiaria) / inputs.arraySondas.Length;
        Debug.Log($"Base cost {aproxCost}");
        AGManager newM = new AGManager();
        cronogramas = newM.CronogramaAlgoritmoGenetico();
    }
    public void LogCronograma()
    {
        if (cronogramas == null || cronogramas.Count == 0)
        {
            SetStatus("Sem cronogramas");
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
