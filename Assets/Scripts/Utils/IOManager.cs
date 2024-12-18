using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UnityEngine;
[System.Serializable]
public static class IOManager
{
    public static string pathExcelInput;
    public static string folderExcelInput;
    public static string excelName;

    private static string Filter_Excel = "Excel Files(.xlsx)|*.xlsx| Excel Files(*.xlsm)|*.xlsm| Excel Files(.xls)|*.xls";
    private static string Filter_PDF = "Portable Document Format (*.pdf)|*.pdf|All Files(*.*)|*.*";
    //private static System.Windows.Forms.OpenFileDialog openFileDialog1;

    public static void Init()
    {
        pathExcelInput = PlayerPrefs.GetString("pathInput");
        excelName = PlayerPrefs.GetString("excelName");
        folderExcelInput = PlayerPrefs.GetString("folderInput");
    }
    public static void GetNewExcelInput(Action callback)
    {
        pathExcelInput = GetFilePath("Escolha o Input", Filter_Excel);
        if (string.IsNullOrWhiteSpace(pathExcelInput)) return;

        excelName = GetFileNameFromPath(pathExcelInput);
        folderExcelInput = GetFolderFromPath(pathExcelInput);

        PlayerPrefs.SetString("pathInput", pathExcelInput);
        PlayerPrefs.SetString("excelName", excelName);
        PlayerPrefs.SetString("folderInput", folderExcelInput);
        if (callback != null) callback.Invoke();
    }

    #region Raw
    private static OpenFileDialog openFileDialog1;
    private static string GetFilePath(string title, string filter)
    {
        openFileDialog1 = new OpenFileDialog();
        openFileDialog1.Filter = filter;
        openFileDialog1.Title = title;
        if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
        {
            return null;
        }
        return openFileDialog1.FileName;
    }
    private static string[] GetMultipleFilePath(string title, string filter)
    {
        openFileDialog1 = new OpenFileDialog();
        openFileDialog1.Filter = filter;
        openFileDialog1.Title = title;
        openFileDialog1.Multiselect = true;
        if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
        {
            return null;
        }
        return openFileDialog1.FileNames;
    }
    private static string[] GetMultipleFileNameFromPath(string[] path)
    {
        List<string> listPath = new List<string>();
        for (int i = 0; i < path.Length; i++)
        {
            string[] strings = path[i].Split('\\');
            listPath.Add(strings.Last());
        }
        return listPath.ToArray();
    }
    private static string GetFileNameFromPath(string path)
    {
        string[] strings = path.Split('\\');
        return strings.Last();
    }
    private static string GetFileNameFromPath(string path, int index)
    {
        //index da direita para esquerda
        string[] strings = path.Split('\\');
        return strings[strings.Length - 1 - index];
    }
    private static string GetFolderFromPath(string path)
    {
        string[] strings = path.Split('\\');
        StringBuilder newString = new StringBuilder();
        for (int i = 0; i < strings.Length - 1; i++)
        {
            newString.Append(strings[i]).Append("\\");
        }
        return newString.ToString();
    }
    private static void OpenFile(string path)
    {
        if (File.Exists(path))
        {
            System.Diagnostics.Process.Start(path);
        }
    }
    #endregion
}
