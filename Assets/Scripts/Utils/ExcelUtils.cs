using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public class ExcelUtils
{
    #region Init 
    public static string folderPath => $"{Application.dataPath.Replace("/Assets", "")}/Inputs/";
    public static string Path(string file) => $"{IOManager.pathOutput}/{file}"; //$"{Application.dataPath}/Inputs/Cronogramas/{file}";
    #endregion

    #region Validação
    public static string ValidarExcel(string path)
    {
        StringBuilder errorText = new StringBuilder();
        Excel xls = Read(path);
        if (xls == null)
        {
            return "PathErorr";
        }
        string[] fields = new string[0];
        bool hasPremissas = false, hasSondas = false, hasProjetos = false, hasPoco = false, hasIntervencoes = false, hasPredecessor = false, hasEntrega = false, hasTags = false;

        for (int i = 0; i < xls.Tables.Count; i++)
        {
            if (!hasSondas) errorText.Append(CheckExcel(xls.Tables[i], "Sondas", out hasSondas, new InputSonda()));
            if (!hasPoco) errorText.Append(CheckExcel(xls.Tables[i], "Poços", out hasPoco, new InputPoco()));
            if (!hasPredecessor) errorText.Append(CheckExcel(xls.Tables[i], "Predecessor", out hasPredecessor, new InputPredecessor()));
            //if (!hasProjetos) errorText.Append(CheckExcel(xls.Tables[i], "Projetos", out hasProjetos, new InputProjeto()));
        }

        //if (!hasPremissas) errorText.Append("Sheet não encontrado: ").AppendLine("PremissasCrono");
        if (!hasSondas) errorText.Append("Sheet não encontrado: ").AppendLine("Sondas");
        //if (!hasProjetos) errorText.Append("Sheet não encontrado: ").AppendLine("Projetos");
        if (!hasPoco) errorText.Append("Sheet não encontrado: ").AppendLine("Pocos");
        //if (!hasIntervencoes) errorText.Append("Sheet não encontrado: ").AppendLine("Eventos");
        if (!hasPredecessor) errorText.Append("Sheet não encontrado: ").AppendLine("Predecessor");
        //if (!hasEntrega) errorText.Append("Sheet não encontrado: ").AppendLine("EntregaANM");
        //if (!hasTags) errorText.Append("Sheet não encontrado: ").AppendLine("TagsANM");
        return errorText.ToString();
    }
    private static string CheckExcel<T>(ExcelTable table, string tableName, out bool finded, T obj, int startLine = 1) where T : IExcelExportImport
    {
        StringBuilder text = new StringBuilder();
        finded = false;
        if (table.TableName == tableName)
        {
            finded = true;
            string[] fields = obj.GetHeaderColumnNames();
            for (int c = 0; c < fields.Length; c++)
            {
                string value = table.GetValue(startLine, c + 1).ToString();
                if (value != fields[c])
                {
                    text.Append("Sheet: ").Append(table.TableName).Append(" - Coluna não encontrada: ").AppendLine(fields[c]);
                }
            }
        }
        return text.ToString();
    }
    #endregion 

    #region Generic
    public static string GetNextSaveName(string baseName, string extension = ".xlsx")
    {
        int counter = 1;
        string counterString = counter < 10 ? $"0{counter}" : counter.ToString();
        string nextName = $"{baseName}{counterString}{extension}";
        string path = Path($"{baseName}{counterString}{extension}");
        //Debug.Log(path);
        while (File.Exists((path)))
        {
            if (counter > 20000) return "";
            counter++;
            counterString = counter < 10 ? $"0{counter}" : counter.ToString();
            nextName = $"{baseName}{counterString}{extension}";
            path = Path($"{baseName}{counterString}{extension}");
        }
        return nextName;
    }
    public static string ExportToExcel<T>(List<T> list, string tableName, string fileName = "") where T : BaseObject, IExcelExportImport, new()
    {
        if (list == null || list.Count == 0) return "";
        fileName = GetNextSaveName(fileName);
        if (fileName == "")
        {
            Debug.Log("ErrorName");
            return "";
        }
        Debug.Log("Start Exporting Excel... " + fileName);
        List<ExcelTable> sheets = new List<ExcelTable>();
        sheets.Add(GetSheet<T>(list, tableName));
        string path = Path(fileName);
        Debug.Log(path);
        ExcelUtils.Write(path, sheets, false);
        return fileName;
        //Debug.Log("Finish Exporting Excel... " + fileName);
    }
    public static string ExportToExcel2<T, K>(List<T> list1, string tableName1, List<K> list2, string tableName2, string fileName = "") where T : BaseObject, IExcelExportImport, new() where K : BaseObject, IExcelExportImport, new()
    {
        if (list1 == null || list1.Count == 0) return "";
        fileName = GetNextSaveName(fileName);
        if (fileName == "")
        {
            Debug.Log("ErrorName");
            return "";
        }
        Debug.Log("Start Exporting Excel... " + fileName);
        List<ExcelTable> sheets = new List<ExcelTable>();
        sheets.Add(GetSheet<T>(list1, tableName1));
        sheets.Add(GetSheet<K>(list2, tableName2));
        string path = Path(fileName);
        //Debug.Log(path);
        ExcelUtils.Write(path, sheets, false);
        return fileName;
        //Debug.Log("Finish Exporting Excel... " + fileName);
    }
    private static ExcelTable GetSheet<T>(List<T> list, string tableName) where T : BaseObject, IExcelExportImport, new()
    {
        ExcelTable sheet = new ExcelTable();
        sheet.TableName = tableName;
        string[] fields = list[0].GetHeaderColumnNames();

        for (int i = 0; i < fields.Length; i++)
        {
            sheet.SetValue(1, i + 1, fields[i]);
        }

        int rowCounter = 2; //onde está a primera linha dos dados (número da linha do excel)
        for (int item = 0; item < list.Count; item++)
        {
            for (int column = 0; column < fields.Length; column++)
            {
                string value = list[item].GetFieldValue(fields[column]);
                if (value == "True") value = "x";
                if (value == "False") value = "";
                sheet.SetValue(rowCounter, column + 1, value);
            }
            rowCounter++;
        }
        return sheet;
    }
    private static ExcelTable GetHeaderSheet<T>(string tableName) where T : BaseObject, IExcelExportImport, new()
    {
        ExcelTable sheet = new ExcelTable();
        sheet.TableName = tableName;
        T item = new T();
        string[] fields = item.GetHeaderColumnNames();

        for (int i = 0; i < fields.Length; i++)
        {
            sheet.SetValue(1, i + 1, fields[i]);
        }
        return sheet;
    }
    //Import
    public static Dictionary<string, T> ImportExcel<T>(string path, string tableName, string folder = null) where T : BaseObject, IExcelExportImport, new()
    {
        Dictionary<string, T> list = new Dictionary<string, T>();
        Excel xls = Read(path);
        if (xls == null)
        {
            List<ExcelTable> sheets = new List<ExcelTable>();
            ExcelTable newTable = GetHeaderSheet<T>(tableName);
            sheets.Add(newTable);
            ExcelUtils.Write(path, sheets, false);
            Debug.Log("Created Excel -> Importing Excel... " + tableName);
            return null;
        }

        //Debug.Log("Start Importing Excel... " + tableName);
        int startLine = 2;

        string[] columsNames = new T().GetHeaderColumnNames();
        int colums = columsNames.Length;

        for (int table = 0; table < xls.Tables.Count; table++)
        {
            if (xls.Tables[table].TableName != tableName) continue;
            int rowsCount = xls.Tables[table].NumberOfRows;
            if (rowsCount > 10000)
            {
                Debug.LogError("10000+ rows error");
                return null;
            }
            for (int row = startLine; row <= rowsCount + startLine; row++)
            {
                string id = xls.Tables[table].GetValue(row, 1).ToString();
                if (string.IsNullOrEmpty(id)) continue;

                string prefabId = id;

                if (prefabId == "") continue;

                T scriptable = new T();
                list.Add(prefabId, scriptable);

                scriptable.ResetFields();
                scriptable.id = prefabId;

                for (int column = 0; column < colums; column++)
                {
                    string value = xls.Tables[table].GetValue(row, column + 1).ToString();
                    scriptable.SetFieldValue(columsNames[column], value);
                }

                scriptable.FinishImport();

                //TODO REORDER
            }
        }
#if UNITY_EDITOR
        AssetDatabase.SaveAssets();
#endif
        //Debug.Log("Finish Importing Excel... " + tableName);
        return list;
    }
    #endregion

    #region RawOp
    /// <summary>
    /// Entrar com o asset folder, //Assets/Folder/file.xlsx
    /// </summary>
    /// <param name="assetFolderPath"></param>
    private static bool CheckFileExcelExistsByFullPath(string path, string tableName, bool create)
    {
        string fullPath = path;
        string folderPath = Utils.GetFolderPathFromFile(path);
        Utils.CheckFolderExistsByFullPath(folderPath, true);
        bool exists = File.Exists(fullPath);
        if (exists)
        {
            return true;
        }
        if (!exists && create)
        {
            CreateEmptyExcel(fullPath, tableName);
            return true;
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// Entrar com o asset folder, //Assets/Folder/file.xlsx
    /// </summary>
    /// <param name="assetFolderPath"></param>
    private static void CheckFileExcelExistsByFullPath<T>(T item, string path, string tableName) where T : BaseObject, IExcelExportImport, new()
    {
        string fullPath = path;
        string folderPath = Utils.GetFolderPathFromFile(path);
        Utils.CheckFolderExistsByFullPath(folderPath, true);
        bool exists = File.Exists(fullPath);
        if (!exists)
        {
            CreateBaseExcel(item, ExcelUtils.folderPath, tableName);
        }
    }
    /// <summary>
    /// Cria um excel com o header do window
    /// </summary>
    /// <param name="path"></param>
    /// <param name="tableName"></param>
    private static void CreateBaseExcel<T>(T item, string path, string tableName) where T : BaseObject, IExcelExportImport, new()
    {
        Excel xls = new Excel();
        ExcelTable sheet = GetHeaderSheet<T>(tableName);
        xls.Tables.Add(sheet);
        ExcelHelper.SaveExcel(xls, path);
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }
    private static void CreateEmptyExcel(string path, string tableName)
    {
        Excel xls = new Excel();
        ExcelTable sheet = new ExcelTable();
        sheet.TableName = tableName;
        xls.Tables.Add(sheet);
        ExcelHelper.SaveExcel(xls, path);
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }
    private static Excel Read(string path)
    {
        string outputPath = path;
        if (!File.Exists(outputPath)) return null;
        Excel xls = ExcelHelper.LoadExcel(outputPath);
        //xls.ShowLog();
        return xls;
    }
    private static ExcelTable CreateTable(string sheetName, List<ExcelTableCell> cells)
    { //sheet
        ExcelTable sheet = new ExcelTable();
        sheet.TableName = sheetName;
        foreach (ExcelTableCell c in cells)
        {
            sheet.SetValue(c.RowIndex, c.ColumnIndex, c.Value);
        }
        return sheet;
    }
    private static void Write(string path, List<ExcelTable> tables, bool openExcel)
    {
        Excel xls = new Excel();
        string outputPath = path;
        string folderPath = Utils.GetFolderPathFromFile(outputPath);
        Utils.CheckFolderExistsByFullPath(folderPath, true);
        foreach (ExcelTable t in tables)
        {
            xls.Tables.Add(t);
        }
        ExcelHelper.SaveExcel(xls, outputPath);
        if (openExcel) OpenExcel(outputPath);
    }
    private static void Write(string path, ExcelTable tables)
    {
        Excel xls = new Excel();
        string outputPath = path;
        string folderPath = Utils.GetFolderPathFromFile(outputPath);
        Utils.CheckFolderExistsByFullPath(folderPath, true);
        xls.Tables.Add(tables);
        ExcelHelper.SaveExcel(xls, outputPath);
        OpenExcel(outputPath);
    }
    private static void OpenExcel(string path)
    {
        System.Diagnostics.Process.Start(path);
    }
    #endregion
}
