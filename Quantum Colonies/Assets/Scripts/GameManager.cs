using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public AssetReference reference;
    public District[][] districts;
    public Tuple<float, float> districtSize;
    private Level currentLevel;
    private GameObject canvas;
    private TMPro.TextMeshProUGUI turnText, casualtiesText;

    void Start(){
        canvas = GameObject.FindWithTag(Tags.Canvas);
        turnText = GameObject.Find("Turn").GetComponent<TMPro.TextMeshProUGUI>();
        casualtiesText = GameObject.Find("Casualties").GetComponent<TMPro.TextMeshProUGUI>();

        StaticVariables.turn = 0;
        currentLevel = StaticVariables.levelSelector.levels[StaticVariables.currentLevel];
        districtSize = new Tuple<float, float>(1000 / currentLevel.rows, 1000 / currentLevel.columns);

        AsyncOperationHandle handle = reference.LoadAssetAsync<GameObject>();
        handle.Completed += (operation) => {
            InstantiateUI();
        };
    }

    public void CheckPartialSolutions(){
        bool result = true;
        bool selected = false;
        bool allSelected = true;
        for (int i = 0; i < currentLevel.rows; i++){
            for(int j = 0; j < currentLevel.columns; j++){
                if(districts[i][j].isShown){
                    continue;
                }
                if(!districts[i][j].selectedAsBomb && !districts[i][j].selectedAsCure){
                    allSelected = false;
                    continue;
                }
                selected = true;
                bool tempResult = districts[i][j].CheckSolution();
                result &= tempResult;
                if(tempResult)
                    Debug.Log("You were right about district " + i + "-" + j);
                else{
                    Debug.Log("You were wrong about district " + i + "-" + j);
                    casualtiesText.text = "Casualties: " + ++StaticVariables.casualties;
                }
            }
        }
        if(!selected){
            Debug.Log("You didn't select any district");
            return;
        }
        else{
            turnText.text = "Turn: " + ++StaticVariables.turn;
            StaticVariables.moves.Clear();
        }
        
        if(allSelected){
            if(result)
                Debug.Log("You won!");
            else
                Debug.Log("You lost!");
        }
    }

    public void CheckFinalSolution(){
        bool selected = true;
        for (int i = 0; i < currentLevel.rows; i++){
            for(int j = 0; j < currentLevel.columns; j++){
                if(districts[i][j].isShown == false && !districts[i][j].selectedAsBomb && !districts[i][j].selectedAsCure){
                    selected = false;
                }
            }
        }
        if(!selected){
            Debug.Log("You didn't select all districts");
            return;
        }
        turnText.text = "Turn: " + ++StaticVariables.turn;
        StaticVariables.moves.Clear();

        bool result = true;
        for (int i = 0; i < currentLevel.rows; i++){
            for(int j = 0; j < currentLevel.columns; j++){
                bool tempResult = districts[i][j].CheckSolution();
                result &= tempResult;
            }
        }

        if(result)
            Debug.Log("You won!");
        else
            Debug.Log("You lost!");
    }

    public void Undo(){
        if(StaticVariables.moves.Count == 0)
            return;
        
        var lastItem = StaticVariables.moves.Pop();
        if(((Tuple<District, int>)lastItem).Item2 == 0)
            ((Tuple<District, int>)lastItem).Item1.Deselect();
        else if(((Tuple<District, int>)lastItem).Item2 == 1)
            ((Tuple<District, int>)lastItem).Item1.SelectAsCure();
        else if(((Tuple<District, int>)lastItem).Item2 == 2)
            ((Tuple<District, int>)lastItem).Item1.SelectAsBomb();
    }

    public void ClearCells(){
        for (int i = 0; i < currentLevel.rows; i++){
            for(int j = 0; j < currentLevel.columns; j++){
                districts[i][j].Deselect();
            }
        }
    }

    private void InstantiateUI(){
        Level currentLevel = StaticVariables.levelSelector.levels[StaticVariables.currentLevel];
        GameObject.Find("Turn").GetComponent<TMPro.TextMeshProUGUI>().text = "Turn: " + StaticVariables.turn;

        districts = new District[currentLevel.rows][];
        Vector2 districtSize = new(1000 / currentLevel.rows, 1000 / currentLevel.columns);
        for (int i = 0; i < currentLevel.rows; i++){
            districts[i] = new District[currentLevel.columns];
            GameObject currentRow = new("Row_" + i);
            currentRow.transform.SetParent(canvas.transform);
            currentRow.transform.localPosition = new Vector3(0, 0, 0);
            for(int j = 0; j < currentLevel.columns; j++){
                districts[i][j] = Instantiate(reference.Asset, new Vector3(i, j, 0), Quaternion.identity).GetComponent<District>();
                districts[i][j].transform.SetParent(currentRow.transform);
                districts[i][j].GetComponent<RectTransform>().sizeDelta = districtSize;
                districts[i][j].UpdatePosition(i, j);
            }
        }

        foreach(var bomb in currentLevel.bombs){
            districts[bomb.Item1][bomb.Item2].isBombed = true;
        }

        foreach(var entanglement in currentLevel.entanglements){
            districts[entanglement.Item1.Item1][entanglement.Item1.Item2].entangledDistrict = districts[entanglement.Item2.Item1][entanglement.Item2.Item2];
            districts[entanglement.Item2.Item1][entanglement.Item2.Item2].entangledDistrict = districts[entanglement.Item1.Item1][entanglement.Item1.Item2];
        }

        for (int i = 0; i < currentLevel.rows; i++){
            for(int j = 0; j < currentLevel.columns; j++){
                if(districts[i][j].isBombed)
                    continue;
                ComputeAdjacentBombs(districts[i][j]);
                districts[i][j].transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = districts[i][j].adjacentBombs.ToString();
            }
        }

        foreach(var initialCell in currentLevel.initialCells){
            districts[initialCell.Item1][initialCell.Item2].ShowDistrict();
        }

        reference.ReleaseAsset();
    }

    private void ComputeAdjacentBombs(District district){
        int row = district.row;
        int column = district.column;
        int[] rows = new int[] { -1, -1, -1, 0, 0, 1, 1, 1 };
        int[] columns = new int[] { -1, 0, 1, -1, 1, -1, 0, 1 };
        for(int i = 0; i < 8; i++){
            if(InsideGrid(row + rows[i], column + columns[i]) && districts[row + rows[i]][column + columns[i]].isBombed)
                district.adjacentBombs++;
        }
    }

    private bool InsideGrid(int row, int column){
        if(row < 0 || row >= currentLevel.rows || column < 0 || column >= currentLevel.columns)
            return false;
        return true;
    }
    
    // private void OnDestroy(){
    // }
}
