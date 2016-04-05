using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using PDollarGestureRecognizer;
using System.Collections.Generic;
using System.Xml;
using System;
using System.IO;

public class DataGestures : MonoBehaviour {

    public string[] requiredGestureNames;
    int currentGestureIndex = 0;

    Gesture[] gestureExamples;

    const string PATH = "";

    void Awake()
    {
        gestureExamples = LoadGesturesData();
    }


    /// <summary>
    /// Save the specified points and path.
    /// </summary>
    /// <param name="points">Points.</param>
    /// <param name="path">Path.</param>
    public static void Save(List<Point> points, int strokeCount, string filename, string gestureName)
    {

		XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
		{
			Indent = true,
		};

		using (XmlWriter writer = XmlWriter.Create(filename, xmlWriterSettings))
		{
			writer.WriteStartDocument();
			writer.WriteStartElement("Gesture");
			writer.WriteAttributeString("Name", gestureName);
			writer.WriteAttributeString("Subject", "10");
			writer.WriteAttributeString("InputType", "stylus");
			writer.WriteAttributeString("Speed", "MEDIUM");

			writer.WriteAttributeString("NumPts", points.Count.ToString());

			for(int i = 0; i < strokeCount; ++i)
			{
				writer.WriteStartElement("Stroke");
				writer.WriteAttributeString("index", (i+1).ToString());
				foreach(Point pt in points)
				{
					if(pt.StrokeID > strokeCount+1)
						break;
					writer.WriteStartElement("Point");
					writer.WriteAttributeString("X", pt.X.ToString());
					writer.WriteAttributeString("Y", pt.Y.ToString());
					writer.WriteEndElement();
				}
				writer.WriteEndElement();
			}
			

			writer.WriteEndElement();
			writer.Close();
		}
    }

	public static void WriteGesture(PDollarGestureRecognizer.Point[] points, string gestureName, string fileName)
        {
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>");
                sw.WriteLine("<Gesture Name = \"{0}\">", gestureName);
                int currentStroke = -1;
                for (int i = 0; i < points.Length; i++)
                {
                    if (points[i].StrokeID != currentStroke)
                    {
                        if (i > 0)
                            sw.WriteLine("\t</Stroke>");
                        sw.WriteLine("\t<Stroke>");
                        currentStroke = points[i].StrokeID;
                    }

                    sw.WriteLine("\t\t<Point X = \"{0}\" Y = \"{1}\" T = \"0\" Pressure = \"0\" />",
                        points[i].X, points[i].Y
                    );
                }
                sw.WriteLine("\t</Stroke>");
                sw.WriteLine("</Gesture>");
            }
        }

    /// <summary>
    /// Loads the gestures data from XML file
    /// </summary>
    /// <returns>The gestures data (points).</returns>
    Gesture[] LoadGesturesData()
    {
        List<Gesture> gestures = new List<Gesture>();

		string[] gestureFiles = new string[] { };
		UnityEngine.Object[] gFiles = Resources.LoadAll<TextAsset>("TextData/GestureSet");

		gestureFiles = new string[gFiles.Length];
		for (int i = 0; i < gFiles.Length; i++)
			gestureFiles[i] = (gFiles[i] as TextAsset).text;
        foreach (string data in gestureFiles)
            gestures.Add(ReadGestureDataString(data));
      
        return gestures.ToArray();
    }

	/// <summary>
    /// Reads the gesture data string.
    /// </summary>
    /// <returns>The gesture data string.</returns>
    /// <param name="data">Data.</param>
    public static Gesture ReadGestureDataString(string data)
    {
		XmlTextReader xmlReader = null;
		System.IO.StringReader stringReader = new System.IO.StringReader(data);
		xmlReader = new XmlTextReader(stringReader);
		return ReadGesture(xmlReader);
    }

	public static Gesture ReadGestureDataFromFile(string fileName) 
	{

		XmlTextReader xmlReader = null;
		Gesture gesture = null;
		
		try {
			
			xmlReader = new XmlTextReader(File.OpenText(fileName));
			gesture = ReadGesture(xmlReader);
				
		} finally {
			
			if (xmlReader != null)
				xmlReader.Close();
		}
		
		return gesture;
	}

	private static Gesture ReadGesture(XmlTextReader xmlReader)
    {
        List<Point> points = new List<Point>();
        int currentStrokeIndex = -1;
        string gestureName = "";
        try
        {
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType != XmlNodeType.Element) continue;
                switch (xmlReader.Name)
                {
                    case "Gesture":
                        gestureName = xmlReader["Name"];
                        if (gestureName.Contains("~")) // '~' character is specific to the naming convention of the MMG set
                            gestureName = gestureName.Substring(0, gestureName.LastIndexOf('~'));
                        if (gestureName.Contains("_")) // '_' character is specific to the naming convention of the MMG set
                            gestureName = gestureName.Replace('_', ' ');
                        break;
                    case "Stroke":
                        currentStrokeIndex++;
                        break;
                    case "Point":
                        points.Add(new Point(
                            float.Parse(xmlReader["X"]),
                            float.Parse(xmlReader["Y"]),
                            currentStrokeIndex
                        ));
                        break;
                }
            }
        }
		catch (Exception Ex)
		{
            Debug.Log("EXCEPTION: " + Ex.GetType().ToString());
            Debug.Log(Ex.Message);
        }
        finally
        {
            if (xmlReader != null)
                xmlReader.Close();
        }
        return new Gesture(points.ToArray(), gestureName);
    }


    public int GetGestureCount()
    {
    	return gestureExamples.Length;
    }

    public string GetGestureNameByIndex(int index)
    {	
    	if(gestureExamples != null)
    		return gestureExamples[index].Name;

    	else
    		return null;
    }

    /// <summary>
    /// Recognizes the gesture input by player
    /// </summary>
    /// <returns>The name of the gesture identified.</returns>
    /// <param name="currentGesturePoints">Current gesture points.</param>
    public string RecognizeGesture(List<Point> currentGesturePoints)
    {
		Debug.Log("*****************Recognizing Gestures*****************");
        Gesture candidate = new Gesture(currentGesturePoints.ToArray());
        string gestureClass = PointCloudRecognizer.Classify(candidate, gestureExamples);
		Debug.Log("Gesture classified: " + gestureClass);
		return gestureClass;
    }

    /// <summary>
    /// Determines whether this instance is required gesture recognized the specified reconizedGestureName.
    /// </summary>
    /// <returns><c>true</c> if this instance is required gesture recognized the specified reconizedGestureName; otherwise, <c>false</c>.</returns>
    /// <param name="reconizedGestureName">Reconized gesture name.</param>
	public bool IsRequiredGestureRecognized(string recognizedGestureName, int gestureid)
    {
    	bool res = false;

		string mod = recognizedGestureName.Replace(' ', '_');

		Debug.Log("Mod:" + mod);

		if(mod == requiredGestureNames[gestureid])
		{
			res = true;
		}

        return res;
    }
}
