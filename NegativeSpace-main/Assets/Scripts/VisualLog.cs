using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;

public class VisualLog : MonoBehaviour {

    public int LineInPixels = 25;

    private List<string> _lines;
    public bool Show;

    private string _logfilename;

    void Awake()
    {
        _logfilename = Application.dataPath + "/log.txt";
        Show = false;
        _lines = new List<string>();
        File.Create(_logfilename).Close();
        WriteLineToFile(DateTime.Now.ToString("yyyy/MM/dd - HH:mm:ss"));
    }

    public void WriteLine(object sender, string line)
    {
        line = "[" + sender.ToString() + "] " + line;
        _lines.Add(line);
        WriteLineToFile(line);
        int possibleLines = (Screen.height / LineInPixels);
        if (_lines.Count >= possibleLines)
        {
            possibleLines = _lines.Count - possibleLines;
            while (possibleLines > 0)
            {
                _lines.RemoveAt(0);
                possibleLines -= 1;
            }
        }
    }

    private void WriteLineToFile(string line)
    {
        File.AppendAllText(_logfilename, line + Environment.NewLine);
    }

    void OnGUI()
    {
        if (Show)
        {
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");
            int top = 10;
            int left = 10;
            foreach (string line in _lines)
            {
                GUI.Label(new Rect(left, top, Screen.width, LineInPixels), line);
                top += LineInPixels;
            }
        }
    }
}
