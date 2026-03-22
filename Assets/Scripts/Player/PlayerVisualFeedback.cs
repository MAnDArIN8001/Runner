using UnityEngine;
using DG.Tweening;

/// <summary>
/// Damage flash, bonus pickup pulse, and invulnerability blink via renderer tint (MaterialPropertyBlock).
/// Avoids scaling the player root because the camera is parented under the player.
/// </summary>
[DisallowMultipleComponent]
public class PlayerVisualFeedback : MonoBehaviour
{
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorId = Shader.PropertyToID("_Color");

    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private PlayerData playerData;

    [Header("Damage")]
    [SerializeField] private Color damageFlashColor = new Color(1f, 0.35f, 0.35f, 1f);
    [SerializeField] private float damageFlashDuration = 0.12f;

    [Header("Pickup")]
    [SerializeField] private Color pickupFlashColor = new Color(0.4f, 1f, 0.55f, 1f);
    [SerializeField] private float pickupFlashDuration = 0.18f;

    [Header("Invulnerability")]
    [SerializeField] private Color invulnerableTint = new Color(0.45f, 0.85f, 1f, 1f);
    [SerializeField] private float invulnerabilityBlinkHalfPeriod = 0.12f;

    private MaterialPropertyBlock _block;
    private int _colorPropertyId = BaseColorId;
    private Color _baseColor;
    private Color _displayColor;

    private Tween _damageTween;
    private Tween _pickupTween;
    private Tween _invulnBlinkTween;

    private void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<Renderer>();
        if (playerData == null)
            playerData = GetComponent<PlayerData>();

        if (targetRenderer == null)
        {
            enabled = false;
            return;
        }

        _block = new MaterialPropertyBlock();

        var mat = targetRenderer.sharedMaterial;
        if (mat != null)
        {
            _colorPropertyId = mat.HasProperty(BaseColorId) ? BaseColorId : ColorId;
            _baseColor = mat.HasProperty(_colorPropertyId) ? mat.GetColor(_colorPropertyId) : Color.white;
        }
        else
        {
            _baseColor = Color.white;
        }

        _displayColor = _baseColor;
        ApplyDisplayColor(_displayColor);
    }

    private void OnEnable()
    {
        GameEvents.OnPlayerDamaged += OnPlayerDamaged;
        GameEvents.OnBonusPickedUp += OnBonusPickedUp;
        GameEvents.OnPlayerInvulnerabilityChanged += OnPlayerInvulnerabilityChanged;
        GameEvents.OnGameReset += OnGameReset;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerDamaged -= OnPlayerDamaged;
        GameEvents.OnBonusPickedUp -= OnBonusPickedUp;
        GameEvents.OnPlayerInvulnerabilityChanged -= OnPlayerInvulnerabilityChanged;
        GameEvents.OnGameReset -= OnGameReset;
        KillFeedbackTweens(false);
    }

    private void OnPlayerDamaged(int damage, int _)
    {
        if (targetRenderer == null)
            return;

        _damageTween?.Kill();
        _damageTween = DOTween.Sequence()
            .Append(DOTween.To(() => _displayColor, ApplyDisplayColor, damageFlashColor, damageFlashDuration * 0.45f)
                .SetEase(Ease.OutQuad))
            .Append(DOTween.To(() => _displayColor, ApplyDisplayColor, _baseColor, damageFlashDuration * 0.55f)
                .SetEase(Ease.InQuad));
    }

    private void OnBonusPickedUp(BonusPickupDefinition _)
    {
        if (targetRenderer == null)
            return;

        bool resumeInvulnBlink = playerData != null && playerData.IsInvulnerable;
        if (resumeInvulnBlink)
            StopInvulnerabilityBlink(false);

        _pickupTween?.Kill();
        _pickupTween = DOTween.Sequence()
            .Append(DOTween.To(() => _displayColor, ApplyDisplayColor, pickupFlashColor, pickupFlashDuration * 0.45f)
                .SetEase(Ease.OutQuad))
            .Append(DOTween.To(() => _displayColor, ApplyDisplayColor, resumeInvulnBlink ? invulnerableTint : _baseColor,
                    pickupFlashDuration * 0.55f)
                .SetEase(Ease.InQuad))
            .OnComplete(() =>
            {
                if (playerData != null && playerData.IsInvulnerable)
                    StartInvulnerabilityBlink();
                else
                    ApplyDisplayColor(_baseColor);
            });
    }

    private void OnPlayerInvulnerabilityChanged(bool active, float timeRemaining)
    {
        if (targetRenderer == null)
            return;

        if (active && timeRemaining > 0f)
        {
            if (_pickupTween != null && _pickupTween.IsActive())
                return;
            StartInvulnerabilityBlink();
            return;
        }

        StopInvulnerabilityBlink(true);
    }

    private void OnGameReset()
    {
        KillFeedbackTweens(true);
    }

    private void StartInvulnerabilityBlink()
    {
        _invulnBlinkTween?.Kill();
        _invulnBlinkTween = DOTween.Sequence()
            .Append(DOTween.To(() => _displayColor, ApplyDisplayColor, invulnerableTint, invulnerabilityBlinkHalfPeriod)
                .SetEase(Ease.InOutSine))
            .Append(DOTween.To(() => _displayColor, ApplyDisplayColor, _baseColor, invulnerabilityBlinkHalfPeriod)
                .SetEase(Ease.InOutSine))
            .SetLoops(-1, LoopType.Restart);
    }

    private void StopInvulnerabilityBlink(bool tweenToBase)
    {
        _invulnBlinkTween?.Kill();
        _invulnBlinkTween = null;
        if (!tweenToBase)
            return;

        DOTween.To(() => _displayColor, ApplyDisplayColor, _baseColor, 0.1f).SetEase(Ease.OutQuad);
    }

    private void KillFeedbackTweens(bool restoreBase)
    {
        _damageTween?.Kill();
        _damageTween = null;
        _pickupTween?.Kill();
        _pickupTween = null;
        _invulnBlinkTween?.Kill();
        _invulnBlinkTween = null;

        if (restoreBase && targetRenderer != null)
        {
            _displayColor = _baseColor;
            ApplyDisplayColor(_baseColor);
        }
    }

    private void ApplyDisplayColor(Color value)
    {
        _displayColor = value;
        if (targetRenderer == null || _block == null)
            return;

        targetRenderer.GetPropertyBlock(_block);
        _block.SetColor(_colorPropertyId, value);
        targetRenderer.SetPropertyBlock(_block);
    }
}
