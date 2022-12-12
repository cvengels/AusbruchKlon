using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    public GameObject blockPrefab;

    [TextArea(1, 12)]
    public string blockArrayBox;
    
    public class BlockColor : IEquatable<BlockColor>
    {
        public string ColorName { get; }
        public Color ColorValue { get; }

        public BlockColor(string colorName, Color colorValue)
        {
            this.ColorName = colorName;
            this.ColorValue = colorValue;
        }

        public override string ToString()
        {
            return "Name: " + ColorName + ", Value: " + ColorValue.ToString();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            BlockColor objAsBlockColor = obj as BlockColor;
            if (objAsBlockColor == null) return false;
            else return Equals(objAsBlockColor);
        }

        public bool Equals(BlockColor other)
        {
            if (other == null) return false;
            return other.ColorName.Equals(this.ColorName);
        }
    }

    public List<BlockColor> blockColors = new List<BlockColor>();

    private void Awake()
    {
        // Block color definitions
        blockColors.Add(new BlockColor("red", Color.red));
        blockColors.Add(new BlockColor("orange", new Color(255, 165, 0)));
        blockColors.Add(new BlockColor("yellow", Color.yellow));
        blockColors.Add(new BlockColor("green", Color.green));
        blockColors.Add(new BlockColor("cyan", Color.cyan));
        blockColors.Add(new BlockColor("blue", Color.blue));
        blockColors.Add(new BlockColor("magenta", Color.magenta));
        blockColors.Add(new BlockColor("white", Color.white));
        blockColors.Add(new BlockColor("gray", Color.gray));
        blockColors.Add(new BlockColor("black", Color.black));
        blockColors.Add(new BlockColor("none", Color.clear));
    }


    void Start()
    {
        if (blockPrefab != null && blockPrefab.CompareTag("Block"))
        {
            Debug.Log("Block Prefab gefunden");

            Vector2 maxBlocks;
            string[][] blockArray2D;
            
            Debug.Log("Vorher: \n" + blockArrayBox);
            CompileTextBlock(blockArrayBox, "magenta", out maxBlocks, out blockArray2D);
            
            // Display the array elements.
            string tmpString = "";
            for (int i = 0; i < blockArray2D.Length; i++)
            {
                for (int j = 0; j < blockArray2D[i].Length; j++)
                {
                    tmpString += j < blockArray2D[i].Length - 1 ? blockArray2D[i][j] + "," : blockArray2D[i][j];
                        
                }
                tmpString += "\n";
            }
            Debug.Log("Nachher: \n" + tmpString);
            
        }
        else
        {
            Debug.LogError("Block Prefab nicht gefunden");
        }
    }

    void CompileTextBlock(string textBlock, string defaultColor, out Vector2 maxItemsPerRow,
        out string[][] textBlockArray)
    {
        Vector2 maxLengths;
        string[][] jaggedArray;

        if (!blockColors.Exists(x => x.ColorName == defaultColor))
        {
            defaultColor = "gray";
        }
        
        if (!String.IsNullOrEmpty(textBlock))
        {
            // Get row count of text block
            string[] textLinesRaw = textBlock.Split('\n');
            int rowCount = textLinesRaw.Length;
            
            // Initialize jagged array with row count
            jaggedArray = new string[rowCount][];
            
            maxLengths.x = 0;
            maxLengths.y = rowCount;

            //Debug.Log(jaggedArray.Length + " Zeilen gefunden");
            
            // Fill array with content from rows
            for (int i = 0; i < rowCount; i++)
            {
                jaggedArray[i] = textLinesRaw[i].Split(',');
            }

            // Extract each row
            for (int i = 0; i < jaggedArray.Length; i++)
            {
                int tmpLength = jaggedArray[i].Length;
                if (tmpLength > maxLengths.x) maxLengths.x = tmpLength;
                //Debug.Log(tmpLength + " Elemente (roh): " + String.Join(" - ", jaggedArray[i]));
                
                // Check if value is usable
                for (int j = 0; j < jaggedArray[i].Length; j++)
                {
                    string tmpString = jaggedArray[i][j];
                    // Remove whitespaces
                    tmpString = new string(tmpString.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray());
                    tmpString = CheckIfNumberOrColor(tmpString) != "" ? CheckIfNumberOrColor(tmpString) : defaultColor;
                    jaggedArray[i][j] = tmpString;
                }
            }
        }
        else
        {
            Debug.LogError("Textblock konnte nicht geparst werden");
            maxLengths = Vector2.zero;
            jaggedArray = new string[][] {};
        }
        maxItemsPerRow = maxLengths;
        textBlockArray = jaggedArray;
    }

    string CheckIfNumberOrColor(string testSubject)
    {
        // Test for numbers
        int tmpNumberValue;
        if (Int32.TryParse(testSubject, out tmpNumberValue))
        {
            //Debug.Log("Zahlenwert gefunden : " + testSubject);
            if (tmpNumberValue >= 0 && tmpNumberValue <= blockColors.Count)
            {
                string colorByValue = blockColors.ElementAt(tmpNumberValue).ColorName;
                //Debug.Log(tmpNumberValue + ": " + colorByValue);
                return colorByValue;
            }
            else
            {
                return "";
            }
        }
        
        // Test for color strings
        if (blockColors.Exists(x => x.ColorName == testSubject))
        {
            return testSubject;
        }
        else
        {
            return "";
        }
    }
    

}

