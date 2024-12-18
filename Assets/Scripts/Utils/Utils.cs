using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using GeneticSharp;
using Unity.VisualScripting;
using UnityEngine;

public static class Utils
{
    public static void LogDictionary<T>(this Dictionary<string, T> obj, string header) where T : class
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(header);
        foreach (var item in obj)
        {
            sb.Append(item.ToString());
        }
        Debug.Log(sb.ToString());
    }
    public static Gene[] ToGene(this int[] ints)
    {
        Gene[] genes = new Gene[ints.Length];
        for (int i = 0; i < genes.Length; i++)
        {
            genes[i] = new Gene(ints[i]);
        }
        return genes;
    }
    public static int[] ToInt(this Gene[] genes)
    {
        int[] ints = new int[genes.Length];
        for (int i = 0; i < ints.Length; i++)
        {
            ints[i] = (int)genes[i].Value;
        }
        return ints;
    }

    #region Types Import/Export Excel
    public static void SetValueByString(this ref int value, string str)
    {
        if (str == null || str == "") return;
        value = int.Parse(str);
    }
    public static void SetValueByString(this ref byte value, string str)
    {
        if (str == null || str == "") return;
        value = byte.Parse(str);
    }
    public static void SetValueByString(this ref long value, string str)
    {
        if (str == null || str == "") return;
        value = long.Parse(str);
    }
    public static void SetValueByString(this ref short value, string str)
    {
        if (str == null || str == "") return;
        value = short.Parse(str);
    }
    public static void SetValueByString(this ref float value, string str)
    {
        if (str == null || str == "") return;
        value = float.Parse(str);
    }
    public static void SetXByString(this ref Vector2 vector, string str)
    {
        if (str == null || str == "") return;
        float value = float.Parse(str);
        vector = new Vector2(value, vector.y);
    }
    public static void SetYByString(this ref Vector2 vector, string str)
    {
        if (str == null || str == "") return;
        float value = float.Parse(str);
        vector = new Vector2(vector.x, value);
    }
    public static void SetValueByString(this ref bool value, string str)
    {
        if (str == null) value = false;
        else if (str == "FALSO") value = false;
        else if (str == "VERDADEIRO") value = true;
        else if (str == "x" || str == "X") value = true;
        else if (str == "") value = false;
        else value = bool.Parse(str);
    }
    public static void SetValueByString(this ref DateTime value, string str)
    {
        if (str == null || str == "") return;
        value = DateTime.Parse(str, new CultureInfo("pt-BR"));
    }
    public static void SetValueByString<T>(this ref T value, string str) where T : struct
    {
        if (str == null || str == "") return;
        value = str.GetEnumByString<T>();
    }
    public static void SetValueByString<T>(this List<T> value, string str) where T : System.Enum
    {
        if (str == null || str == "") return;
        string[] strings = str.Split('|');
        foreach (var item in strings)
        {
            value.Add(item.GetEnumByString<T>());
        }
    }
    public static T GetEnumByString<T>(this string text)
    {
        T myEnum = (T)Enum.Parse(typeof(T), text.Replace(" ", "_"));
        return myEnum;
    }
    public static string GetStringByEnumList<T>(this List<T> enumList) where T : struct
    {
        char sep = '|';
        StringBuilder text = new StringBuilder();
        foreach (var item in enumList)
        {
            text.Append(item.ToString()).Append(sep);
        }
        return text.ToString().RemoveLastChar();
    }
    public static string RemoveLastChar(this string finalText)
    {
        if (string.IsNullOrEmpty(finalText)) return "";
        return finalText.Remove(finalText.Length - 1);
    }
    public static string LogArray(this int[] array, string separator)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var item in array)
        {
            sb.Append(item.ToString()).Append(separator);
        }
        return sb.ToString();
    }
    #endregion
    public static int GetRandomValue(int minInclusive, int maxExclusive)
    {
        return UnityEngine.Random.Range(minInclusive, maxExclusive);
        //return RandomizationProvider.Current.GetInt(minInclusive, maxExclusive);
    }

    /// <summary>
    /// min inclusive, max exclusive
    /// </summary>
    /// <param name="length"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static int[] GetUniqueIntsOrdered(int length, int min, int max, bool ascending)
    {
        int[] intsNormal = GetUniqueInts(length, min, max);
        return ascending ? intsNormal.OrderBy(i => i).ToArray() : intsNormal.OrderByDescending(i => i).ToArray();
    }
    /// <summary>
    /// min inclusive, max exclusive
    /// </summary>
    /// <param name="length"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static int[] GetUniqueInts(int length, int min, int max)
    {
        int diff = max - min;
        if (diff < length)
        {
            Debug.Log("Error in random function");
            return null;
        }
        var orderedValues = Enumerable.Range(min, diff).ToList();
        int[] ints = new int[length];
        for (int i = 0; i < length; i++)
        {
            var removeIndex = GetRandomValue(0, orderedValues.Count);
            ints[i] = orderedValues[removeIndex];
            orderedValues.RemoveAt(removeIndex);
        }
        return ints;
    }

    /*
    4,95 - 7,15
    public static int GetRandomIndex(List<int> usedIndex, int minInclusive, int maxExclusive)
    {
        if (usedIndex == null || usedIndex.Count == 0) return GetRandomValue(minInclusive, maxExclusive);
        int randomInt = GetRandomValue(0, 100);
        int diff = maxExclusive - minInclusive;
        int excludes = usedIndex.Count >= diff ? 0 : usedIndex.Count;
        int random = Mathf.FloorToInt(randomInt / 100f * (maxExclusive - minInclusive - excludes)) + minInclusive;
        int finalRandom = random;
        List<int> usedIndexSorted = usedIndex.OrderBy(x => x).ToList();
        for (int i = 0; i < excludes; i++)
        {
            if (random >= usedIndexSorted[i]) random++;
        }
        if (random >= maxExclusive || random < minInclusive) return finalRandom;
        return random;
    }*/

    /*
    4,92 - 7,02
    */
    public static int GetRandomIndex(Dictionary<int, int> usedIndex, int minInclusive, int maxExclusive)
    {
        if (usedIndex == null || usedIndex.Count == 0) return GetRandomValue(minInclusive, maxExclusive);
        int[] randomList = GetUniqueInts(maxExclusive - minInclusive, minInclusive, maxExclusive);
        for (int i = 0; i < randomList.Length; i++)
        {
            if (usedIndex.ContainsKey(randomList[i])) continue;
            return randomList[i];
        }
        return GetRandomValue(minInclusive, maxExclusive);
    }

    /// <returns>First -> index -- Second -> dict item</returns>
    public static (int, TValue) GetRandomDict<TKey, TValue>(IDictionary<TKey, TValue> dict, Dictionary<int, int> usedIndexs)
    {
        int rand = GetRandomIndex(usedIndexs, 0, dict.Count);
        List<TValue> values = Enumerable.ToList(dict.Values);
        return (rand, values[rand]);
    }
    public static List<T> RemoveIndexFromList<T>(List<T> list, List<int> removeIndex)
    {
        if (removeIndex.Count == 0) return list;
        for (int i = removeIndex.Count - 1; i >= 0; i--)
        {
            list.RemoveAt(removeIndex[i]);
        }
        return list;
    }
    public static string ToStringKey<TKey, TValue>(this Dictionary<TKey, TValue> dic)
    {
        StringBuilder newText = new StringBuilder();
        newText.Append("Dictionary, Length = ").AppendLine(dic.Count.ToString());
        foreach (var item in dic)
        {
            newText.Append(item.Key).Append(" | ");
        }
        return newText.ToString();
    }
    public static string ToStringValue<TKey, TValue>(this Dictionary<TKey, TValue> dic)
    {
        StringBuilder newText = new StringBuilder();
        newText.Append("Dictionary, Length = ").AppendLine(dic.Count.ToString());
        foreach (var item in dic)
        {
            newText.Append(item.Key).Append(" | ").AppendLine(item.Value.ToString());
        }
        return newText.ToString();
    }
    public static string DictionaryToStringKeyName<TKey, TValue>(Dictionary<TKey, TValue> dic) where TValue : BaseObject
    {
        StringBuilder newText = new StringBuilder();
        newText.Append("Dictionary, Length = ").AppendLine(dic.Count.ToString());
        foreach (var item in dic)
        {
            newText.Append(item.Key).Append(" | ").AppendLine(item.Value.displayName);
        }
        return newText.ToString();
    }

    #region Getters
    public static T GetEnumByString<T>(string text, T enumerator)
    {
        text = text.Split('(')[0].Trim();
        text = text.Replace(" ", "_").Replace(".", "");
        T myEnum = (T)Enum.Parse(typeof(T), text);
        return myEnum;
    }
    public static Type GetType(string type)
    {
        if (type == "string" || type == "System.String")
        {
            return typeof(string);
        }
        else if (type == "float" || type == "System.Float")
        {
            return typeof(float);
        }
        else if (type == "int" || type == "System.Int32")
        {
            return typeof(int);
        }
        return typeof(string);
    }
    public static string GetType(Type type)
    {
        if (type == typeof(string))
        {
            return "string";
        }
        else if (type == typeof(float))
        {
            return "float";
        }
        else if (type == typeof(int))
        {
            return "int";
        }
        return "string";
    }
    public static T GetNextSlot<T>(List<T> poolerList, GameObject prefab, Transform content, int index = -1) where T : MonoBehaviour
    {
        if (index == -1) return GetNextSlot(poolerList, prefab, content);
        if (poolerList.Count <= index)
        {
            GameObject newStoreSlot = MonoBehaviour.Instantiate(prefab, content);
            newStoreSlot.name = prefab.name + index.ToString();
            poolerList.Add(newStoreSlot.GetComponent<T>());
        }
        return poolerList[index];
    }
    private static T GetNextSlot<T>(List<T> poolerList, GameObject prefab, Transform content) where T : MonoBehaviour
    {
        for (int i = 0; i < poolerList.Count; i++)
        {
            if (!poolerList[i].gameObject.activeInHierarchy)
                return poolerList[i];
        }
        GameObject newStoreSlot = MonoBehaviour.Instantiate(prefab, content);
        newStoreSlot.name = prefab.name + poolerList.Count.ToString();
        poolerList.Add(newStoreSlot.GetComponent<T>());
        return poolerList[poolerList.Count - 1];
    }
    #endregion

    #region Time
    public static bool SetBoolFromX(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;
        else return true;
    }
    public static int StringToInt(string value, bool nullMax)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            if (nullMax) value = "9999999";
            else value = "0";
        }
        return Int32.Parse(value);
    }

    /// <summary>
    /// se o string for null, vai pegar uma data máxima ou mínima
    /// </summary>
    public static DateTime StringToDateTime(string dateString, bool nullMax)
    {
        if (string.IsNullOrWhiteSpace(dateString))
        {
            if (nullMax) dateString = "25/12/2099";
            else dateString = "25/12/2009";
        }
        return Convert.ToDateTime(dateString);
    }
    public static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    public static long ConvertToTimestamp(DateTime value)
    {
        TimeSpan elapsedTime = value - Epoch;
        return (long)elapsedTime.TotalSeconds;
    }
    public static string DateTimeToHHMM(DateTime date)
    {
        int hour = date.Hour;
        int min = date.Minute;
        return hour.ToString("D2") + ":" + min.ToString("D2");
    }
    public static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddSeconds(unixTimeStamp);
        return dtDateTime;
    }
    public static string SecToMin(double secs)
    {
        float sec = (float)secs;
        int xs, ys;
        string x, y;
        xs = Mathf.FloorToInt(sec / 60f);
        if (sec > 60)
        {
            ys = Mathf.FloorToInt(sec - xs * 60);
        }
        else
        {
            ys = Mathf.FloorToInt(sec);
        }
        x = xs.ToString("D2");
        y = ys.ToString("D2");
        return x + ":" + y;
    }
    public static string SecToHourMin(double secs)
    {
        float sec = (float)secs;
        int hs, ms, ss;
        string h, m, s;
        ms = Mathf.FloorToInt(sec / 60f);
        if (sec > 60)
        {
            ss = Mathf.FloorToInt(sec - ms * 60);
        }
        else
        {
            ss = Mathf.FloorToInt(sec);
        }
        hs = Mathf.FloorToInt(ms / 60f);
        if (ms > 60)
        {
            ms = Mathf.FloorToInt(ms - hs * 60);
        }
        else
        {
            ms = Mathf.FloorToInt(ms);
        }
        h = hs.ToString("D2");
        m = ms.ToString("D2");
        s = ss.ToString("D2");
        return h + ":" + m + ":" + s;
    }
    public static string SecToHourMinAbrev(double secs)
    {
        float sec = (float)secs;
        int hs, ms, ss;
        string h, m, s;
        ms = Mathf.FloorToInt(sec / 60f);
        if (sec > 60)
        {
            ss = Mathf.FloorToInt(sec - ms * 60);
        }
        else
        {
            ss = Mathf.FloorToInt(sec);
        }
        hs = Mathf.FloorToInt(ms / 60f);
        if (ms > 60)
        {
            ms = Mathf.FloorToInt(ms - hs * 60);
        }
        else
        {
            ms = Mathf.FloorToInt(ms);
        }
        h = hs.ToString("D2");
        m = ms.ToString("D2");
        s = ss.ToString("D2");
        return h + "h" + m + "m";
    }
    // pretty print seconds as hours:minutes:seconds(.milliseconds/100)s
    public static string PrettySeconds(float seconds)
    {
        TimeSpan t = System.TimeSpan.FromSeconds(seconds);
        string res = "";
        if (t.Days > 0)
            res += t.Days + "d";
        if (t.Hours > 0)
            res += " " + t.Hours + "h";
        if (t.Minutes > 0)
            res += " " + t.Minutes + "m";
        // 0.5s, 1.5s etc. if any milliseconds. 1s, 2s etc. if any seconds
        if (t.Milliseconds > 0)
            res += " " + t.Seconds + "." + (t.Milliseconds / 100) + "s";
        else if (t.Seconds > 0)
            res += " " + t.Seconds + "s";
        // if the string is still empty because the value was '0', then at least
        // return the seconds instead of returning an empty string
        return res != "" ? res : "0s";
    }
    #endregion

    #region Order
    public static void AddToItemList<T, K>(ref List<T> list, T item, Func<T, K> comparer, bool ascending)
    {
        if (ascending)
            AddToItemListAscending(ref list, item, comparer);
        else
            AddToItemListDescending(ref list, item, comparer);
    }
    private static void AddToItemListAscending<T, K>(ref List<T> list, T item, Func<T, K> comparer)
    {
        double value = double.Parse(comparer(item).ToString());
        int listLength = list.Count;
        if (listLength > 0)
        {
            double firstValue = double.Parse(comparer(list[0]).ToString());
            if (listLength > 1)
            {
                double lastValue = double.Parse(comparer(list[listLength - 1]).ToString());
                int index2 = SearchIndexAscending(value, firstValue, lastValue, listLength, 0, listLength - 1, list, comparer);
                list.Insert(index2, item);
            }
            else
            {
                if (value >= firstValue)
                {
                    list.Add(item);
                }
                else
                {
                    list.Insert(0, item);
                }
            }
        }
        else
        {
            list.Add(item);
        }
    }
    /// <summary>
    /// começa com firstIndex = 0 e lastIndex = ultimo, depois pega o timedomeio e decide se vai olhar do meio para baixo ou do meio para cima
    /// </summary>
    private static int SearchIndexAscending<T, K>(double value, double firstValue, double lastValue, int listLength, int firstIndex, int lastIndex, List<T> list, Func<T, K> comparer)
    {
        if (value >= lastValue)
        {
            return firstIndex + listLength;
        }
        else if (value <= firstValue)
        {
            return 0;
        }
        else
        {
            int midIndex = (firstIndex + lastIndex) / 2;
            double mid = double.Parse(comparer(list[midIndex]).ToString());
            if (value > mid)
            {
                if (listLength > 2)
                {
                    return SearchIndexAscending(value, mid, lastValue, listLength - midIndex + firstIndex, midIndex, lastIndex, list, comparer);
                }
                else
                {
                    return midIndex + 1;
                }
            }
            else if (value < mid)
            {
                if (listLength > 2)
                {
                    return SearchIndexAscending(value, firstValue, mid, midIndex - firstIndex + 1, firstIndex, midIndex, list, comparer);
                }
                else
                {
                    return midIndex + 1;
                }
            }
            else
            {
                return midIndex;
            }
        }
    }
    private static void AddToItemListDescending<T, K>(ref List<T> list, T item, Func<T, K> comparer)
    {
        double value = double.Parse(comparer(item).ToString());
        int listLength = list.Count;
        if (listLength > 0)
        {
            double firstValue = double.Parse(comparer(list[0]).ToString());
            if (listLength > 1)
            {
                double lastValue = double.Parse(comparer(list[listLength - 1]).ToString());
                int index = SearchIndexDescending(value, firstValue, lastValue, listLength, 0, listLength - 1, list, comparer);
                list.Insert(index, item);
            }
            else
            {
                if (value >= firstValue)
                {
                    list.Insert(0, item);
                }
                else
                {
                    list.Add(item);
                }
            }
        }
        else
        {
            list.Add(item);
        }
    }
    private static int SearchIndexDescending<T, K>(double value, double firstValue, double lastValue, int listLength, int firstIndex, int lastIndex, List<T> list, Func<T, K> comparer)
    {
        if (value >= firstValue)
        {
            return 0;
        }
        else if (value <= lastValue)
        {
            return firstIndex + listLength;
        }
        else
        {
            int midIndex = (firstIndex + lastIndex) / 2;
            double mid = double.Parse(comparer(list[midIndex]).ToString());
            if (value < mid)
            {
                if (listLength > 2)
                {
                    return SearchIndexDescending(value, mid, lastValue, listLength - midIndex + firstIndex, midIndex, lastIndex, list, comparer);
                }
                else
                {
                    return midIndex + 1;
                }
            }
            else if (value > mid)
            {
                if (listLength > 2)
                {
                    return SearchIndexDescending(value, firstValue, mid, midIndex - firstIndex + 1, firstIndex, midIndex, list, comparer);
                }
                else
                {
                    return midIndex + 1;
                }
            }
            else
            {
                return midIndex;
            }
        }
    }
    #endregion

    #region TextEditor
    public static string UnidadePonto(long value)
    {
        var nfi = new NumberFormatInfo { NumberDecimalSeparator = ",", NumberGroupSeparator = "." };
        string res = value.ToString("#,##0", nfi); //result will always be 1.234.567,00
        return res;
    }
    public static string UnidadePonto(int value)
    {
        var nfi = new NumberFormatInfo { NumberDecimalSeparator = ",", NumberGroupSeparator = "." };
        string res = value.ToString("#,##0", nfi); //result will always be 1.234.567,00
        return res;
    }
    public static string Unidade(long value, bool casaDecimal = true)
    {
        if (casaDecimal)
        {
            if (value == 0)
            {
                return "0";
            }
            else if (value > 0 & value <= 99999)
            {
                return string.Format("{0:#}", value);
            }
            else if (value > 99999 & value <= 999999)
            {
                return string.Format("{0:#,0}.{1:#,00}k", (int)(value / 1000), (int)((value % 1000) / 10));
            }
            else if (value > 999999 & value <= 999999999)
            {
                return string.Format("{0:#,0}.{1:#,00}M", (int)(value / 1000000), (int)((value % 1000000) / 10000));
            }
            else if (value > 999999999 & value <= 999999999999)
            {
                return string.Format("{0:#,0}.{1:#,00}B", (int)(value / 1000000000), (int)((value % 1000000000) / 10000000));
            }
            else if (value > 999999999999 & value <= 999999999999999)
            {
                return string.Format("{0:#,0}.{1:#,00}T", (int)(value / 1000000000000), (int)((value % 1000000000000) / 10000000000));
            }
            else if (value > 999999999999999 & value <= 999999999999999999)
            {
                return string.Format("{0:#,0}.{1:#,00}Qa", (int)(value / 1000000000000000), (int)((value % 1000000000000000) / 10000000000000));
            }
            else if (value > 999999999999999999 & value / 1000000000000000000 <= 999)
            {
                return string.Format("{0:#,0}.{1:#,00}Qi", (int)(value / 1000000000000000000), (int)((value % 1000000000000000000) / 10000000000000000));
            }
            else if (value / 1000000000000000000 > 999 & value / 1000000000000000000 <= 999999)
            {
                float value2 = value / 1000000000000000000;
                return string.Format("{0:#,0}.{1:#,00}Sx", (int)(value2 / 1000), (int)((value2 % 1000) / 10));
            }
            else if (value / 1000000000000000000 > 999999 & value / 1000000000000000000 <= 999999999)
            {
                float value2 = value / 1000000000000000000;
                return string.Format("{0:#,0}.{1:#,00}Sp", (int)(value2 / 1000000), (int)((value2 % 1000000) / 10000));
            }
            else if (value / 1000000000000000000 > 999999999 & value / 1000000000000000000 <= 999999999999)
            {
                float value2 = value / 1000000000000000000;
                return string.Format("{0:#,0}.{1:#,00}Oc", (int)(value2 / 1000000000), (int)((value2 % 1000000000) / 10000000));
            }
            else if (value / 1000000000000000000 > 999999999999 & value / 1000000000000000000 <= 999999999999999)
            {
                float value2 = value / 1000000000000000000;
                return string.Format("{0:#,0}.{1:#,00}N", (int)(value2 / 1000000000000), (int)((value2 % 1000000000000) / 10000000000));
            }
            else if (value / 1000000000000000000 > 999999999999999)
            {
                float value2 = value / 1000000000000000000;
                return string.Format("{0:#,0}.{1:#,00}D", (int)(value2 / 1000000000000000), (int)((value2 % 1000000000000000) / 10000000000000));
            }
            else
            {
                return string.Format("{0:#,0.0}", value);
            }
        }
        else
        {
            if (value == 0)
            {
                return "0";
            }
            else if (value > 0 & value <= 99999)
            {
                return string.Format("{0:#}", value);
            }
            else if (value > 99999 & value <= 999999)
            {
                return string.Format("{0:#,0}k", (int)(value / 1000));
            }
            else if (value > 999999 & value <= 999999999)
            {
                return string.Format("{0:#,0}M", (int)(value / 1000000));
            }
            else if (value > 999999999 & value <= 999999999999)
            {
                return string.Format("{0:#,0}B", (int)(value / 1000000000));
            }
            else if (value > 999999999999 & value <= 999999999999999)
            {
                return string.Format("{0:#,0}T", (int)(value / 1000000000000));
            }
            else if (value > 999999999999999 & value <= 999999999999999999)
            {
                return string.Format("{0:#,0}Qa", (int)(value / 1000000000000000));
            }
            else if (value > 999999999999999999 & value / 1000000000000000000 <= 999)
            {
                return string.Format("{0:#,0}Qi", (int)(value / 1000000000000000000));
            }
            else if (value / 1000000000000000000 > 999 & value / 1000000000000000000 <= 999999)
            {
                float value2 = value / 1000000000000000000;
                return string.Format("{0:#,0}Sx", (int)(value2 / 1000));
            }
            else if (value / 1000000000000000000 > 999999 & value / 1000000000000000000 <= 999999999)
            {
                float value2 = value / 1000000000000000000;
                return string.Format("{0:#,0}Sp", (int)(value2 / 1000000));
            }
            else if (value / 1000000000000000000 > 999999999 & value / 1000000000000000000 <= 999999999999)
            {
                float value2 = value / 1000000000000000000;
                return string.Format("{0:#,0}Oc", (int)(value2 / 1000000000));
            }
            else if (value / 1000000000000000000 > 999999999999 & value / 1000000000000000000 <= 999999999999999)
            {
                float value2 = value / 1000000000000000000;
                return string.Format("{0:#,0}N", (int)(value2 / 1000000000000));
            }
            else if (value / 1000000000000000000 > 999999999999999)
            {
                float value2 = value / 1000000000000000000;
                return string.Format("{0:#,0}D", (int)(value2 / 1000000000000000));
            }
            else
            {
                return string.Format("{0:#,0}", value);
            }
        }
    }
    public static string UnidadeOnlyK(long value, bool casaDecimal = true)
    {
        if (casaDecimal)
        {
            if (value == 0)
            {
                return "0";
            }
            else if (value > 0 & value <= 99999)
            {
                return string.Format("{0:#}", value);
            }
            else if (value > 99999)
            {
                return string.Format("{0:#,0}.{1:#,00} k", (int)(value / 1000), (int)((value % 1000) / 10));
            }
            else
            {
                return string.Format("{0:#,0.0}", value);
            }
        }
        else
        {
            if (value == 0)
            {
                return "0";
            }
            else if (value > 0 & value <= 99999)
            {
                return string.Format("{0:#,0}", value);
            }
            else if (value > 99999)
            {
                //se quiser separação por ponto "{0:#,0}k"
                return string.Format("{0:#,0} k", (int)(value / 1000));
            }
            else
            {
                return string.Format("{0:#,0}", value);
            }
        }
    }
    #endregion

    #region IO
    public static string GetFolderPathFromFile(string filePath)
    {
        StringBuilder folder = new StringBuilder();
        string[] folderParts = filePath.Split('/');
        for (int i = 0; i < folderParts.Length - 1; i++)
        {
            folder.Append(folderParts[i]).Append("/");
        }
        return folder.ToString();
    }
    public static string GetFolderFromFile(string filePath)
    {
        StringBuilder folder = new StringBuilder();
        string[] folderParts = filePath.Split('/');
        if (folderParts.Length > 1)
        {
            return folderParts[folderParts.Length - 2];
        }
        return "";
    }
    public static string GetFileNameFromPath(string filePath)
    {
        StringBuilder folder = new StringBuilder();
        string[] folderParts = filePath.Split('/');
        return folderParts[folderParts.Length - 1].ToString();
    }
    /// <summary>
    /// Entra com o Full Path c:/Arquivos/Etc/Projeto/Assets/Folder/file.xxx e sai com o project path //Assets/Folder.etc
    /// </summary>
    /// <returns></returns>
    public static string GetProjectPath(string fullPath)
    {
        string[] s = fullPath.Split('/');
        StringBuilder newPath = new StringBuilder();
        bool started = false;
        foreach (var name in s)
        {
            if (name == "Assets") started = true;
            if (name != "Assets" && started == true)
            {
                newPath.Append("\\");
            }
            if (started)
            {
                newPath.Append(name);
            }
        }
        return newPath.ToString();
    }

    /// <summary>
    /// Entrar com o asset folder, //Assets/Folder.etc
    /// </summary>
    /// <param name="assetFolderPath"></param>
    public static bool CheckFolderExistsByAssetPath(string assetFolderPath, bool createNew)
    {
        string fullPath = $"{Application.dataPath.Replace("/Assets", "/")}/{assetFolderPath}";
        bool exists = Directory.Exists(fullPath);
        if (!exists && createNew)
        {
            Directory.CreateDirectory(fullPath);
            return true;
        }
        else
        {
            return exists;
        }
    }
    /// <summary>
    /// Entrar com o asset folder, //Assets/Folder/file.xxx //NÃO USAR PARA CRIAR NOVO EXCEL!!! USAR O EXCELUTILS!!
    /// </summary>
    /// <param name="assetFolderPath"></param>
    public static bool CheckFileExistsByAssetPath(string assetFilePath, bool createNew)
    {
        string fullPath = $"{Application.dataPath.Replace("/Assets", "/")}/{assetFilePath}";
        string folderPath = Utils.GetFolderPathFromFile(assetFilePath);
        CheckFolderExistsByAssetPath(folderPath, true);
        bool exists = File.Exists(fullPath);
        if (!exists && createNew)
        {
            File.Create(fullPath);
            return true;
        }
        else
        {
            return exists;
        }
    }
    /// <summary>
    /// Entrar com o asset folder, c:/Arquivos/Etc/Projeto//Assets/Folder.etc
    /// </summary>
    /// <param name="folderPath"></param>
    public static bool CheckFolderExistsByFullPath(string folderPath, bool createNew)
    {
        string fullPath = folderPath;
        bool exists = Directory.Exists(fullPath);
        if (!exists && createNew)
        {
            Directory.CreateDirectory(fullPath);
            return true;
        }
        else
        {
            return exists;
        }
    }
    /// <summary>
    /// Entrar com o asset folder, Full Path c:/Arquivos/Etc/Projeto/Assets/Folder/file.xxx //NÃO USAR PARA CRIAR NOVO EXCEL!!! USAR O EXCELUTILS!!
    /// </summary>
    /// <param name="assetFolderPath"></param>
    public static bool CheckFileExistsByFullPath(string filePath, bool createNew)
    {
        string fullPath = filePath;
        string folderPath = Utils.GetFolderPathFromFile(filePath);
        CheckFolderExistsByAssetPath(folderPath, true);
        bool exists = File.Exists(fullPath);
        if (!exists && createNew)
        {
            File.Create(fullPath);
            return true;
        }
        else
        {
            return exists;
        }
    }
    #endregion

}

