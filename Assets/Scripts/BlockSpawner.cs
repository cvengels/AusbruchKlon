using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    [SerializeField] public GameObject blockPrefab;

    [SerializeField] public float blockOffset;

    [TextArea(1, 12), SerializeField] public string blockArrayBox;
    
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


            string[][] blockArray2D;
            
            Debug.Log("Vorher: \n" + blockArrayBox);
            CompileTextBlock(blockArrayBox, "magenta", out blockArray2D);

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
            
            GenerateBlocks(blockPrefab, blockArray2D, blockOffset);
        }
        else
        {
            Debug.LogError("Block Prefab nicht gefunden");
        }
    }


    void GenerateBlocks(GameObject blockPrefab, string[][] blockArray, float offset)
    {
        if (blockPrefab != null)
        {
            Vector2Int maxBlocks = Vector2Int.zero;
            Vector2 blockSize = Vector2.zero;
            Vector2 blockRowStart = Vector2.zero;
            maxBlocks.y = blockArray.Length;

            foreach (var blockRow in blockArray)
            {
                maxBlocks.x = Mathf.Max(maxBlocks.x, blockRow.Length);
            }

            blockSize = CalculateBrickSize(transform, new Vector2(blockArray[0].Length, blockArray[0].Length), offset);
            
            print(offset + "; " + blockArray[0].Length + "; " + blockSize.x);
            
            blockRowStart = new Vector2(
                blockArray[0].Length % 2 == 0 ? 
                    transform.position.x - (offset / 2) - (offset * ((blockArray[0].Length / 2) - 1)) - ((blockArray[0].Length / 2) * blockSize.x) : 
                    transform.position.x - (blockSize.x / 2) - (offset * (blockArray[0].Length - 1 / 2)) - (blockArray[0].Length - 1 / 2),
                transform.position.y + (transform.localScale.y / 2) - (transform.localScale.y / 2) - (offset + blockSize.y / 2)
            );

            for (int i = 0; i < blockArray[0].Length; i++)
            {
                Vector2 startPoint = new Vector2(
                    blockRowStart.x + ((i * offset) + (i * blockSize.x)),
                    blockRowStart.y
                    );
                GameObject newBlock = Instantiate(blockPrefab, startPoint, Quaternion.identity);
                newBlock.transform.localScale = blockSize;
                newBlock.name = "Block " + (i + 1);
                Color myColor = Color.clear; ColorUtility.TryParseHtmlString (blockArray[0][i], out myColor);
                newBlock.GetComponent<SpriteRenderer>().color = myColor;
            }
        }
    }

    Vector2 CalculateBrickSize(Transform generatorSize, Vector2 blockCount, float offset)
    {
        Vector2 blockSize = new Vector2(
            (transform.localScale.x - ((blockCount.x + 1) * offset)) / blockCount.x,
            (transform.localScale.y - ((blockCount.y + 1) * offset)) / blockCount.y
        );
        
        if (blockSize.x <= 0f || blockSize.y <= 0f)
        {
            Debug.Log("Offset der Blöcke zu groß. wird auf " + offset * 0.95f + "geändert und neu probiert ...");
            blockSize = CalculateBrickSize(generatorSize, blockCount, offset * 0.95f);
        }
        return blockSize;
    }

    void CompileTextBlock(string textBlock, string defaultColor, out string[][] textBlockArray)
    {
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

            //Debug.Log(jaggedArray.Length + " Zeilen gefunden");
            
            // Fill array with content from rows
            for (int i = 0; i < rowCount; i++)
            {
                jaggedArray[i] = textLinesRaw[i].Split(',');
            }

            // Extract each row
            for (int i = 0; i < jaggedArray.Length; i++)
            {
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
            jaggedArray = new string[][] {};
        }
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

