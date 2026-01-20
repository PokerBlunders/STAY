using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GPS : MonoBehaviour
{
    // 公开的玩家位置属性
    public Vector3 PlayerPosition => transform.position;
    public Vector2 PlayerPosition2D => transform.position;

    // 公开方法：获取玩家位置
    public Vector3 GetPlayerPosition()
    {
        return transform.position;
    }

    // 公开方法：获取玩家位置的2D版本
    public Vector2 GetPlayerPosition2D()
    {
        return transform.position;
    }

    [Header("按键设置")]
    [SerializeField] private KeyCode activationKey = KeyCode.T;
    [SerializeField] private bool toggleMode = false; // true=切换模式, false=按住模式

    [Header("目标点设置")]
    [SerializeField] private List<Transform> targets = new List<Transform>();
    [SerializeField] private bool showTargetIcons = true;
    [SerializeField] private Sprite targetIconSprite;
    [SerializeField] private float targetIconSize = 0.5f;

    [Header("路径设置")]
    [SerializeField] private float pathUpdateInterval = 0.05f;
    [SerializeField] private float pathWidth = 0.1f;
    [SerializeField] private int pathSmoothness = 5; // 路径平滑度
    [SerializeField] private float maxPathDistance = 100f; // 最大显示距离

    [Header("路径颜色")]
    [SerializeField]
    private Color[] pathColors = new Color[]
    {
        new Color(1f, 0.3f, 0.3f, 0.8f), // 红色
        new Color(0.3f, 0.8f, 0.3f, 0.8f), // 绿色
        new Color(0.3f, 0.5f, 1f, 0.8f), // 蓝色
        new Color(1f, 0.8f, 0.2f, 0.8f)  // 黄色
    };

    [Header("路径效果")]
    [SerializeField] private bool useParticles = true;
    [SerializeField] private GameObject pathParticlePrefab;
    [SerializeField] private float particleDensity = 0.5f; // 粒子密度
    [SerializeField] private float particleSize = 0.3f;
    [SerializeField] private float particleSpeed = 2f;

    [Header("UI指示器")]
    [SerializeField] private bool showDistance = true;
    [SerializeField] private GameObject distanceUIPrefab;
    [SerializeField] private Vector2 uiOffset = new Vector2(0, 0.5f);

    // 私有变量
    private List<LineRenderer> pathRenderers = new List<LineRenderer>();
    private List<GameObject> targetIcons = new List<GameObject>();
    private List<GameObject> distanceIndicators = new List<GameObject>();
    private List<TextMesh> distanceTexts = new List<TextMesh>();
    private List<List<ParticleSystem>> activeParticles = new List<List<ParticleSystem>>();
    private bool isActive = false;
    private float lastUpdateTime = 0f;
    private Transform canvasTransform;

    void Start()
    {
        // 初始化所有路径
        InitializePaths();

        // 创建或查找Canvas
        SetupCanvas();

        // 初始化UI指示器
        if (showDistance && distanceUIPrefab != null)
        {
            InitializeDistanceIndicators();
        }
    }

    void Update()
    {
        // 处理按键输入
        HandleInput();

        // 更新路径显示
        if (isActive && Time.time - lastUpdateTime > pathUpdateInterval)
        {
            UpdateAllPaths();
            lastUpdateTime = Time.time;
        }

        // 更新UI指示器位置
        UpdateDistanceIndicators();
    }

    void HandleInput()
    {
        if (toggleMode)
        {
            if (Input.GetKeyDown(activationKey))
            {
                isActive = !isActive;
                SetPathsActive(isActive);
            }
        }
        else
        {
            if (Input.GetKeyDown(activationKey))
            {
                isActive = true;
                SetPathsActive(true);
            }
            else if (Input.GetKeyUp(activationKey))
            {
                isActive = false;
                SetPathsActive(false);
            }
        }
    }

    void InitializePaths()
    {
        // 清理旧数据
        foreach (var renderer in pathRenderers)
        {
            if (renderer != null) Destroy(renderer.gameObject);
        }
        pathRenderers.Clear();
        activeParticles.Clear();

        // 为每个目标创建路径
        for (int i = 0; i < Mathf.Min(targets.Count, 4); i++) // 最多4个目标
        {
            if (targets[i] == null) continue;

            // 创建路径GameObject
            GameObject pathObj = new GameObject($"Path_{i}_{targets[i].name}");
            pathObj.transform.SetParent(transform);
            pathObj.transform.localPosition = Vector3.zero;

            // 添加LineRenderer
            LineRenderer lineRenderer = pathObj.AddComponent<LineRenderer>();

            // 设置LineRenderer属性
            lineRenderer.startWidth = pathWidth;
            lineRenderer.endWidth = pathWidth;
            lineRenderer.positionCount = 2; // 初始设置为2个点

            // 设置材质和颜色
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = pathColors[i % pathColors.Length];
            lineRenderer.endColor = pathColors[i % pathColors.Length];

            // 设置排序图层（确保在2D场景中正确显示）
            lineRenderer.sortingLayerName = "UI"; // 或您项目中合适的图层
            lineRenderer.sortingOrder = 10;

            // 添加到列表
            pathRenderers.Add(lineRenderer);
            activeParticles.Add(new List<ParticleSystem>());

            // 初始隐藏
            lineRenderer.enabled = false;

            // 创建目标图标
            if (showTargetIcons)
            {
                CreateTargetIcon(i);
            }
        }
    }

    void CreateTargetIcon(int targetIndex)
    {
        if (targets[targetIndex] == null) return;

        GameObject iconObj = new GameObject($"TargetIcon_{targetIndex}");
        iconObj.transform.SetParent(targets[targetIndex]);
        iconObj.transform.localPosition = Vector3.zero + Vector3.back * 0.1f; // 稍微向前

        // 添加SpriteRenderer
        SpriteRenderer spriteRenderer = iconObj.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = targetIconSprite != null ? targetIconSprite : CreateDefaultIcon();
        spriteRenderer.color = pathColors[targetIndex % pathColors.Length];
        spriteRenderer.sortingLayerName = "UI";
        spriteRenderer.sortingOrder = 5;
        spriteRenderer.transform.localScale = Vector3.one * targetIconSize;

        targetIcons.Add(iconObj);

        // 添加脉冲动画
        StartCoroutine(PulseIcon(iconObj.transform, targetIndex));
    }

    IEnumerator PulseIcon(Transform icon, int colorIndex)
    {
        Vector3 originalScale = icon.localScale;
        Color originalColor = icon.GetComponent<SpriteRenderer>().color;

        while (true)
        {
            if (icon == null) yield break;

            float pulseTime = 1.5f;
            float elapsedTime = 0f;

            while (elapsedTime < pulseTime)
            {
                if (icon == null) yield break;

                float t = elapsedTime / pulseTime;
                float scale = Mathf.Lerp(0.8f, 1.2f, Mathf.PingPong(t * 2, 1));
                icon.localScale = originalScale * scale;

                Color pulseColor = Color.Lerp(originalColor, Color.white, Mathf.PingPong(t, 0.5f));
                icon.GetComponent<SpriteRenderer>().color = pulseColor;

                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }

    void SetupCanvas()
    {
        // 查找或创建Canvas
        GameObject canvasObj = GameObject.Find("GPS_Canvas");
        if (canvasObj == null)
        {
            canvasObj = new GameObject("GPS_Canvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
        }
        canvasTransform = canvasObj.transform;
    }

    void InitializeDistanceIndicators()
    {
        for (int i = 0; i < Mathf.Min(targets.Count, 4); i++)
        {
            if (targets[i] == null) continue;

            GameObject indicatorObj;
            if (distanceUIPrefab != null)
            {
                indicatorObj = Instantiate(distanceUIPrefab, canvasTransform);
            }
            else
            {
                indicatorObj = CreateDefaultIndicator();
            }

            indicatorObj.name = $"DistanceIndicator_{i}";
            distanceIndicators.Add(indicatorObj);

            // 获取或添加TextMesh组件
            TextMesh textMesh = indicatorObj.GetComponent<TextMesh>();
            if (textMesh == null)
            {
                textMesh = indicatorObj.AddComponent<TextMesh>();
                textMesh.fontSize = 20;
                textMesh.anchor = TextAnchor.MiddleCenter;
                textMesh.alignment = TextAlignment.Center;
            }

            textMesh.color = pathColors[i % pathColors.Length];
            distanceTexts.Add(textMesh);

            // 初始隐藏
            indicatorObj.SetActive(false);
        }
    }

    GameObject CreateDefaultIndicator()
    {
        GameObject obj = new GameObject("DistanceIndicator");
        obj.transform.SetParent(canvasTransform);

        TextMesh textMesh = obj.AddComponent<TextMesh>();
        textMesh.text = "0m";
        textMesh.fontSize = 20;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;

        return obj;
    }

    void UpdateDistanceIndicators()
    {
        if (!showDistance || !isActive) return;

        for (int i = 0; i < distanceIndicators.Count; i++)
        {
            if (i >= targets.Count || targets[i] == null) continue;

            // 更新位置：世界坐标转换为屏幕坐标
            Vector3 screenPos = Camera.main.WorldToScreenPoint(targets[i].position + (Vector3)uiOffset);
            distanceIndicators[i].transform.position = screenPos;

            // 更新距离文本
            float distance = Vector2.Distance(transform.position, targets[i].position);
            distanceTexts[i].text = $"{distance:F1}m";

            // 根据距离改变颜色
            Color color = pathColors[i % pathColors.Length];
            if (distance < 3f) color = Color.green;
            else if (distance < 10f) color = Color.yellow;
            distanceTexts[i].color = color;

            // 显示/隐藏
            distanceIndicators[i].SetActive(isActive && distance > 1f);
        }
    }

    void SetPathsActive(bool active)
    {
        foreach (LineRenderer renderer in pathRenderers)
        {
            if (renderer != null)
            {
                renderer.enabled = active;
            }
        }

        // 播放声音效果
        if (active)
        {
            PlayActivationSound();
        }

        // 激活/停用粒子
        if (useParticles)
        {
            if (active)
            {
                StartCoroutine(SpawnParticles());
            }
            else
            {
                StopAllCoroutines();
                ClearParticles();
            }
        }

        // 显示/隐藏距离指示器
        if (showDistance)
        {
            foreach (var indicator in distanceIndicators)
            {
                if (indicator != null)
                {
                    indicator.SetActive(active);
                }
            }
        }
    }

    void UpdateAllPaths()
    {
        for (int i = 0; i < pathRenderers.Count; i++)
        {
            if (i < targets.Count && targets[i] != null)
            {
                UpdatePath(i);
            }
        }
    }

    void UpdatePath(int pathIndex)
    {
        if (pathIndex >= targets.Count || targets[pathIndex] == null)
            return;

        Vector2 startPos = transform.position;
        Vector2 targetPos = targets[pathIndex].position;

        // 计算距离
        float distance = Vector2.Distance(startPos, targetPos);

        // 如果距离太远，不显示路径
        if (distance > maxPathDistance)
        {
            pathRenderers[pathIndex].enabled = false;
            return;
        }

        pathRenderers[pathIndex].enabled = true;

        // 生成路径点
        List<Vector3> pathPoints = GeneratePathPoints(startPos, targetPos, distance);

        // 更新LineRenderer
        LineRenderer lineRenderer = pathRenderers[pathIndex];
        lineRenderer.positionCount = pathPoints.Count;
        lineRenderer.SetPositions(pathPoints.ToArray());

        // 设置颜色渐变
        Gradient gradient = new Gradient();
        Color pathColor = pathColors[pathIndex % pathColors.Length];
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(pathColor, 0.0f),
                new GradientColorKey(pathColor, 1.0f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(0.9f, 0.0f),
                new GradientAlphaKey(0.3f, 1.0f)
            }
        );
        lineRenderer.colorGradient = gradient;
    }

    List<Vector3> GeneratePathPoints(Vector2 start, Vector2 end, float distance)
    {
        List<Vector3> points = new List<Vector3>();

        // 简单直线路径
        if (pathSmoothness <= 0)
        {
            points.Add(start);
            points.Add(end);
            return points;
        }

        // 带轻微曲线的路径（适合2D横版）
        int segmentCount = Mathf.Max(2, Mathf.CeilToInt(distance * pathSmoothness));

        for (int i = 0; i <= segmentCount; i++)
        {
            float t = i / (float)segmentCount;

            // 线性插值
            Vector2 point = Vector2.Lerp(start, end, t);

            // 添加轻微的曲线（正弦波）
            if (distance > 1f)
            {
                float curveAmount = Mathf.Sin(t * Mathf.PI) * 0.2f;
                point += Vector2.up * curveAmount;
            }

            // 确保z坐标一致（2D游戏）
            points.Add(new Vector3(point.x, point.y, 0));
        }

        return points;
    }

    IEnumerator SpawnParticles()
    {
        while (isActive && useParticles)
        {
            for (int i = 0; i < pathRenderers.Count; i++)
            {
                if (i >= targets.Count || targets[i] == null || !pathRenderers[i].enabled)
                    continue;

                SpawnParticleAlongPath(i);
            }

            yield return new WaitForSeconds(0.1f / particleDensity);
        }
    }

    void SpawnParticleAlongPath(int pathIndex)
    {
        if (pathRenderers[pathIndex].positionCount < 2) return;

        // 在路径上随机选择一个位置
        int segment = Random.Range(0, pathRenderers[pathIndex].positionCount - 1);
        Vector3 spawnPos = pathRenderers[pathIndex].GetPosition(segment);

        GameObject particleObj;
        if (pathParticlePrefab != null)
        {
            particleObj = Instantiate(pathParticlePrefab, spawnPos, Quaternion.identity);
        }
        else
        {
            particleObj = CreateDefaultParticle();
            particleObj.transform.position = spawnPos;
        }

        // 设置粒子属性
        ParticleSystem particleSystem = particleObj.GetComponent<ParticleSystem>();
        if (particleSystem == null) return;

        var main = particleSystem.main;
        main.startColor = pathColors[pathIndex % pathColors.Length];
        main.startSize = particleSize;
        main.startLifetime = 1f;

        // 设置移动方向（朝向目标）
        Vector3 direction = (targets[pathIndex].position - spawnPos).normalized;
        var velocity = particleSystem.velocityOverLifetime;
        velocity.enabled = true;
        velocity.space = ParticleSystemSimulationSpace.World;
        velocity.x = direction.x * particleSpeed;
        velocity.y = direction.y * particleSpeed;

        // 添加到列表并设置自动销毁
        activeParticles[pathIndex].Add(particleSystem);
        StartCoroutine(DestroyParticleAfterDelay(particleObj, 2f, pathIndex));
    }

    GameObject CreateDefaultParticle()
    {
        GameObject particle = new GameObject("PathParticle");

        // 添加SpriteRenderer
        SpriteRenderer sr = particle.AddComponent<SpriteRenderer>();
        sr.sprite = CreateCircleSprite(16, pathColors[0]);
        sr.sortingLayerName = "UI";
        sr.sortingOrder = 15;

        // 添加简单粒子效果
        ParticleSystem ps = particle.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.startSize = particleSize;
        main.startLifetime = 1f;
        main.maxParticles = 1;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.enabled = false;

        return particle;
    }

    IEnumerator DestroyParticleAfterDelay(GameObject particle, float delay, int pathIndex)
    {
        yield return new WaitForSeconds(delay);

        if (particle != null)
        {
            // 淡出效果
            SpriteRenderer sr = particle.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                float fadeTime = 0.5f;
                float elapsed = 0f;
                Color originalColor = sr.color;

                while (elapsed < fadeTime && sr != null)
                {
                    elapsed += Time.deltaTime;
                    sr.color = Color.Lerp(originalColor, Color.clear, elapsed / fadeTime);
                    yield return null;
                }
            }

            Destroy(particle);

            // 从列表中移除
            if (pathIndex < activeParticles.Count)
            {
                ParticleSystem ps = particle.GetComponent<ParticleSystem>();
                if (ps != null) activeParticles[pathIndex].Remove(ps);
            }
        }
    }

    void ClearParticles()
    {
        foreach (var particleList in activeParticles)
        {
            foreach (var particle in particleList)
            {
                if (particle != null) Destroy(particle.gameObject);
            }
            particleList.Clear();
        }
    }

    void PlayActivationSound()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.volume = 0.5f;
        }

        // 可以在这里添加声音播放逻辑
        // audioSource.PlayOneShot(yourSoundClip);
    }

    Sprite CreateDefaultIcon()
    {
        Texture2D tex = new Texture2D(64, 64);
        Vector2 center = new Vector2(32, 32);

        for (int y = 0; y < 64; y++)
        {
            for (int x = 0; x < 64; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);

                if (distance < 30 && distance > 25)
                {
                    float alpha = Mathf.Clamp01(1 - Mathf.Abs(distance - 27.5f) / 2.5f);
                    tex.SetPixel(x, y, new Color(1, 1, 1, alpha));
                }
                else if (distance <= 10)
                {
                    tex.SetPixel(x, y, Color.white);
                }
                else
                {
                    tex.SetPixel(x, y, Color.clear);
                }
            }
        }

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));
    }

    Sprite CreateCircleSprite(int resolution, Color color)
    {
        Texture2D tex = new Texture2D(resolution, resolution);
        Vector2 center = new Vector2(resolution / 2, resolution / 2);
        float radius = resolution / 2 - 2;

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);

                if (distance <= radius)
                {
                    float alpha = Mathf.Clamp01(1 - distance / radius);
                    alpha = Mathf.Pow(alpha, 2); // 让边缘更柔和
                    tex.SetPixel(x, y, new Color(color.r, color.g, color.b, alpha));
                }
                else
                {
                    tex.SetPixel(x, y, Color.clear);
                }
            }
        }

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, resolution, resolution), new Vector2(0.5f, 0.5f));
    }

    // 公开方法：动态添加目标
    public void AddTarget(Transform newTarget)
    {
        if (targets.Count >= 4)
        {
            Debug.LogWarning("已达到最大目标数量(4)");
            return;
        }

        targets.Add(newTarget);
        InitializePaths(); // 重新初始化所有路径

        if (showDistance)
        {
            InitializeDistanceIndicators();
        }
    }

    // 公开方法：移除目标
    public void RemoveTarget(Transform targetToRemove)
    {
        int index = targets.IndexOf(targetToRemove);
        if (index >= 0)
        {
            targets.RemoveAt(index);

            // 清理对应的路径
            if (index < pathRenderers.Count && pathRenderers[index] != null)
            {
                Destroy(pathRenderers[index].gameObject);
            }

            // 重新初始化
            InitializePaths();
        }
    }

    // 公开方法：清除所有目标
    public void ClearAllTargets()
    {
        targets.Clear();

        foreach (var renderer in pathRenderers)
        {
            if (renderer != null) Destroy(renderer.gameObject);
        }
        pathRenderers.Clear();

        foreach (var icon in targetIcons)
        {
            if (icon != null) Destroy(icon);
        }
        targetIcons.Clear();
    }

    // 编辑器辅助
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (targets != null)
        {
            for (int i = 0; i < Mathf.Min(targets.Count, 4); i++)
            {
                if (targets[i] != null)
                {
                    Gizmos.color = pathColors[i % pathColors.Length];
                    Gizmos.DrawWireSphere(targets[i].position, 0.3f);
                    Gizmos.DrawLine(transform.position, targets[i].position);
                }
            }
        }
    }
#endif
}