using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.IO;
using UnityEngine.Networking;

public class DatabaseHandler
{
    void aAwake()
    {
        List<QuestionData> questionsList = new List<QuestionData>();

        // Create database
        string connection = "URI=file:" + Application.dataPath + "/DataBase/" + "AllIndiaQuizDataBase.db";

        // Open connection
        IDbConnection dbcon = new SqliteConnection(connection);
        dbcon.Open();

        // Create table
        //IDbCommand dbcmd;
        //dbcmd = dbcon.CreateCommand();
        //string q_createTable = "CREATE TABLE IF NOT EXISTS my_table (id INTEGER PRIMARY KEY, val INTEGER )";

        //dbcmd.CommandText = q_createTable;
        //dbcmd.ExecuteReader();

        // Insert values in table
        //IDbCommand cmnd = dbcon.CreateCommand();
        //cmnd.CommandText = "INSERT INTO my_table (id, val) VALUES (0, 5)";
        //cmnd.ExecuteNonQuery();

        // Read and print all values in table
        IDbCommand cmnd_read = dbcon.CreateCommand();
        IDataReader reader;
        string query = "SELECT * FROM question_set";
        cmnd_read.CommandText = query;
        reader = cmnd_read.ExecuteReader();


        while (reader.Read())
        {
            QuestionData questionData = new QuestionData();
            questionData.question = reader[2].ToString();
            questionData.option1 = reader[3].ToString();
            questionData.option2 = reader[4].ToString();
            questionData.option3 = reader[5].ToString();
            questionData.option4 = reader[6].ToString();

            questionData.answer = Convert.ToInt32(reader[7].ToString());

            questionsList.Add(questionData);
        }

        // Close connection
        dbcon.Close();

    }

    public List<QuestionData> GetQuestionList(int set_id)
    {
        List<QuestionData> questionsList = new List<QuestionData>();

        //string connection = "URI=file:" + Application.dataPath + "/DataBase/" + "AllIndiaQuizDataBase.db";
        //   string connection = Application.persistentDataPath + "/DataBase/" + "AllIndiaQuizDataBase.db";
        string connection = "";

#if UNITY_EDITOR
        Debug.Log("Unity Editor");
        connection = "URI=file:" + Application.dataPath + "/DataBase/" + "AllIndiaQuizDataBase.db";

#elif UNITY_ANDROID 
        Debug.Log("Unity Android");

        WWW loadDB = new WWW("jar:file://" + Application.dataPath + "!/assets/" + "AllIndiaQuizDataBase.db");  // this is the path to your StreamingAssets in android
        while (!loadDB.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check

        string realPath = Application.persistentDataPath + "/AllIndiaQuizDataBase.db";

        File.WriteAllBytes(realPath, loadDB.bytes);
        connection = "URI=file:" + realPath;

#else
    Debug.Log("Any other platform");

#endif

        // Open connection
        IDbConnection dbcon = new SqliteConnection(connection);
        dbcon.Open();

        // Read and print all values in table
        IDbCommand cmnd_read = dbcon.CreateCommand();
        IDataReader reader;
        string query = "SELECT * FROM question_set";
        cmnd_read.CommandText = query;
        reader = cmnd_read.ExecuteReader();

        while (reader.Read())
        {
            if (reader[1].ToString() == set_id.ToString())
            {
                QuestionData questionData = new QuestionData();
                questionData.question = reader[2].ToString();
                questionData.option1 = reader[3].ToString();
                questionData.option2 = reader[4].ToString();
                questionData.option3 = reader[5].ToString();
                questionData.option4 = reader[6].ToString();

                questionData.answer = Convert.ToInt32(reader[7].ToString());

                questionsList.Add(questionData);
            }
        }

        // Close connection
        dbcon.Close();

        return questionsList;
    }
}
