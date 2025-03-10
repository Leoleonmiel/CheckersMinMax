using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Fields
    private Utils.PlayerID id;
    public Utils.Type color;
    public Utils.PlayerType type;
    public List<Checker> checkers = new();

    #endregion

    #region Properties
    public Utils.PlayerID ID => id;
    #endregion
    #region PublicMethods
    public void Init(Utils.PlayerID id, Utils.PlayerType type)
    {
        this.id = id;
        this.type = type;
        gameObject.name = $"{id}";
    }

    public void ResetCheckersChoosing()
    {
        foreach (Checker checker in checkers)
        {
            checker.choosenSpriteRenderer.enabled = false;
            checker.choosenSpriteRenderer.color = Color.white;
        }
    }

    public void SelectChecker(Checker selectedChecker, Color chosenColor)
    {
        ResetCheckersChoosing();

        if (checkers.Contains(selectedChecker))
        {
            selectedChecker.choosenSpriteRenderer.enabled = true;
            selectedChecker.choosenSpriteRenderer.color = chosenColor;
        }
    }
    #endregion
}
