using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.AI;

public class TilePiece : MonoBehaviour
{

    public Board b {get; private set;}
    public Vector3Int pos {get; private set;}
    public Vector3Int[] cells {get; private set;}
    public TetrominoData data {get; private set;}
    public int rotationIndex {get; private set;}
    public float stepDelay = 1f;
    public float lockDelay = 0.5f;
    private float stepTime;
    private float lockTime;
    public void Initialize(Board board, Vector3Int pos, TetrominoData data) {
        this.b = board;
        this.pos = pos;
        this.data = data;
        this.rotationIndex = 0;
        stepTime = Time.unscaledTime + this.stepDelay;
        lockTime = 0f;

        if(this.cells == null) {
            this.cells = new Vector3Int[data.cells.Length]; //basically just four, cuz every piece is 4 cells
        }

        for(int i =0; i< data.cells.Length; i++) {
            this.cells[i] = (Vector3Int)data.cells[i];
        }
    }


    private void Update() {

        this.b.Clear(this);
        this.lockTime += Time.unscaledDeltaTime;
        if(Input.GetKeyDown(KeyCode.RightArrow)) {
            Move(Vector2Int.right);
        } 
        else if(Input.GetKeyDown(KeyCode.LeftArrow)) {
            Move(Vector2Int.left);
        }
        else if(Input.GetKeyDown(KeyCode.DownArrow)) {
            Move(Vector2Int.down); 
        }
        else if(Input.GetKeyDown(KeyCode.S)) {
            Drop();
        }
        else if(Input.GetKeyDown(KeyCode.A)) {
            Rotate(-1);
        }
        else if(Input.GetKeyDown(KeyCode.D)) {
            Rotate(1);
        }

        if(Time.unscaledTime >= this.stepTime) {
            Step();
        }
        this.b.Set(this);
    }

    private void Step() {
        this.stepTime = Time.unscaledTime + this.stepDelay;
        Move(Vector2Int.down);

        if(this.lockTime >= this.lockDelay) {
            Lock();
        }
    }

    private void Lock() {
        this.b.Set(this);
        this.b.ClearLines();
        this.b.SpawnPiece();

;    }
    private void Rotate(int dir){  // taken from youtube || the math is too hard
        int currentRotation =  this.rotationIndex;
        this.rotationIndex = Wrap(this.rotationIndex +dir, 0, 4);
        DoingRotation(dir);

        if(!TestWallKicks(rotationIndex, dir)) {  // when flat against the wall, it wont rotate
            this.rotationIndex = currentRotation;
            DoingRotation(-dir);
        }

    }

    private void DoingRotation(int dir) {
        for(int i =0; i< this.cells.Length; i++) {
            UnityEngine.Vector3 cell = this.cells[i];

            int x, y;

           if(this.data.tetromino.Equals(Tetromino.I) || this.data.tetromino.Equals(Tetromino.O)) {
                cell.x -= 0.5f;
                cell.y -= 0.5f;
                x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * dir) + (cell.y * Data.RotationMatrix[1] * dir));
                y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * dir) + (cell.y * Data.RotationMatrix[3] * dir));
           } else { 
                x = Mathf.RoundToInt((cell.x * Data.RotationMatrix[0] * dir) + (cell.y * Data.RotationMatrix[1] * dir));
                y = Mathf.RoundToInt((cell.x * Data.RotationMatrix[2] * dir) + (cell.y * Data.RotationMatrix[3] * dir));
           }    

           this.cells[i] = new Vector3Int(x,y,0);
        }
    }

    private bool TestWallKicks(int rotationIndex, int RotateDir) { //taken from https://github.com/zigurous/unity-tetris-tutorial/blob/main/Assets/Scripts/Data.cs
        int wallKicksIndex = GetWallKickIndex(rotationIndex, RotateDir);
        //tests for 5 ish test for each "wallKick" and see which one works
        for(int i = 0; i <this.data.wallKicks.GetLength(1); i++) {
            Vector2Int move = this.data.wallKicks[wallKicksIndex, i];


            if(Move(move)) {
                return true;
            }
        }

        return false;
    }

    //getting what wallkick we need
    private int GetWallKickIndex (int rotationIndex, int RotateDir) { //taken from github
        int wallKicksIndex = rotationIndex * 2;

        if(RotateDir < 0) {
            wallKicksIndex--;
        }

        return Wrap(wallKicksIndex, 0, this.data.wallKicks.GetLength(0));
    }

    private int Wrap(int input, int min, int max) { //taken from youtube
        if(input < min) {
            return max - (min - input) % (max - min);
        } else  {
            return min - (input - max) %  (max - min);
        }
    }

    private void Drop() {
        while(Move(Vector2Int.down)) {
            continue;
        }
        Lock();
    }
    private bool Move( Vector2Int space) {
        Vector3Int newPos = this.pos;
        newPos.x += space.x;
        newPos.y += space.y;

        bool valid = this.b.isValidPos(this, newPos);

        if(valid) {
            this.pos = newPos;
            this.lockTime = 0f;
        }

        return valid;

    }
}
