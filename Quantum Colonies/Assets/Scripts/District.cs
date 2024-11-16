using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class District : MonoBehaviour, IPointerClickHandler
{
    public int row, column;

    public bool isShown = false;
    public bool isBombed = false;
    public int adjacentBombs = 0;
    public District entangledDistrict = null;

    // Selection
    public bool selectedAsBomb = false;
    public bool selectedAsCure = false;

    public void ShowDistrict(){
        isShown = true;
        GetComponent<Image>().color = new Color(50, 50, 50);
        GetComponent<Button>().enabled = false;
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public void UpdatePosition(int row, int column){
        this.row = row;
        this.column = column;
        float rowDif = (StaticVariables.endingArea.Item1 - StaticVariables.beginningArea.Item1) / StaticVariables.levelSelector.levels[StaticVariables.currentLevel].rows;
        float colDif = Math.Abs(StaticVariables.endingArea.Item2 - StaticVariables.beginningArea.Item2) / StaticVariables.levelSelector.levels[StaticVariables.currentLevel].columns;
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector2 initialPosition = new(rectTransform.sizeDelta.x / 2 + StaticVariables.beginningArea.Item1, StaticVariables.beginningArea.Item2 - rectTransform.sizeDelta.y / 2);
        gameObject.transform.localPosition = new Vector3(initialPosition.x + rowDif * column, initialPosition.y - colDif * row, 0);
        gameObject.name = "District_" + row + "-" + column;
    }

    public void OnPointerClick(PointerEventData eventData){
        if(isShown)
            return;
        if(eventData.button == PointerEventData.InputButton.Middle){
            if(selectedAsCure){
                Deselect();
                StaticVariables.moves.Push(new Tuple<District, int>(this, 1));
            }
            else if(selectedAsBomb){
                Deselect();
                StaticVariables.moves.Push(new Tuple<District, int>(this, 2));
            }
        }
        if(eventData.button == PointerEventData.InputButton.Left){
            if(selectedAsCure){
                Deselect();
                StaticVariables.moves.Push(new Tuple<District, int>(this, 1));
            }
            else if(selectedAsBomb){
                SelectAsCure();
                StaticVariables.moves.Push(new Tuple<District, int>(this, 2));
            }
            else{
                SelectAsCure();
                StaticVariables.moves.Push(new Tuple<District, int>(this, 0));
            }
        }
        else if(eventData.button == PointerEventData.InputButton.Right){
            if(selectedAsBomb){
                Deselect();
                StaticVariables.moves.Push(new Tuple<District, int>(this, 2));
            }
            else if(selectedAsCure){
                SelectAsBomb();
                StaticVariables.moves.Push(new Tuple<District, int>(this, 1));
            }
            else{
                SelectAsBomb();
                StaticVariables.moves.Push(new Tuple<District, int>(this, 0));
            }
        }
        Debug.Log(StaticVariables.moves.Count);
    }

    public void SelectAsCure(){
        selectedAsCure = true;
        selectedAsBomb = false;
        GetComponent<Image>().color = Color.blue;
    }

    public void SelectAsBomb(){
        selectedAsBomb = true;
        selectedAsCure = false;
        GetComponent<Image>().color = Color.red;
    }

    public void Deselect(){
        if(!isShown){
            selectedAsCure = false;
            selectedAsBomb = false;
            GetComponent<Image>().color = Color.grey;
        }
    }

    public bool CheckSolution(){
        bool result = true;
        if(selectedAsCure || selectedAsBomb)
            GetComponent<Button>().enabled = false;
        else 
            return true;
        
        if (selectedAsBomb && !isBombed || !selectedAsBomb && isBombed){
            result = false;
            // End game
        }
        else if (selectedAsCure && isBombed || !selectedAsCure && !isBombed){
            result = false;
            // End game
        }

        ShowDistrict();
        if(result == false)
            GetComponent<Image>().color = Color.black;
        selectedAsBomb = false;
        selectedAsCure = false;
        return result;
    }
}
