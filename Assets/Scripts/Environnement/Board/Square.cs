using UnityEngine;
using UnityEngine.UIElements;

public class Square : MonoBehaviour
{
    #region Fields
    public enum Type
    {
        Black,
        White
    }
    public int id;
    public Type color;
    #endregion

    #region Properties
    public Checker currentChecker { get; private set; }
    public int row { get; private set; }
    public int col { get; private set; }
    #endregion

    #region PublicMethods
    public void InitSquare(int id, int boardSize)
    {
        this.id = id;

        row = id / boardSize;
        col = id % boardSize;

        if ((row + col) % 2 == 0)
        {
            color = Type.Black;
        }
        else
        {
            color = Type.White;
        }
    }

    public void PlaceChecker(Checker checker)
    {
        currentChecker = checker;
        checker.CurrentSquare = this;
    }

    public void RemoveChecker()
    {
        currentChecker = null;
    }

    public bool HasChecker()
    {
        return currentChecker != null;
    }

    public void Highlight(bool enable)
    {
        SpriteRenderer spriteRdr = transform.GetComponentInChildren<SpriteRenderer>();
        if (spriteRdr != null)
        {
            spriteRdr.enabled = enable;
            spriteRdr.color = Color.green;
        }
    }
    #endregion
}
