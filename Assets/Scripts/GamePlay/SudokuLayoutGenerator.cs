using System;
using UnityEngine;

public class SudokuLayoutGenerator : MonoBehaviour
{
    public GameObject numberBlockPrefab;
    public float blockSize = 0.59f;
    public float blockGap = 0.13f;
    public Vector2 offsetPosition;

    public static SudokuLayoutGenerator _instance;

    void Awake(){
        _instance = this;
    }

    public static SudokuLayoutGenerator Instance() {
	if(_instance == null) {
            Debug.LogWarning("Error: Nothing instantiated as of now!");
        }
        return _instance;
    }    

    void Start(){
        // GenerateSudokuLayout();
    }

    public NumberBlock[,] GenerateSudokuLayout(){
        string holderName = "SudokuHolder";
	if(transform.Find(holderName)){
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        Transform sudokuHolder = new GameObject(holderName).transform;
        sudokuHolder.parent = transform;

        var numberBlocks = new NumberBlock[9, 9];
        for (int i = 0; i < 9; i++) {
            for (int j = 0; j < 9; j++) {
                Vector3 newPos = new Vector3(offsetPosition.x + ((j - 4) * (blockSize/2f + blockGap)),
					     offsetPosition.y + ((-i + 4) * (blockSize/2f + blockGap)),
					     -2);

                GameObject newObj = Instantiate(numberBlockPrefab, newPos, Quaternion.identity);
                newObj.transform.localScale = new Vector3(blockSize, blockSize, 1);
                NumberBlock block = newObj.GetComponent<NumberBlock>();
                block.transform.parent = sudokuHolder;

		block.location.x = j;
                block.location.y = i;
                numberBlocks[i, j] = block;
            }
	}

        return numberBlocks;
    }

}


