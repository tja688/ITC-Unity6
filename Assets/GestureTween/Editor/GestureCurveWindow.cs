using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

namespace GestureTween.Editor
{
    /// <summary>
    /// 手绘曲线编辑窗口 - 通过绘制生成 AnimationCurve
    /// </summary>
    public class GestureCurveWindow : EditorWindow
    {
        #region Stroke Data

        private struct StrokeSample
        {
            public Vector2 position;
            public float timestamp;
        }

        private readonly List<StrokeSample> _samples = new();
        private bool _isDrawing;

        #endregion

        #region Generated Curve

        private AnimationCurve _generatedCurve;
        private float _smoothing = 0.3f;

        #endregion

        #region Canvas Settings

        private const float CanvasWidth = 400f;
        private const float CanvasHeight = 300f;
        private const float CanvasPadding = 20f;
        private Rect _canvasRect;

        #endregion

        #region Preview

        private Tween _previewTween;
        private float _previewProgress;

        #endregion

        [MenuItem("Window/GestureTween/Curve Drawer")]
        public static void ShowWindow()
        {
            var window = GetWindow<GestureCurveWindow>("Gesture Curve");
            window.minSize = new Vector2(450, 500);
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("手绘曲线生成器", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("在画布区域绘制：快速绘制 = 加速，慢速绘制 = 减速", MessageType.Info);

            EditorGUILayout.Space(10);

            // 画布区域
            DrawCanvas();

            EditorGUILayout.Space(10);

            // 控制面板
            DrawControls();

            EditorGUILayout.Space(10);

            // 曲线预览
            DrawCurvePreview();

            EditorGUILayout.Space(10);

            // 导出按钮
            DrawExportButtons();

            // 处理重绘
            if (_isDrawing || (_previewTween != null && _previewTween.IsActive()))
            {
                Repaint();
            }
        }

        #region Canvas Drawing

        private void DrawCanvas()
        {
            EditorGUILayout.LabelField("绘制区域", EditorStyles.miniBoldLabel);

            _canvasRect = GUILayoutUtility.GetRect(CanvasWidth, CanvasHeight);
            _canvasRect.x += CanvasPadding;
            _canvasRect.width -= CanvasPadding * 2;

            // 背景
            EditorGUI.DrawRect(_canvasRect, new Color(0.15f, 0.15f, 0.15f));

            // 网格线
            DrawGrid();

            // 绘制笔触
            DrawStroke();

            // 处理输入
            HandleInput();

            // 边框
            Handles.color = Color.gray;
            Handles.DrawWireDisc(_canvasRect.center, Vector3.forward, 0); // 触发 Handles 初始化
            var corners = new Vector3[]
            {
                new(_canvasRect.xMin, _canvasRect.yMin, 0),
                new(_canvasRect.xMax, _canvasRect.yMin, 0),
                new(_canvasRect.xMax, _canvasRect.yMax, 0),
                new(_canvasRect.xMin, _canvasRect.yMax, 0),
                new(_canvasRect.xMin, _canvasRect.yMin, 0)
            };
            Handles.DrawPolyLine(corners);
        }

        private void DrawGrid()
        {
            Handles.color = new Color(0.3f, 0.3f, 0.3f, 0.5f);

            // 水平线
            for (int i = 1; i < 4; i++)
            {
                float y = _canvasRect.yMin + _canvasRect.height * i / 4f;
                Handles.DrawLine(new Vector3(_canvasRect.xMin, y), new Vector3(_canvasRect.xMax, y));
            }

            // 垂直线
            for (int i = 1; i < 4; i++)
            {
                float x = _canvasRect.xMin + _canvasRect.width * i / 4f;
                Handles.DrawLine(new Vector3(x, _canvasRect.yMin), new Vector3(x, _canvasRect.yMax));
            }
        }

        private void DrawStroke()
        {
            if (_samples.Count < 2) return;

            Handles.color = new Color(0.2f, 0.8f, 1f);

            for (int i = 0; i < _samples.Count - 1; i++)
            {
                var p1 = _samples[i].position;
                var p2 = _samples[i + 1].position;
                Handles.DrawLine(new Vector3(p1.x, p1.y), new Vector3(p2.x, p2.y));
            }

            // 绘制点
            Handles.color = Color.white;
            foreach (var sample in _samples)
            {
                Handles.DrawSolidDisc(new Vector3(sample.position.x, sample.position.y), Vector3.forward, 2f);
            }
        }

        private void HandleInput()
        {
            Event e = Event.current;

            if (!_canvasRect.Contains(e.mousePosition)) return;

            switch (e.type)
            {
                case EventType.MouseDown when e.button == 0:
                    _isDrawing = true;
                    _samples.Clear();
                    _generatedCurve = null;
                    AddSample(e.mousePosition);
                    e.Use();
                    break;

                case EventType.MouseDrag when _isDrawing && e.button == 0:
                    AddSample(e.mousePosition);
                    e.Use();
                    break;

                case EventType.MouseUp when e.button == 0:
                    if (_isDrawing && _samples.Count >= 2)
                    {
                        GenerateCurve();
                    }
                    _isDrawing = false;
                    e.Use();
                    break;
            }
        }

        private void AddSample(Vector2 mousePos)
        {
            _samples.Add(new StrokeSample
            {
                position = mousePos,
                timestamp = (float)EditorApplication.timeSinceStartup
            });
        }

        #endregion

        #region Curve Generation

        private void GenerateCurve()
        {
            if (_samples.Count < 2) return;

            // 计算每段的速度
            var velocities = new List<float>();
            for (int i = 1; i < _samples.Count; i++)
            {
                var delta = _samples[i].position - _samples[i - 1].position;
                var timeDelta = _samples[i].timestamp - _samples[i - 1].timestamp;
                float speed = timeDelta > 0.001f ? delta.magnitude / timeDelta : 0f;
                velocities.Add(speed);
            }

            // 归一化速度
            float maxVelocity = 0f;
            foreach (var v in velocities) if (v > maxVelocity) maxVelocity = v;
            if (maxVelocity < 0.001f) maxVelocity = 1f;

            // 构建曲线：速度快的地方进度增加快
            _generatedCurve = new AnimationCurve();
            float accumulatedProgress = 0f;
            float totalTime = _samples[^1].timestamp - _samples[0].timestamp;

            _generatedCurve.AddKey(new Keyframe(0f, 0f));

            for (int i = 0; i < velocities.Count; i++)
            {
                float normalizedTime = (_samples[i + 1].timestamp - _samples[0].timestamp) / totalTime;
                float normalizedVelocity = velocities[i] / maxVelocity;

                // 速度越快，进度增加越多
                float progressStep = normalizedVelocity / velocities.Count;
                accumulatedProgress += progressStep;

                // 应用平滑插值
                float smoothedProgress = Mathf.Lerp(normalizedTime, accumulatedProgress, 1f - _smoothing);

                _generatedCurve.AddKey(new Keyframe(normalizedTime, Mathf.Clamp01(smoothedProgress)));
            }

            // 确保曲线从 (0,0) 到 (1,1)
            NormalizeCurve();

            // 平滑切线
            SmoothCurveTangents();
        }

        private void NormalizeCurve()
        {
            if (_generatedCurve == null || _generatedCurve.length < 2) return;

            var keys = _generatedCurve.keys;

            // 确保第一个点是 (0, 0)
            keys[0] = new Keyframe(0f, 0f);

            // 确保最后一个点是 (1, 1)
            keys[^1] = new Keyframe(1f, 1f);

            _generatedCurve.keys = keys;
        }

        private void SmoothCurveTangents()
        {
            if (_generatedCurve == null) return;

            for (int i = 0; i < _generatedCurve.length; i++)
            {
                AnimationUtility.SetKeyLeftTangentMode(_generatedCurve, i, AnimationUtility.TangentMode.ClampedAuto);
                AnimationUtility.SetKeyRightTangentMode(_generatedCurve, i, AnimationUtility.TangentMode.ClampedAuto);
            }
        }

        #endregion

        #region Controls

        private void DrawControls()
        {
            EditorGUILayout.BeginHorizontal();

            // 平滑度
            EditorGUILayout.LabelField("平滑度", GUILayout.Width(50));
            float newSmoothing = EditorGUILayout.Slider(_smoothing, 0f, 1f);
            if (!Mathf.Approximately(newSmoothing, _smoothing))
            {
                _smoothing = newSmoothing;
                if (_samples.Count >= 2)
                {
                    GenerateCurve();
                }
            }

            EditorGUILayout.Space(20);

            // 清除按钮
            if (GUILayout.Button("清除", GUILayout.Width(60)))
            {
                _samples.Clear();
                _generatedCurve = null;
                _previewTween?.Kill();
                _previewProgress = 0f;
            }

            EditorGUILayout.EndHorizontal();
        }

        #endregion

        #region Curve Preview

        private void DrawCurvePreview()
        {
            EditorGUILayout.LabelField("生成曲线", EditorStyles.miniBoldLabel);

            if (_generatedCurve != null)
            {
                // 曲线字段
                EditorGUI.BeginChangeCheck();
                _generatedCurve = EditorGUILayout.CurveField("曲线预览", _generatedCurve, Color.cyan, new Rect(0, 0, 1, 1));
                if (EditorGUI.EndChangeCheck())
                {
                    // 用户手动编辑了曲线
                }

                EditorGUILayout.Space(5);

                // 预览控制
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("▶ 预览", GUILayout.Width(80)))
                {
                    PlayPreview();
                }

                EditorGUILayout.LabelField($"进度: {_previewProgress:P0}", GUILayout.Width(100));

                // 进度条
                var progressRect = GUILayoutUtility.GetRect(100, 20);
                EditorGUI.ProgressBar(progressRect, _previewProgress, "");

                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.HelpBox("在上方画布绘制以生成曲线", MessageType.None);
            }
        }

