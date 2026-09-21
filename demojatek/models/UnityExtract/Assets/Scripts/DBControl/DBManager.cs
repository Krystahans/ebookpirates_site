using System;
using System.IO;
using UnityEngine;

public class DBManager : MonoBehaviour
{
    public static DBManager Instance { get; private set; }
    public string PlayerName;
    public int Stored;
    public string isTutorialOK = "";
    
    [Header("Triggers")]
    public bool hasAscended;
    public bool hasMoved;
    public bool firstReset;
    public bool firstWind;
    public bool firstChase;
    public bool secoundChase;
    public bool firstCatch;
    public int lastMonolog;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        ReadFromDBFile();
    }

    private string GetDBPath()
    {
        string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string dir = Path.Combine(localAppData, "eBookPirates");
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        return Path.Combine(dir, "DBTemp.txt");
    }

    public bool ReadFromDBFile()
    {
        try
        {
            string path = GetDBPath();
            if (!File.Exists(path))
            {
                return false;
            }

            string[] lines = File.ReadAllLines(path);
            foreach (string line in lines)
            {
                string trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed)) continue;

                if (trimmed.StartsWith("Stored:"))
                {
                    string val = trimmed.Substring("Stored:".Length).Trim();
                    int.TryParse(val, out Stored);
                }
                else if (trimmed.StartsWith("isTutorialOK:"))
                {
                    isTutorialOK = trimmed.Substring("isTutorialOK:".Length).Trim();
                }
                else if (trimmed == "Triggers:") { /* section header */ }
                else if (trimmed.StartsWith("hasAscended:")) bool.TryParse(trimmed.Substring("hasAscended:".Length).Trim(), out hasAscended);
                else if (trimmed.StartsWith("hasMoved:")) bool.TryParse(trimmed.Substring("hasMoved:".Length).Trim(), out hasMoved);
                else if (trimmed.StartsWith("firstReset:")) bool.TryParse(trimmed.Substring("firstReset:".Length).Trim(), out firstReset);
                else if (trimmed.StartsWith("firstWind:")) bool.TryParse(trimmed.Substring("firstWind:".Length).Trim(), out firstWind);
                else if (trimmed.StartsWith("firstChase:")) bool.TryParse(trimmed.Substring("firstChase:".Length).Trim(), out firstChase);
                else if (trimmed.StartsWith("secoundChase:")) bool.TryParse(trimmed.Substring("secoundChase:".Length).Trim(), out secoundChase);
                else if (trimmed.StartsWith("firstCatch:")) bool.TryParse(trimmed.Substring("firstCatch:".Length).Trim(), out firstCatch);
                else if (trimmed.StartsWith("lastMonolog:")) int.TryParse(trimmed.Substring("lastMonolog:".Length).Trim(), out lastMonolog);
            }
            Debug.Log($"Loaded DB from {path}. Stored: {Stored}, isTutorialOK: {isTutorialOK}, Triggers: {hasAscended}, {hasMoved}, {firstReset}, {firstWind}, {firstChase}, {secoundChase}, {firstCatch}, {lastMonolog}");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error reading DB file: {e.Message}");
            return false;
        }
    }

    public bool SaveToDBFile()
    {
        try
        {
            string path = GetDBPath();
            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.WriteLine($"Stored: {Stored}");
                writer.WriteLine($"isTutorialOK: {isTutorialOK}");
                writer.WriteLine("Triggers:");
                writer.WriteLine($"hasAscended: {hasAscended}");
                writer.WriteLine($"hasMoved: {hasMoved}");
                writer.WriteLine($"firstReset: {firstReset}");
                writer.WriteLine($"firstWind: {firstWind}");
                writer.WriteLine($"firstChase: {firstChase}");
                writer.WriteLine($"secoundChase: {secoundChase}");
                writer.WriteLine($"firstCatch: {firstCatch}");
                writer.WriteLine($"lastMonolog: {lastMonolog}");
            }
            Debug.Log($"Saved DB to {path}. Stored: {Stored}, isTutorialOK: {isTutorialOK}, Triggers: {hasAscended}, {hasMoved}, {firstReset}, {firstWind}, {firstChase}, {secoundChase}, {firstCatch}, {lastMonolog}");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving DB file: {e.Message}");
            return false;
        }
    }
}
