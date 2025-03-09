using UnityEditor;
using UnityEngine;

public class Checker : MonoBehaviour
{
    #region Fields
    public int id;
    public Utils.Type type;
    public Renderer meshRenderer;
    public SpriteRenderer choosenSpriteRenderer;
    public SpriteRenderer baseSpriteRenderer;
    [SerializeField] Sprite kingSprite;

    [Header("Checker Settings:")]
    public Color choosenCheckerColor = new Color(0, 122, 255, 255);
    public Color highlightCheckerColor = new Color(0, 122, 255, 255);

    private bool isHighLighted = false;

    public Square CurrentSquare { get; set; }
    public bool IsKing { get; private set; } = false;
    #endregion

    #region Properties

    #endregion
    #region UnityMessages
    public void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            if (this != null)
            {
                GUIStyle style = new GUIStyle(EditorStyles.textField) { normal = new GUIStyleState() { textColor = Color.blue }, fontStyle = FontStyle.Bold };
                Handles.Label(transform.position + Vector3.up / 2, string.Format("{0}", this.id), style);
            }
        }
    }
    #endregion

    #region PublicMethods
    public void Init(int id)
    {
        this.id = id;
        gameObject.name = "Checker: " + id;
    }

    public void Select()
    {
        if (choosenSpriteRenderer != null)
        {
            choosenSpriteRenderer.enabled = true;
            choosenSpriteRenderer.color = choosenCheckerColor;
        }
    }

    public void Deselect()
    {
        if (choosenSpriteRenderer != null)
        {
            choosenSpriteRenderer.enabled = false;
            choosenSpriteRenderer.color = isHighLighted ? highlightCheckerColor : Color.white;
        }
    }

    public void PromoteToKing()
    {
        if (IsKing) { return; } 

        IsKing = true;
        baseSpriteRenderer.sprite = kingSprite;
    }

    public void Highlight(bool enable)
    {
        isHighLighted = enable;
        if (choosenSpriteRenderer != null)
        {
            choosenSpriteRenderer.enabled = isHighLighted;
            choosenSpriteRenderer.color = isHighLighted ? highlightCheckerColor : Color.white;
        }
    }
    #endregion

}
