using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    #region Fields
    [Header("Camera transforms:")]
    [SerializeField] private Transform Player1View;
    [SerializeField] private Transform Player2View;
    [SerializeField] private Transform TopDownTransform;

    [Space(10f)]
    private bool _isPerspectiveCamera;
    private float _currentFieldOfView;
    private Transform currentPlayerView;
    #endregion

    #region Properties
    public bool IsPerspectiveCamera { get => _isPerspectiveCamera; set => _isPerspectiveCamera = value; }
    #endregion

    #region UnityMessages
    void Start()
    {
        _currentFieldOfView = Camera.main.fieldOfView;
        InitCamera();
        GameManager.Instance.OnTurnSwitched += SwitchToPlayerView;
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnTurnSwitched -= SwitchToPlayerView;
        }
    }
    #endregion

    #region PublicMethods
    public void ToggleCameraView()
    {
        if (IsPerspectiveCamera)
        {
            SwitchToTopDown();
        }
        else
        {
            SwitchToPerspective();
        }
    }

    public void SwitchToPlayerView(Player player)
    {
        if (player.type == Utils.PlayerType.Human)
        {
            currentPlayerView = (player.ID == Utils.PlayerID.Player1) ? Player1View : Player2View;
        }


        if (_isPerspectiveCamera)
        {
            MoveCameraTo(currentPlayerView.position, currentPlayerView.rotation, _currentFieldOfView);
        }
        else
        {
            SwitchToTopDown();
        }
    }
    #endregion

    #region PrivateMethods
    private void InitCamera()
    {
        _isPerspectiveCamera = true;
        currentPlayerView = Player1View;
        MoveCameraTo(currentPlayerView.position, currentPlayerView.rotation, _currentFieldOfView);
    }

    private void SwitchToPerspective()
    {
        IsPerspectiveCamera = true;
        MoveCameraTo(currentPlayerView.position, currentPlayerView.rotation, _currentFieldOfView);
    }

    private void SwitchToTopDown()
    {
        IsPerspectiveCamera = false;
        Vector3 position = TopDownTransform.position;
        Quaternion rotation = currentPlayerView == Player1View ? Quaternion.Euler(90f, 0f, 0f) : Quaternion.Euler(90f, 180f, 0f);
        MoveCameraTo(position, rotation, GetTopDownFOV());
    }

    private void MoveCameraTo(Vector3 targetPosition, Quaternion targetRotation, float targetFOV)
    {
        Camera.main.transform.DOKill();
        Camera.main.transform.DOMove(targetPosition, 0.5f).SetEase(Ease.OutSine);

        Camera.main.transform
            .DORotateQuaternion(targetRotation, 0.5f)
            .SetEase(Ease.OutSine);

        DOTween.Kill("CameraFOV");
        DOTween.To(() => Camera.main.fieldOfView, x => Camera.main.fieldOfView = x, targetFOV, 1f)
            .SetEase(Ease.OutSine)
            .SetId("CameraFOV");
    }

    private float GetTopDownFOV()
    {
        return (Camera.main.aspect > 1.3f && Camera.main.aspect < 1.4f) ? 65f : 60f;
    }
    #endregion
}
