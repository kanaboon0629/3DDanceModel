using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SymmetryJsonProcessor
{
    public static void ProcessJson(string inputFilePath, string outputFilePath)
    {
        // 元のJSONファイルの読み込み
        if (!File.Exists(inputFilePath))
        {
            Debug.LogError("Input file not found: " + inputFilePath);
            return;
        }

        string jsonData = File.ReadAllText(inputFilePath);
        Debug.Log("Read JSON data: " + jsonData); // JSONデータの内容をログ出力

        try
        {
            var data = MiniJSON.Json.Deserialize(jsonData) as Dictionary<string, object>;
            if (data == null)
            {
                Debug.LogError("Failed to parse input JSON data");
                return;
            }

            // デシリアライズ結果を確認
            Debug.Log("Deserialized data: " + MiniJSON.Json.Serialize(data));

            // 型変換を試みる
            var convertedData = new Dictionary<string, Dictionary<string, List<float>>>();
            foreach (var key in data.Keys)
            {
                var innerDict = data[key] as Dictionary<string, object>;
                if (innerDict == null)
                {
                    Debug.LogError("Inner dictionary is null for key: " + key);
                    return;
                }

                var newInnerDict = new Dictionary<string, List<float>>();
                foreach (var innerKey in innerDict.Keys)
                {
                    var floatList = innerDict[innerKey] as List<object>;
                    if (floatList == null)
                    {
                        Debug.LogError("Float list is null for key: " + innerKey);
                        return;
                    }

                    newInnerDict[innerKey] = floatList.ConvertAll(item => Convert.ToSingle(item));
                }

                convertedData[key] = newInnerDict;
            }

            // すべての要素に対してz軸の要素をy軸対称にする
            foreach (var key in convertedData.Keys)
            {
                foreach (var joint in convertedData[key].Keys)
                {
                    convertedData[key][joint][2] = -convertedData[key][joint][2];
                }
            }

            // "left_〇〇"と"right_〇〇"について各要素の値を入れ替える
            foreach (var key in convertedData.Keys)
            {
                SwapValues(convertedData[key], "left_upperleg", "right_upperleg");
                SwapValues(convertedData[key], "left_lowerleg", "right_lowerleg");
                SwapValues(convertedData[key], "left_foot", "right_foot");
                SwapValues(convertedData[key], "left_upperarm", "right_upperarm");
                SwapValues(convertedData[key], "left_lowerarm", "right_lowerarm");
                SwapValues(convertedData[key], "left_hand", "right_hand");
            }

            // 新しいJSONファイルに書き込む
            string newJsonData = MiniJSON.Json.Serialize(convertedData);
            File.WriteAllText(outputFilePath, newJsonData);

            Debug.Log($"新しいJSONファイルが作成されました: {outputFilePath}");
        }
        catch (Exception ex)
        {
            Debug.LogError("Exception during JSON processing: " + ex.Message);
        }
    }

    private static void SwapValues(Dictionary<string, List<float>> data, string key1, string key2)
    {
        var temp = data[key1];
        data[key1] = data[key2];
        data[key2] = temp;
    }
}
