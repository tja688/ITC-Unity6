using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class ScrollStackEffectController : IReplicaEffect<ScrollStackEffectConfig, ScrollStackEffectModel>
{
    public string EffectId => "scroll-stack-v2";

    private ReplicaHostContext _context;
    private ScrollStackEffectConfig _config;
    private ScrollStackEffectView _view;
    private ScrollStackEffectModel _model;
    private bool _initialized;

    private float _currentScrollY;
    private float _styledTargetScrollY;
    private float _styledVelocityY;
    private float _lastRawScrollY;
    private bool _hasRawScrollSample;
    private bool _programmaticDriving;
    private float _programmaticTargetY;
    private readonly List<float> _baseCardTops = new List<float>();

    public void Initialize(ReplicaHostContext context, ScrollStackEffectConfig config)
    {
        if (_initialized)
        {
            return;
        }

        _context = context ?? throw new ArgumentNullException(nameof(context));
        _config = config != null ? config : ScriptableObject.CreateInstance<ScrollStackEffectConfig>();
        _view = ScrollStackEffectViewBuilder.Build(_context.MountRoot, _config);
        _initialized = true;
    }

    public void SetModel(ScrollStackEffectModel model, bool animated = true)
    {
        if (!_initialized || _view == null)
        {
            return;
        }

        _model = model ?? new ScrollStackEffectModel();
        var items = _model.items ?? new List<ScrollStackItemModel>();
        if (_model.items == null)
        {
            _model.items = items;
        }

        // Clear existing
        foreach (var item in _view.Items)
        {
            if (item.Root != null) UnityEngine.Object.Destroy(item.Root.gameObject);
        }
        _view.Items.Clear();
        _baseCardTops.Clear();

        float currentY = 0;
        for (int i = 0; i < items.Count; i++)
        {
            var itemView = ScrollStackEffectViewBuilder.BuildItem(_view.Content, _config, i);
            var itemData = items[i] ?? new ScrollStackItemModel();

            if (itemView.Title != null) itemView.Title.text = itemData.title;
            if (itemView.Description != null) itemView.Description.text = itemData.description;
            if (itemView.Image != null && itemData.image != null) itemView.Image.sprite = itemData.image;
            if (itemView.Background != null) itemView.Background.color = itemData.backgroundColor;

            itemView.Root.anchoredPosition = new Vector2(0, -currentY);
            _baseCardTops.Add(currentY);


            _view.Items.Add(itemView);

            float itemHeight = itemView.Root.rect.height;
            currentY += itemHeight + _config.itemDistance;
        }

        // Layout content
        _view.Content.sizeDelta = new Vector2(0, currentY);
        if (_view.EndSpacer != null)
        {
            _view.EndSpacer.anchoredPosition = new Vector2(0, -currentY);
            _view.Content.sizeDelta = new Vector2(0, currentY + _view.EndSpacer.rect.height);
        }


        _currentScrollY = 0;
        _styledTargetScrollY = 0;
        _styledVelocityY = 0;
        _lastRawScrollY = 0;
        _hasRawScrollSample = false;
        _programmaticDriving = false;
        _programmaticTargetY = 0;
        if (_view.Scroller != null && _view.Scroller.content != null)
        {
            _view.Scroller.content.anchoredPosition = Vector2.zero;
        }
    }

    public void PlayIn(ReplicaTransition transition)
    {
        if (_view?.Root != null) _view.Root.gameObject.SetActive(true);
    }

    public void PlayOut(ReplicaTransition transition, Action onComplete = null)
    {
        if (_view?.Root != null) _view.Root.gameObject.SetActive(false);
        onComplete?.Invoke();
    }

    public void Tick(float deltaTime, float unscaledDeltaTime)
    {
        if (!_initialized || _view == null || _view.Scroller == null || _view.Scroller.content == null || _view.Scroller.viewport == null)
        {
            return;
        }

        var dt = Mathf.Max(0.0001f, _context != null && _context.UseUnscaledTime ? unscaledDeltaTime : deltaTime);
        var rawScrollY = _view.Scroller.content.anchoredPosition.y;

        if (!_hasRawScrollSample)
        {
            _hasRawScrollSample = true;
            _lastRawScrollY = rawScrollY;
            _styledTargetScrollY = rawScrollY;
            _currentScrollY = rawScrollY;
        }

        var rawDelta = rawScrollY - _lastRawScrollY;
        _lastRawScrollY = rawScrollY;

        if (_programmaticDriving)
        {
            _styledTargetScrollY = ClampToScrollBounds(_programmaticTargetY, false);
            _currentScrollY = Mathf.SmoothDamp(
                _currentScrollY,
                _styledTargetScrollY,
                ref _styledVelocityY,
                Mathf.Max(0.03f, _config.programmaticSmoothTime),
                Mathf.Max(100f, _config.programmaticMaxSpeed),
                dt);

            if (_config.syncScrollerOnProgrammaticDrive)
            {
                var synced = _view.Scroller.content.anchoredPosition;
                synced.y = _currentScrollY;
                _view.Scroller.content.anchoredPosition = synced;
                _lastRawScrollY = _currentScrollY;
            }

            if (Mathf.Abs(_currentScrollY - _styledTargetScrollY) <= Mathf.Max(0.1f, _config.programmaticSnapThreshold))
            {
                _currentScrollY = _styledTargetScrollY;
                _styledVelocityY = 0f;
                _programmaticDriving = false;
            }
        }
        else
        {
            if (Mathf.Abs(rawDelta) > 0.001f)
            {
                var boostedDelta = rawDelta * Mathf.Max(1f, _config.manualSensitivity);
                var burst = Mathf.Sign(rawDelta) * Mathf.Min(
                    Mathf.Abs(rawDelta) * Mathf.Abs(rawDelta) * Mathf.Max(0f, _config.manualBurst),
                    Mathf.Max(0f, _config.manualBurstClamp));
                _styledTargetScrollY += boostedDelta + burst;
            }

            _styledTargetScrollY = Mathf.Lerp(
                _styledTargetScrollY,
                rawScrollY,
                Mathf.Clamp01(dt * Mathf.Max(0f, _config.manualSettleToRaw)));

            _styledTargetScrollY = ClampToScrollBounds(_styledTargetScrollY, true);
            _currentScrollY = Mathf.SmoothDamp(
                _currentScrollY,
                _styledTargetScrollY,
                ref _styledVelocityY,
                Mathf.Max(0.02f, _config.manualSmoothTime),
                Mathf.Max(100f, _config.manualMaxSpeed),
                dt);
        }

        UpdateTransforms();
    }

    public void DriveToLayer(int layerIndex, bool animated = true)
    {
        if (!_initialized || _view?.Scroller == null || _baseCardTops.Count == 0)
        {
            return;
        }

        layerIndex = Mathf.Clamp(layerIndex, 0, _baseCardTops.Count - 1);
        var target = GetLayerPinStartScrollY(layerIndex);
        SetProgrammaticTarget(target, animated);
    }

    public void DriveToProgress(float progress01, bool animated = true)
    {
        if (!_initialized || _view?.Scroller == null)
        {
            return;
        }

        var target = Mathf.Lerp(0f, GetMaxScrollY(), Mathf.Clamp01(progress01));
        SetProgrammaticTarget(target, animated);
    }

    public void DriveToTop(bool animated = true)
    {
        DriveToProgress(0f, animated);
    }

    public void DriveToBottom(bool animated = true)
    {
        DriveToProgress(1f, animated);
    }

    private void SetProgrammaticTarget(float targetY, bool animated)
    {
        _programmaticTargetY = ClampToScrollBounds(targetY, false);
        _styledTargetScrollY = _programmaticTargetY;

        if (!animated)
        {
            _programmaticDriving = false;
            _styledVelocityY = 0f;
            _currentScrollY = _programmaticTargetY;

            if (_view?.Scroller?.content != null)
            {
                var synced = _view.Scroller.content.anchoredPosition;
                synced.y = _currentScrollY;
                _view.Scroller.content.anchoredPosition = synced;
                _lastRawScrollY = _currentScrollY;
            }

            return;
        }

        _programmaticDriving = true;
    }

    private float GetLayerPinStartScrollY(int layerIndex)
    {
        if (_view?.Scroller?.viewport == null || layerIndex < 0 || layerIndex >= _baseCardTops.Count)
        {
            return 0f;
        }

        var viewportHeight = _view.Scroller.viewport.rect.height;
        var stackPosPx = Mathf.Clamp01(_config.stackPosition) * viewportHeight;
        var cardBaseTop = _baseCardTops[layerIndex];
        var pinStart = cardBaseTop - stackPosPx - (_config.itemStackDistance * layerIndex);
        return ClampToScrollBounds(pinStart, false);
    }

    private float GetMaxScrollY()
    {
        if (_view?.Content == null || _view.Scroller?.viewport == null)
        {
            return 0f;
        }

        return Mathf.Max(0f, _view.Content.rect.height - _view.Scroller.viewport.rect.height);
    }

    private float ClampToScrollBounds(float value, bool allowOverscroll)
    {
        var max = GetMaxScrollY();
        if (!allowOverscroll)
        {
            return Mathf.Clamp(value, 0f, max);
        }

        var overscroll = Mathf.Max(0f, _config.manualOverscroll);
        return Mathf.Clamp(value, -overscroll, max + overscroll);
    }

    private void UpdateTransforms()
    {
        if (_view == null || _config == null || _view.Scroller == null || _view.Items.Count == 0 || _view.Scroller.viewport == null)
        {
            return;
        }

        float viewportHeight = _view.Scroller.viewport.rect.height;
        float stackPosPx = Mathf.Clamp01(_config.stackPosition) * viewportHeight;
        float scaleEndPosPx = Mathf.Clamp01(_config.scaleEndPosition) * viewportHeight;


        float endElementTop = 0;
        if (_view.EndSpacer != null)
        {
            endElementTop = Mathf.Abs(_view.EndSpacer.anchoredPosition.y);
        }

        for (int i = 0; i < _view.Items.Count; i++)
        {
            if (i >= _baseCardTops.Count)
            {
                break;
            }

            var item = _view.Items[i];
            if (item.Root == null) continue;

            float cardBaseTop = _baseCardTops[i];

            // Trigger calc
            float triggerStart = cardBaseTop - stackPosPx - _config.itemStackDistance * i;
            float triggerEnd = cardBaseTop - scaleEndPosPx;
            float pinStart = triggerStart;
            float pinEnd = endElementTop - viewportHeight / 2f;

            float scaleProgress = 0;
            if (_currentScrollY > triggerStart)
            {
                float range = triggerEnd - triggerStart;
                scaleProgress = range > 0.001f ? Mathf.Clamp01((_currentScrollY - triggerStart) / range) : 1f;
            }

            scaleProgress = EaseOutExpo(scaleProgress);

            float targetScale = _config.baseScale + i * _config.itemScale;
            float scale = 1f - scaleProgress * (1f - targetScale);
            float rotation = _config.rotationAmount != 0 ? i * _config.rotationAmount * scaleProgress : 0;

            float translateY = 0;
            if (_currentScrollY >= pinStart && _currentScrollY <= pinEnd)
            {
                translateY = -(stackPosPx + i * _config.itemStackDistance) - _currentScrollY + cardBaseTop;
            }
            else if (_currentScrollY > pinEnd)
            {
                translateY = -(stackPosPx + i * _config.itemStackDistance) - pinEnd + cardBaseTop;
            }

            if (_currentScrollY >= pinStart - 32f && _currentScrollY <= pinStart + 32f)
            {
                var enterBlend = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(pinStart - 32f, pinStart + 32f, _currentScrollY));
                translateY *= enterBlend;
            }

            item.Root.localScale = new Vector3(scale, scale, 1f);
            item.Root.localRotation = Quaternion.Euler(0, 0, rotation);

            var pos = item.Root.anchoredPosition;
            pos.y = -cardBaseTop + translateY;
            item.Root.anchoredPosition = pos;
        }
    }

    private static float EaseOutExpo(float t)
    {
        t = Mathf.Clamp01(t);
        if (t >= 1f)
        {
            return 1f;
        }

        return 1f - Mathf.Pow(2f, -10f * t);
    }

    public void Dispose()
    {
        if (_view?.Root != null)
        {
            UnityEngine.Object.Destroy(_view.Root.gameObject);
        }

        _view = null;
        _model = null;
        _context = null;
        _config = null;
        _baseCardTops.Clear();
        _styledTargetScrollY = 0f;
        _styledVelocityY = 0f;
        _lastRawScrollY = 0f;
        _hasRawScrollSample = false;
        _programmaticDriving = false;
        _programmaticTargetY = 0f;
        _initialized = false;
    }
}
