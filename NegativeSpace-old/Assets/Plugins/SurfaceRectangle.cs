using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SurfaceRectangle
{
    private Vector3 _bl;
    public Vector3 SurfaceBottomLeft { get { return _bl; } }
    private Vector3 _br;
    public Vector3 SurfaceBottomRight { get { return _br; } }
    private Vector3 _tl;
    public Vector3 SurfaceTopLeft { get { return _tl; } }
    private Vector3 _tr;
    public Vector3 SurfaceTopRight { get { return _tr; } }

    private string _folder;

    public SurfaceRectangle()
    {
        _folder = Application.dataPath + "/Surface/";
        _bl = Vector3.zero;
        _br = Vector3.zero;
        _tl = Vector3.zero;
        _tr = Vector3.zero;
    }

    public SurfaceRectangle(Vector3 BL, Vector3 BR, Vector3 TL, Vector3 TR)
    {
        _folder = null;
        _bl = BL;
        _br = BR;
        _tl = TL;
        _tr = TR;
    }

    public bool load()
    {

        if (Directory.Exists(_folder))
        {
            string[] files = Directory.GetFiles(_folder);
            foreach (string file in files)
            {
                if (file == _folder + "rectangle.txt")
                {
                    foreach (string line in File.ReadAllLines(file))
                    {
                        Debug.Log(line);

                        if (line.Length != 0 || line[0] != '%')
                        {
                            string[] st = line.Split('=');
                            if (st.Length == 2)
                            {
                                if (st[0] == "tracker.surface.bottom.left") { if (!_parseVector3(st[1], out _bl)) { return false; } }
                                else if (st[0] == "tracker.surface.bottom.right") { if (!_parseVector3(st[1], out _br)) { return false; } }
                                else if (st[0] == "tracker.surface.top.left") { if (!_parseVector3(st[1], out _tl)) { return false; } }
                                else if (st[0] == "tracker.surface.top.right") { if (!_parseVector3(st[1], out _tr)) { return false; } }
                            }
                        }
                    }
                }
            }
        }
        return true;
    }

    private bool _parseVector3(string str, out Vector3 point)
    {

        point = new Vector3(); ;
        string[] line = str.Split(':');
        if (line.Length == 3)
        {
            try
            {
                point.x = float.Parse(line[0].Replace(',', '.'));
                point.y = float.Parse(line[1].Replace(',', '.'));
                point.z = float.Parse(line[2].Replace(',', '.'));
            }
            catch
            {
                return false;
            }
        }
        else return false;

        return true;
    }
}