        private void PlayPreview()
        {
            _previewTween?.Kill();
            _previewProgress = 0f;

            _previewTween = DOVirtual.Float(0f, 1f, 1f, value =>
            {
                _previewProgress = value;
                Repaint();
            }).SetEase(_generatedCurve).OnComplete(() =>
            {
                _previewProgress = 1f;
                Repaint();
            });
        }

        #endregion

        #region Export

        private void DrawExportButtons()
        {
            EditorGUILayout.LabelField("导出", EditorStyles.miniBoldLabel);

            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginDisabledGroup(_generatedCurve == null);

            if (GUILayout.Button("保存为 Preset 资产"))
            {
                SaveAsPreset();
            }

            if (GUILayout.Button("应用到选中的 DOTweenAnimation"))
            {
                ApplyToSelectedAnimation();
            }

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndHorizontal();
        }

        private void SaveAsPreset()
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "保存曲线预设",
                "NewGestureCurve",
                "asset",
                "选择保存位置",
                "Assets/GestureTween/Presets"
            );

            if (string.IsNullOrEmpty(path)) return;

            var preset = CreateInstance<GestureCurvePreset>();
            preset.easeCurve = new AnimationCurve(_generatedCurve.keys);
            preset.recommendedDuration = 1f;
            preset.description = $"手绘生成于 {System.DateTime.Now:yyyy-MM-dd HH:mm}";

            AssetDatabase.CreateAsset(preset, path);
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = preset;

            Debug.Log($"[GestureTween] 曲线预设已保存: {path}");
        }

        private void ApplyToSelectedAnimation()
        {
            var selected = Selection.activeGameObject;
            if (selected == null)
            {
                EditorUtility.DisplayDialog("GestureTween", "请先选中一个包含 DOTweenAnimation 的 GameObject", "确定");
                return;
            }

            var anim = selected.GetComponent<DOTweenAnimation>();
            if (anim == null)
            {
                EditorUtility.DisplayDialog("GestureTween", "选中的 GameObject 没有 DOTweenAnimation 组件", "确定");
                return;
            }

            Undo.RecordObject(anim, "Apply Gesture Curve");
            anim.easeType = Ease.INTERNAL_Custom;
            anim.easeCurve = new AnimationCurve(_generatedCurve.keys);
            EditorUtility.SetDirty(anim);

            Debug.Log($"[GestureTween] 曲线已应用到: {selected.name}");
        }

        #endregion

        private void OnDisable()
        {
            _previewTween?.Kill();
        }
    }
}
