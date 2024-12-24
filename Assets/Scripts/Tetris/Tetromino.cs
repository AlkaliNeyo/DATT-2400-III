using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
public enum  Tetromino {
    I,
    O,
    T,
    J,
    L,
    S,
    Z,

}

[System.Serializable]
public struct TetrominoData {

    public Tetromino tetromino;
    public Tile tile;
    public Vector2Int[] cells { get; private set; }
    public Vector2Int[,] wallKicks { get; private set; }
    public void Initialize () {
       this.cells = Data.Cells[this.tetromino];
       this.wallKicks = Data.WallKicks[this.tetromino];
    }
}