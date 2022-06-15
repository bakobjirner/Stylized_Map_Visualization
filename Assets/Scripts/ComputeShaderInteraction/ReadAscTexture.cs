using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadAscTexture : MonoBehaviour
{

    private Texture2D texture;
    public int width = 10000;
    public int height = 10000;
    private int[][] heightValues;
    public TextAsset textAsset;
    
    // Start is called before the first frame update
    void Start()
    {
        texture = new Texture2D(width, height);
        ReadFile();
    }

    private void ReadFile()
    {
        
        
    }
    
    private void CreateTexture()
    {
        
    }
}
