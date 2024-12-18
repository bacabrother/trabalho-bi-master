using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputDados
{
    public Dictionary<string, InputProjeto> inputProjetos; //TODO
    public Dictionary<string, InputSonda> inputSondas;
    public Dictionary<string, InputPoco> inputPoco;
    public Dictionary<string, InputAtividade> inputAtividades;
    public Dictionary<string, InputInsumo> inputInsumos; //TODO
    public Dictionary<string, InputEstoque> inputEstoque; //TODO
    public Dictionary<string, InputPredecessor> inputPredecessor;
    public Dictionary<string, InputCrono> inputCrono; //TODO

    public InputSonda[] arraySondas;
    public InputAtividade[] arrayAtividades;

    public InputDados()
    {
        Clear();
    }
    public void Clear()
    {
        InputAtividade.counter = 0;
        inputProjetos = new Dictionary<string, InputProjeto>();
        inputSondas = new Dictionary<string, InputSonda>();
        inputPoco = new Dictionary<string, InputPoco>();
        inputAtividades = new Dictionary<string, InputAtividade>();
        inputInsumos = new Dictionary<string, InputInsumo>();
        inputEstoque = new Dictionary<string, InputEstoque>();
        inputPredecessor = new Dictionary<string, InputPredecessor>();
        inputCrono = new Dictionary<string, InputCrono>();
        arraySondas = null;
        arrayAtividades = null;
    }
    public void ProcessInput(string path, Action callback)
    {
        Clear();
        inputProjetos = ExcelUtils.ImportExcel<InputProjeto>(path, "Projetos");
        inputSondas = ExcelUtils.ImportExcel<InputSonda>(path, "Sondas");
        inputPoco = ExcelUtils.ImportExcel<InputPoco>(path, "Poços");
        inputInsumos = ExcelUtils.ImportExcel<InputInsumo>(path, "Insumos");
        inputEstoque = ExcelUtils.ImportExcel<InputEstoque>(path, "Estoque");
        inputPredecessor = ExcelUtils.ImportExcel<InputPredecessor>(path, "Predecessor");
        inputCrono = ExcelUtils.ImportExcel<InputCrono>(path, "CronoEntrada");

        Init();
        if (callback != null) callback.Invoke();
    }
    private void Init()
    {
        //essa ordem é importante
        InitPocos();
        InitPredecessor();
        CreateArray();
    }
    private void InitPocos()
    {
        inputAtividades = new Dictionary<string, InputAtividade>();
        foreach (var item in inputPoco)
        {
            //Cria as atividades
            item.Value.Init();
            foreach (var act in item.Value.atividades)
            {
                //Adiciona as atividades na lista de atividades
                inputAtividades.Add(act.id, act);
            }
            //Colocar os predecessores naturais, como completação depende de perfuração
            item.Value.SetNaturalPredecessors();
        }
    }
    private void InitPredecessor()
    {
        //Preenche na atividade os predecessore e sucessores
        foreach (var item in inputPredecessor)
        {
            item.Value.Init();
        }
        //inputPredecessor.LogDictionary("Predecessor");
    }
    private void CreateArray()
    {
        arrayAtividades = new InputAtividade[inputAtividades.Count];
        int counter = 0;
        foreach (var item in inputAtividades)
        {
            arrayAtividades[counter] = item.Value;
            counter++;
        }
        arraySondas = new InputSonda[inputSondas.Count];
        counter = 0;
        foreach (var item in inputSondas)
        {
            arraySondas[counter] = item.Value;
            counter++;
        }
    }
}