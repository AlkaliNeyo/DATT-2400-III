using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    GameScript gameScript;
    public TilePiece activeT { get; private set; }
    public Tilemap tileMap { get; private set; }
    public TetrominoData[] tetrominos;
    public Vector3Int spawnPos;
    ArrayList aList;
    public Vector2Int boardSize = new Vector2Int(10,20);

    public RectInt Bounds {
        get {
            Vector2Int pos = new Vector2Int(-this.boardSize.x/2, -this.boardSize.y/2);
            return new RectInt(pos, this.boardSize);
        }
    }

   void Awake() {

    this.tileMap = GetComponentInChildren<Tilemap>();
    this.gameScript = GameObject.Find("GameScript").GetComponent<GameScript>();
    this.activeT = GetComponentInChildren<TilePiece>();
    ArrayList aList = gameScript.blockList;


    for(int i=0; i<this.tetrominos.Length; i++) {
        this.tetrominos[i].Initialize();
    }
   }

   private void Start()
   {
        SpawnPiece();
   }

   public void SpawnPiece() {
    int last = gameScript.getLength() -1;
    int index = (int)gameScript.blockList[last];
    TetrominoData data = this.tetrominos[index];

    this.activeT.Initialize(this, spawnPos, data);
    if(isValidPos(this.activeT, this.spawnPos)) {
        Set(this.activeT);
    } else {
        GameOver();
    }

    gameScript.removeBlock();
   }

   private void GameOver() {
    this.tileMap.ClearAllTiles();
   }

   public void Set(TilePiece t) {
    for(int i=0; i<t.cells.Length; i++) {
        Vector3Int  tilePos = t.cells[i] + t.pos;
        this.tileMap.SetTile(tilePos, t.data.tile);
    }
   }
   public void Clear(TilePiece t) {
    for(int i=0; i<t.cells.Length; i++) {
        Vector3Int  tilePos = t.cells[i] + t.pos;
        this.tileMap.SetTile(tilePos, null);
    }
   }


   public bool isValidPos(TilePiece t, Vector3Int pos) {

        RectInt bounds = this.Bounds;
        for(int i =0; i<t.cells.Length; i++) {

            Vector3Int  tilePos = t.cells[i] + pos;

            if(!bounds.Contains((Vector2Int)tilePos)) {
                return false;
            }

            if(this.tileMap.HasTile(tilePos)) {
                return false;
            }
        }
        return true;
   }

   public void ClearLines(){

        RectInt bounds = this.Bounds;
        int row = bounds.yMin;

       while(row< bounds.yMax) {
            if (IsLineFull(row)) {
                LineClear(row);
            } else {
                row++;
            }
            }
   }

   private void LineClear(int row) {
    RectInt bounds = this.Bounds;
    
    for(int i = bounds.xMin; i < bounds.xMax; i++ ) {
        Vector3Int pos = new Vector3Int(i, row,0); 
        tileMap.SetTile(pos, null);
    }
    while(row < bounds.yMax) {
        for(int i = bounds.xMin; i < bounds.xMax; i++ ) {
        Vector3Int pos = new Vector3Int(i, row + 1, 0); 
        TileBase tileUp = this.tileMap.GetTile(pos);

        pos = new Vector3Int(i, row, 0);
        this.tileMap.SetTile(pos, tileUp);

    }
    row++;

    }
   }



   private bool IsLineFull(int row) {
    RectInt bounds = this.Bounds;
    
    for(int i = bounds.xMin; i < bounds.xMax; i++ ) {
        Vector3Int pos = new Vector3Int(i, row,0); 
        if(!tileMap.HasTile(pos)) {
            return false;
        }
    }
    return true;
   }
}


