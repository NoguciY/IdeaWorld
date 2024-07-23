using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;        //ファイル操作に「File」クラスを使用してファイルを読み込んだり、書き込んだり、データを列挙するため
using UnityEditor;

public class StageGenerator : MonoBehaviour
{
    const float defaultStageObjWidth = 1.8f;
    const float defaultStageObjHeight = 1.0f;
    const float intiPosX = -10.7f;  //初期位置(x座標)
    const float intiPosY = -5.0f;   //初期位置(y座標)

    //CSVの中身を入れるリスト(二次元配列)
    List<string[]> csvDatas = new List<string[]>();
    GameObject stageObj;

    //csvデータの読み込み
    public void ReadStageData()
    {
        //Resourcesフォルダ内のcsv読み込み
        //TextAsset型の変数にResourcesn内の読み込むCSVファイルを格納する
        //Unityでスクリプト内で直接が読み込みが可能になる
        TextAsset csvFile = Resources.Load("csv/IdeaWorld_Stage1") as TextAsset;
        
        //TextAsset型のcsvFile変数をStringReaderに変換
        StringReader reader = new StringReader(csvFile.text);
        
        string ObjectAddress = "StageObject/";

        int maxX = -1;      //csvファイルの列数、初期値は一応-1
        int maxY = -1;      //csvファイルの行数、初期値は、配列が0から始まるため-1
        string line = "";   //csvファイル１行分を格納する変数

        //「,」で分割しつつ一行ずつ読み込みリストに追加していく
        //reader.Peaekが-1になるまで
        //csvファイルの内容を１行ずつ読み込む
        //reader.Peek():次の文字が存在するかどうかを判断する
        while (reader.Peek() != -1)
        {
            //読み込んだcsvファイルを１行ずつ読み込みline変数に格納
            line = reader.ReadLine();

            //string.Split(''):文字列を()の文字で分割する
            //csvDatasを「,」ごとに区切り、区切ったデータ(0や1)をリストに追加
            csvDatas.Add(line.Split(','));

            //行数をカウント
            maxY++;
        }
        //csvファイル１行ごとの列数を格納
        maxX = CountChar(line, ',');

        //ここまででcsvファイルの行数と列数を取得している

        //列数分繰り返す
        for (int x = 0; x <= maxX; ++x)
        {
            //行数分繰り返す
            for (int y = 0; y <= maxY; ++y)
            {
                //csvのデータが0でない場合
                if (csvDatas[y][x] != "0")
                {
                    //ステージのオブジェクトを配置する
                    //オブジェクトの位置(現在の列数,全行数 - 現在の行数)
                    //左下からオブジェクトと配置する(地面の位置を調整しやすいから)
                    CreateStageObject(maxY - y, x, ObjectAddress + csvDatas[y][x]);
                }
            }
        }
    }

    //プレハブを作成する
    //第一引数：生成するy座標
    //第二引数：生成するx座標
    //第三引数：生成するオブジェクトの名前
    private void CreateStageObject(int y, int x, string objName)
    {
        //生成するステージのオブジェクトを取得
        GameObject stageObjPrefab = (GameObject)Resources.Load(objName);

        //オブジェクトの縦横スケール
        float objSizeX = stageObjPrefab.transform.localScale.x;
        float objSizeY = stageObjPrefab.transform.localScale.y;

        //オブジェクトを生成する位置 = ステージ1マスの左上
        Vector2 stageObjPos = new Vector2(x * defaultStageObjWidth + intiPosX,
                                           y * defaultStageObjHeight + intiPosY);

        //オブジェクトを生成する
        stageObj = Instantiate(stageObjPrefab, stageObjPos, Quaternion.identity);

        //ステージ1マスの左上を基準にしてオブジェクトの位置を整える
        stageObj.transform.position += new Vector3(defaultStageObjWidth / 2 * objSizeX, -(defaultStageObjHeight / 2.0f * objSizeY), 0);

        //オブジェクトの中に入れる
        stageObj.transform.parent = transform;
    }

    //列数をカウントする
    public static int CountChar(string s, char c)
    {
        //string.Replace("置き換えたい文字列","置き換える文字列") : 文字列の一部を別の文字列に置き換える
        //string.ToString : int型やflaot型をstring型に変換する
        //                ★char型もstring型に変換できる★
        //csvファイルの１行分の文字数を
        //csvファイルの１行分の「,」を無と入れ替えた文字数で引いた文字数を返す
        //「,」の数が返される(列数(配列)は0からカウントする)
        return s.Length - s.Replace(c.ToString(), "").Length;
    }

    void Awake()
    {
        //ステージのデータを読み込み、ステージを作成する
        ReadStageData();
    }
}
