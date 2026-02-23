using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    private bool isGrounded;
    private bool canJump;
    private int jumpCount;

    private PlayerState _currentState = PlayerState.Move;
    private int _currentLine;

    [SerializeField] private Vector3[] _lines;

    [Space]
    [SerializeField] private PlayerConfig config;
    [SerializeField] private PlayerData playerData;

    private Rigidbody rb;

    private Tween jumpTween;
    private Tween swapLaneTween;

    #region Initialization

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // Switch to kinematic to avoid physics bugs like unwanted bouncing

        Instance = this;
    }

    private void Start()
    {
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        if (_lines == null || _lines.Length == 0)
        {
            Debug.LogError("Lines not assigned!");
            enabled = false;
            return;
        }

        _currentLine = _lines.Length / 2;

        Vector3 startPos = transform.position;
        transform.position = new Vector3(_lines[_currentLine].x, startPos.y, startPos.z);

        jumpCount = 0;
        canJump = true;
        _currentState = PlayerState.Move;
    }

    #endregion

    private void OnEnable()
    {
        GameEvents.OnGameReset += ResetPlayer;
    }

    private void OnDisable()
    {
        GameEvents.OnGameReset -= ResetPlayer;
        KillAllTweens();
    }

    private void FixedUpdate()
    {
        if (!GameManager.Instance.IsGameRunning() || !playerData.IsAlive)
            return;

        CheckGround();

        // Manual forward movement to avoid physics inconsistencies
        if (_currentState != PlayerState.Jump)
        {
            float speed = GameManager.Instance.GetCurrentGameSpeed();
            transform.position += Vector3.forward * speed * Time.fixedDeltaTime;
        }

        if (!isGrounded && _currentState != PlayerState.Jump)
        {
            transform.position += Vector3.down * 9.81f * Time.fixedDeltaTime;
        }
    }

    #region Ground

    private void CheckGround()
    {
        isGrounded = Physics.Raycast(
            transform.position,
            Vector3.down,
            config.raycastDistance
        );

        if (isGrounded)
        {
            jumpCount = 0;
            canJump = true;
        }
    }

    #endregion

    #region Movement

    public void MoveRight()
    {
        if (!playerData.IsAlive) return;

        int targetLine = _currentLine - 1;

        if (targetLine >= 0)
            SwapLane(targetLine);
    }

    public void MoveLeft()
    {
        if (!playerData.IsAlive) return;

        int targetLine = _currentLine + 1;

        if (targetLine < _lines.Length)
            SwapLane(targetLine);
    }

    private void SwapLane(int targetLineIndex)
    {
        if (_currentState != PlayerState.Move)
            return;

        if (targetLineIndex == _currentLine)
            return;

        swapLaneTween?.Kill();

        _currentState = PlayerState.SwapLine;

        float duration = config.laneSwapDuration;
        float targetX = _lines[targetLineIndex].x;

        swapLaneTween = transform.DOMoveX(targetX, duration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                _currentLine = targetLineIndex;
                _currentState = PlayerState.Move;
                GameEvents.InvokePlayerMoved(targetLineIndex);
            });
    }

    #endregion

    #region Jump

    public void Jump()
    {
        if (!playerData.IsAlive) return;
        if (!canJump || jumpCount >= 2) return;

        jumpTween?.Kill();

        _currentState = PlayerState.Jump;

        jumpCount++;
        canJump = false;

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.forward * config.jumpLengthZ;

        float forwardSpeed = GameManager.Instance.GetCurrentGameSpeed();
        float duration = config.jumpDuration / forwardSpeed;

        jumpTween = transform.DOJump(
                endPos,
                config.jumpHeight,
                1,
                duration
            )
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                _currentState = PlayerState.Move;
                GameEvents.InvokePlayerJumped();
            });
    }

    #endregion

    #region Reset

    public void ResetPlayer()
    {
        KillAllTweens();

        jumpCount = 0;
        canJump = true;

        _currentLine = _lines.Length / 2;

        Vector3 pos = transform.position;
        transform.position = new Vector3(_lines[_currentLine].x, pos.y, pos.z);

        _currentState = PlayerState.Move;

        playerData?.ResetHealth();
    }

    #endregion

    private void KillAllTweens()
    {
        jumpTween?.Kill();
        swapLaneTween?.Kill();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.TryGetComponent(out Obstacle obstacle))
        {
            playerData.TakeDamage(obstacle.GetDamage());
            
            obstacle.Break();
        }
    }

    private enum PlayerState
    {
        Idle,
        Move,
        SwapLine,
        Jump
    }
}