using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

// Aula Aventura RA - Modulo 2 : Auto-Setup
// Singleton: si hay dos instancias en la escena, la segunda se destruye sola.
public class Setup_M2 : MonoBehaviour
{
    [Header("Ajuste de escala AR")]
    public float ballScale = 0.08f;

    // --- Singleton ---------------------------------------------------------
    static Setup_M2 _instance;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        if (ScoreManager.Instance == null)
            new GameObject("ScoreManager").AddComponent<ScoreManager>();
    }

    void OnDestroy()
    {
        if (_instance == this) _instance = null;
    }

    void Start()
    {
        // Solo la instancia valida ejecuta el setup
        if (_instance != this) return;

        Transform parent = GetARParent();
        CreateBalls(parent);
        CreateSlots(parent);
        CreateConfetti(parent);
        WireUI(FindOrCreateGameManager());
    }

    // --- PARENT --------------------------------------------------------------

    Transform GetARParent()
    {
        GameObject it = GameObject.Find("ImageTarget");
        if (it != null) return it.transform;
        return new GameObject("Modulo2_Root").transform;
    }

    // --- PELOTAS ---------------------------------------------------------------
    // Euler(-90,0,0) = la cara del Quad apunta en +Y del ImageTarget = hacia la camara.
    // Posicion: y = 0.03f (delante del target), z = +0.05f (fila superior)

    void CreateBalls(Transform parent)
    {
        foreach (NumberBall old in FindObjectsByType<NumberBall>(FindObjectsSortMode.None))
            Destroy(old.gameObject);

        int[] orden = Shuffle(new int[] { 1, 2, 3, 4, 5 });

        for (int i = 0; i < 5; i++)
        {
            int num = orden[i];

            GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Quad);
            ball.name = "Ball_" + num;
            ball.transform.SetParent(parent, false);
            ball.transform.localScale = Vector3.one * ballScale;

            float x = -0.16f + i * 0.08f;
            // y=0.03 => delante del target (hacia camara en espacio Vuforia)
            // z=0.05 => fila superior (arriba del centro del target)
            ball.transform.localPosition = new Vector3(x, 0.03f, 0.05f);
            ball.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);

            Texture2D tex = Resources.Load<Texture2D>("Assets_Modulo2/num_" + num);
            Renderer rend = ball.GetComponent<Renderer>();
            if (tex != null)
            {
                Material mat = new Material(Shader.Find("Unlit/Transparent"));
                if (mat.shader == null || !mat.shader.isSupported)
                    mat = new Material(Shader.Find("Unlit/Texture"));
                mat.mainTexture = tex;
                rend.material = mat;
            }
            else
            {
                Material mat = new Material(Shader.Find("Unlit/Color"));
                if (mat.shader == null || !mat.shader.isSupported)
                    mat = rend.material;
                mat.color = new Color(0.2f, 0.8f, 0.2f, 1f);
                rend.material = mat;
            }

            // Numero en el slot: imagen num_X.png semi-transparente
            Texture2D numTex = Resources.Load<Texture2D>("Assets_Modulo2/num_" + num);
            if (numTex != null)
            {
                GameObject numOverlay = GameObject.CreatePrimitive(PrimitiveType.Quad);
                numOverlay.name = "SlotNum_" + num;
                numOverlay.transform.SetParent(slot.transform, false);
                numOverlay.transform.localPosition = new Vector3(0f, 0f, 0.001f);
                numOverlay.transform.localRotation = Quaternion.identity;
                numOverlay.transform.localScale = Vector3.one * 0.75f;
                Material numMat = new Material(Shader.Find("Unlit/Transparent"));
                if (numMat.shader == null || !numMat.shader.isSupported)
                    numMat = new Material(Shader.Find("Unlit/Texture"));
                numMat.mainTexture = numTex;
                numOverlay.GetComponent<Renderer>().material = numMat;
                Collider nc = numOverlay.GetComponent<Collider>();
                if (nc != null) Destroy(nc);
            }

            NumberBall nb = ball.AddComponent<NumberBall>();
            nb.ballNumber = num;
        }
    }

    // --- SLOTS -----------------------------------------------------------------

    void CreateSlots(Transform parent)
    {
        foreach (SlotTarget old in FindObjectsByType<SlotTarget>(FindObjectsSortMode.None))
            Destroy(old.gameObject);

        Texture2D slotTex = Resources.Load<Texture2D>("Assets_Modulo2/slot_vacio");

        for (int i = 0; i < 5; i++)
        {
            int num = i + 1;

            GameObject slot = GameObject.CreatePrimitive(PrimitiveType.Quad);
            slot.name = "Slot_" + num;
            slot.transform.SetParent(parent, false);
            slot.transform.localScale = Vector3.one * ballScale * 1.1f;

            float x = -0.16f + i * 0.08f;
            // z=-0.05 => fila inferior (debajo del centro del target)
            slot.transform.localPosition = new Vector3(x, 0.03f, -0.05f);
            slot.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);

            Renderer rend = slot.GetComponent<Renderer>();
            if (slotTex != null)
            {
                Material mat = new Material(Shader.Find("Unlit/Transparent"));
                if (mat.shader == null || !mat.shader.isSupported)
                    mat = new Material(Shader.Find("Unlit/Texture"));
                mat.mainTexture = slotTex;
                rend.material = mat;
            }
            else
            {
                Material mat = new Material(Shader.Find("Unlit/Color"));
                if (mat.shader == null || !mat.shader.isSupported)
                    mat = rend.material;
                mat.color = new Color(0.2f, 0.8f, 0.2f, 1f);
                rend.material = mat;
            }

            // Numero en el slot: imagen num_X.png semi-transparente
            Texture2D numTex = Resources.Load<Texture2D>("Assets_Modulo2/num_" + num);
            if (numTex != null)
            {
                GameObject numOverlay = GameObject.CreatePrimitive(PrimitiveType.Quad);
                numOverlay.name = "SlotNum_" + num;
                numOverlay.transform.SetParent(slot.transform, false);
                numOverlay.transform.localPosition = new Vector3(0f, 0f, 0.001f);
                numOverlay.transform.localRotation = Quaternion.identity;
                numOverlay.transform.localScale = Vector3.one * 0.75f;
                Material numMat = new Material(Shader.Find("Unlit/Transparent"));
                if (numMat.shader == null || !numMat.shader.isSupported)
                    numMat = new Material(Shader.Find("Unlit/Texture"));
                numMat.mainTexture = numTex;
                numOverlay.GetComponent<Renderer>().material = numMat;
                Collider nc = numOverlay.GetComponent<Collider>();
                if (nc != null) Destroy(nc);
            }

            SlotTarget st = slot.AddComponent<SlotTarget>();
            st.slotNumber = num;
        }
    }

    // --- CONFETTI ----------------------------------------------------------------

    void CreateConfetti(Transform parent)
    {
        // Destruye confetti viejo si existe
        Transform old = parent.Find("Confetti");
        if (old != null) Destroy(old.gameObject);

        GameObject go = new GameObject("Confetti");
        go.transform.SetParent(parent, false);
        go.transform.localPosition = new Vector3(0, 0.1f, 0);

        ParticleSystem ps = go.AddComponent<ParticleSystem>();
        go.SetActive(false);

        var main = ps.main;
        main.startLifetime = 2.5f;
        main.startSpeed = 0.8f;
        main.startSize = 0.015f;
        main.maxParticles = 200;
        main.loop = false;
        main.playOnAwake = false;
        main.startColor = new ParticleSystem.MinMaxGradient(Color.red, Color.cyan);

        var em = ps.emission;
        em.rateOverTime = 0;
        em.SetBursts(new[] { new ParticleSystem.Burst(0f, 200) });

        GameManager_M2 gm = FindOrCreateGameManager();
        gm.confetti = ps;
        gm.happyFaceGO = go;
    }

    // --- GAME MANAGER --------------------------------------------------------

    GameManager_M2 FindOrCreateGameManager()
    {
        GameManager_M2 gm = FindFirstObjectByType<GameManager_M2>();
        if (gm != null) return gm;
        return new GameObject("GameManager_M2").AddComponent<GameManager_M2>();
    }

    // --- UI: limpia canvas viejo y crea UI limpia de M2 -----------------------

    void WireUI(GameManager_M2 gm)
    {
        // Busca o crea Canvas
        Canvas cv = FindFirstObjectByType<Canvas>();
        if (cv == null)
        {
            GameObject cGO = new GameObject("Canvas_M2");
            cv = cGO.AddComponent<Canvas>();
            cv.renderMode = RenderMode.ScreenSpaceOverlay;
            cGO.AddComponent<CanvasScaler>();
            cGO.AddComponent<GraphicRaycaster>();
        }
        cv.renderMode = RenderMode.ScreenSpaceOverlay;

        // LIMPIAR todo el canvas (elimina UI leftover de M1)
        for (int i = cv.transform.childCount - 1; i >= 0; i--)
            Destroy(cv.transform.GetChild(i).gameObject);

        Transform c = cv.transform;

        // UI de M2
        gm.progressText = MakeText(c, "ProgressText", "Colocados: 0 / 5",
            new Vector2(0, -40), new Vector2(260, 50), TextAnchor.MiddleRight, 24, Color.white,
            new Vector2(1, 1), new Vector2(1, 1));

        gm.instructionText = MakeText(c, "Instruccion", "Apunta al target. Arrastra las pelotas en orden 1 -> 5",
            new Vector2(0, -40), new Vector2(500, 50), TextAnchor.MiddleCenter, 22, Color.white,
            new Vector2(.5f, 1), new Vector2(.5f, 1));

        // Panel carita feliz
        GameObject hfPanel = MakePanel(c, "HappyFacePanel", Vector2.zero, new Vector2(320, 320), new Color(0, 0, 0, 0.5f));
        hfPanel.SetActive(false);
        gm.happyFacePanel = hfPanel;

        Sprite happySprite = LoadSprite("Assets_Modulo2/carita_feliz");
        if (happySprite != null)
        {
            GameObject imgGO = new GameObject("CaritaImg");
            imgGO.transform.SetParent(hfPanel.transform, false);
            Image img = imgGO.AddComponent<Image>();
            img.sprite = happySprite;
            img.preserveAspect = true;
            RectTransform rt = imgGO.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero; rt.anchoredPosition = Vector2.zero;
        }

        // Win Panel
        GameObject winPanel = MakePanel(c, "WinPanel", Vector2.zero, new Vector2(500, 420), new Color(0.05f, 0.05f, 0.2f, 0.95f));
        winPanel.SetActive(false);
        gm.winPanel = winPanel;
        Transform wp = winPanel.transform;

        Sprite trophySprite = LoadSprite("Assets_Modulo2/trophy");
        if (trophySprite != null) AddCornerImage(wp, trophySprite, new Vector2(190, 160), new Vector2(70, 70));

        gm.winMessage = MakeText(wp, "WinMessage", "¡Excelente!",
            new Vector2(0, 145), new Vector2(420, 60), TextAnchor.MiddleCenter, 38, Color.yellow,
            new Vector2(.5f, .5f), new Vector2(.5f, .5f));

        Sprite starSprite = LoadSprite("Assets_Modulo2/star");
        if (starSprite != null) AddStarsRow(wp, starSprite, new Vector2(0, 80));

        gm.scoreM1Text = MakeText(wp, "ScoreM1", "Modulo 1: 0 pts",
            new Vector2(0, 20), new Vector2(420, 50), TextAnchor.MiddleCenter, 26, Color.white,
            new Vector2(.5f, .5f), new Vector2(.5f, .5f));

        gm.scoreM2Text = MakeText(wp, "ScoreM2", "Modulo 2: 0 pts",
            new Vector2(0, -35), new Vector2(420, 50), TextAnchor.MiddleCenter, 26, Color.white,
            new Vector2(.5f, .5f), new Vector2(.5f, .5f));

        gm.scoreTotalText = MakeText(wp, "ScoreTotal", "TOTAL: 0 pts",
            new Vector2(0, -95), new Vector2(420, 60), TextAnchor.MiddleCenter, 32, Color.cyan,
            new Vector2(.5f, .5f), new Vector2(.5f, .5f));

        MakeButton(wp, "BtnRestart", "Jugar de Nuevo",
            new Vector2(-70, -165), new Vector2(200, 55), new Color(0.1f, 0.6f, 0.1f),
            gm.RestartGame);

        MakeButton(wp, "BtnM1", "Ir a Modulo 1",
            new Vector2(100, -165), new Vector2(180, 55), new Color(0.1f, 0.3f, 0.7f),
            () => SceneManager.LoadScene(0));
    }

    // --- HELPERS -----------------------------------------------------------------

    Sprite LoadSprite(string path)
    {
        Texture2D tex = Resources.Load<Texture2D>(path);
        if (tex == null) return null;
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    }

    void AddCornerImage(Transform parent, Sprite sprite, Vector2 pos, Vector2 size)
    {
        GameObject go = new GameObject("TrophyIcon");
        go.transform.SetParent(parent, false);
        Image img = go.AddComponent<Image>();
        img.sprite = sprite; img.preserveAspect = true;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos; rt.sizeDelta = size;
    }

    void AddStarsRow(Transform parent, Sprite starSprite, Vector2 centerPos)
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject go = new GameObject("Star_" + i);
            go.transform.SetParent(parent, false);
            Image img = go.AddComponent<Image>();
            img.sprite = starSprite; img.preserveAspect = true;
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(45, 45);
            rt.anchoredPosition = new Vector2(centerPos.x + (i - 1) * 55f, centerPos.y);
        }
    }

    Text MakeText(Transform parent, string name, string content,
        Vector2 pos, Vector2 size, TextAnchor anchor, int fs, Color color,
        Vector2 aMin, Vector2 aMax)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = aMin; rt.anchorMax = aMax; rt.sizeDelta = size;
        rt.anchoredPosition = pos;
        Text t = go.AddComponent<Text>();
        t.text = content;
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.fontSize = fs; t.fontStyle = FontStyle.Bold;
        t.color = color; t.alignment = anchor;
        Shadow sh = go.AddComponent<Shadow>();
        sh.effectColor = new Color(0, 0, 0, 0.7f);
        sh.effectDistance = new Vector2(1.5f, -1.5f);
        return t;
    }

    GameObject MakePanel(Transform parent, string name, Vector2 pos, Vector2 size, Color bg)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos; rt.sizeDelta = size;
        go.AddComponent<Image>().color = bg;
        return go;
    }

    void MakeButton(Transform parent, string name, string label,
        Vector2 pos, Vector2 size, Color bg, UnityEngine.Events.UnityAction onClick)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos; rt.sizeDelta = size;
        go.AddComponent<Image>().color = bg;
        go.AddComponent<Button>().onClick.AddListener(onClick);
        GameObject tGO = new GameObject("Text");
        tGO.transform.SetParent(go.transform, false);
        Text t = tGO.AddComponent<Text>();
        t.text = label;
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.fontSize = 22; t.fontStyle = FontStyle.Bold;
        t.color = Color.white; t.alignment = TextAnchor.MiddleCenter;
        RectTransform trt = tGO.GetComponent<RectTransform>();
        trt.anchorMin = Vector2.zero; trt.anchorMax = Vector2.one;
        trt.sizeDelta = Vector2.zero; trt.anchoredPosition = Vector2.zero;
    }

    int[] Shuffle(int[] arr)
    {
        int[] r = (int[])arr.Clone();
        for (int i = r.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int tmp = r[i]; r[i] = r[j]; r[j] = tmp;
        }
        return r;
    }
}
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

// Aula Aventura RA - Modulo 2 : Auto-Setup
// Singleton: si hay dos instancias en la escena, la segunda se destruye sola.
public class Setup_M2 : MonoBehaviour
{
    [Header("Ajuste de escala AR")]
    public float ballScale = 0.08f;

    // --- Singleton ---------------------------------------------------------
    static Setup_M2 _instance;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        if (ScoreManager.Instance == null)
            new GameObject("ScoreManager").AddComponent<ScoreManager>();
    }

    void OnDestroy()
    {
        if (_instance == this) _instance = null;
    }

    void Start()
    {
        // Solo la instancia valida ejecuta el setup
        if (_instance != this) return;

        Transform parent = GetARParent();
        CreateBalls(parent);
        CreateSlots(parent);
        CreateConfetti(parent);
        WireUI(FindOrCreateGameManager());
    }

    // --- PARENT --------------------------------------------------------------

    Transform GetARParent()
    {
        GameObject it = GameObject.Find("ImageTarget");
        if (it != null) return it.transform;
        return new GameObject("Modulo2_Root").transform;
    }

    // --- PELOTAS ---------------------------------------------------------------
    // Euler(-90,0,0) = la cara del Quad apunta en +Y del ImageTarget = hacia la camara.
    // Posicion: y = 0.03f (delante del target), z = +0.05f (fila superior)

    void CreateBalls(Transform parent)
    {
        foreach (NumberBall old in FindObjectsByType<NumberBall>(FindObjectsSortMode.None))
            Destroy(old.gameObject);

        int[] orden = Shuffle(new int[] { 1, 2, 3, 4, 5 });

        for (int i = 0; i < 5; i++)
        {
            int num = orden[i];

            GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Quad);
            ball.name = "Ball_" + num;
            ball.transform.SetParent(parent, false);
            ball.transform.localScale = Vector3.one * ballScale;

            float x = -0.16f + i * 0.08f;
            // y=0.03 => delante del target (hacia camara en espacio Vuforia)
            // z=0.05 => fila superior (arriba del centro del target)
            ball.transform.localPosition = new Vector3(x, 0.03f, 0.05f);
            ball.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);

            Texture2D tex = Resources.Load<Texture2D>("Assets_Modulo2/num_" + num);
            Renderer rend = ball.GetComponent<Renderer>();
            if (tex != null)
            {
                Material mat = new Material(Shader.Find("Unlit/Transparent"));
                if (mat.shader == null || !mat.shader.isSupported)
                    mat = new Material(Shader.Find("Unlit/Texture"));
                mat.mainTexture = tex;
                rend.material = mat;
            }
            else
            {
                Material mat = new Material(Shader.Find("Unlit/Color"));
                if (mat.shader == null || !mat.shader.isSupported)
                    mat = rend.material;
                mat.color = new Color(0.2f, 0.8f, 0.2f, 1f);
                rend.material = mat;
            }

            NumberBall nb = ball.AddComponent<NumberBall>();
            nb.ballNumber = num;
        }
    }

    // --- SLOTS -----------------------------------------------------------------

    void CreateSlots(Transform parent)
    {
        foreach (SlotTarget old in FindObjectsByType<SlotTarget>(FindObjectsSortMode.None))
            Destroy(old.gameObject);

        Texture2D slotTex = Resources.Load<Texture2D>("Assets_Modulo2/slot_vacio");

        for (int i = 0; i < 5; i++)
        {
            int num = i + 1;

            GameObject slot = GameObject.CreatePrimitive(PrimitiveType.Quad);
            slot.name = "Slot_" + num;
            slot.transform.SetParent(parent, false);
            slot.transform.localScale = Vector3.one * ballScale * 1.1f;

            float x = -0.16f + i * 0.08f;
            // z=-0.05 => fila inferior (debajo del centro del target)
            slot.transform.localPosition = new Vector3(x, 0.03f, -0.05f);
            slot.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);

            Renderer rend = slot.GetComponent<Renderer>();
            if (slotTex != null)
            {
                Material mat = new Material(Shader.Find("Unlit/Transparent"));
                if (mat.shader == null || !mat.shader.isSupported)
                    mat = new Material(Shader.Find("Unlit/Texture"));
                mat.mainTexture = slotTex;
                rend.material = mat;
            }
            else
            {
                Material mat = new Material(Shader.Find("Unlit/Color"));
                if (mat.shader == null || !mat.shader.isSupported)
                    mat = rend.material;
                mat.color = new Color(0.2f, 0.8f, 0.2f, 1f);
                rend.material = mat;
            }

            // Numero en el slot: imagen num_X.png semi-transparente
            Texture2D numTex = Resources.Load<Texture2D>("Assets_Modulo2/num_" + num);
            if (numTex != null)
            {
                GameObject numOverlay = GameObject.CreatePrimitive(PrimitiveType.Quad);
                numOverlay.name = "SlotNum_" + num;
                numOverlay.transform.SetParent(slot.transform, false);
                numOverlay.transform.localPosition = new Vector3(0f, 0f, 0.001f);
                numOverlay.transform.localRotation = Quaternion.identity;
                numOverlay.transform.localScale = Vector3.one * 0.75f;
                Material numMat = new Material(Shader.Find("Unlit/Transparent"));
                if (numMat.shader == null || !numMat.shader.isSupported)
                    numMat = new Material(Shader.Find("Unlit/Texture"));
                numMat.mainTexture = numTex;
                numOverlay.GetComponent<Renderer>().material = numMat;
                Collider nc = numOverlay.GetComponent<Collider>();
                if (nc != null) Destroy(nc);
            }

            SlotTarget st = slot.AddComponent<SlotTarget>();
            st.slotNumber = num;
        }
    }

    // --- CONFETTI ----------------------------------------------------------------

    void CreateConfetti(Transform parent)
    {
        // Destruye confetti viejo si existe
        Transform old = parent.Find("Confetti");
        if (old != null) Destroy(old.gameObject);

        GameObject go = new GameObject("Confetti");
        go.transform.SetParent(parent, false);
        go.transform.localPosition = new Vector3(0, 0.1f, 0);

        ParticleSystem ps = go.AddComponent<ParticleSystem>();
        go.SetActive(false);

        var main = ps.main;
        main.startLifetime = 2.5f;
        main.startSpeed = 0.8f;
        main.startSize = 0.015f;
        main.maxParticles = 200;
        main.loop = false;
        main.playOnAwake = false;
        main.startColor = new ParticleSystem.MinMaxGradient(Color.red, Color.cyan);

        var em = ps.emission;
        em.rateOverTime = 0;
        em.SetBursts(new[] { new ParticleSystem.Burst(0f, 200) });

        GameManager_M2 gm = FindOrCreateGameManager();
        gm.confetti = ps;
        gm.happyFaceGO = go;
    }

    // --- GAME MANAGER --------------------------------------------------------

    GameManager_M2 FindOrCreateGameManager()
    {
        GameManager_M2 gm = FindFirstObjectByType<GameManager_M2>();
        if (gm != null) return gm;
        return new GameObject("GameManager_M2").AddComponent<GameManager_M2>();
    }

    // --- UI: limpia canvas viejo y crea UI limpia de M2 -----------------------

    void WireUI(GameManager_M2 gm)
    {
        // Busca o crea Canvas
        Canvas cv = FindFirstObjectByType<Canvas>();
        if (cv == null)
        {
            GameObject cGO = new GameObject("Canvas_M2");
            cv = cGO.AddComponent<Canvas>();
            cv.renderMode = RenderMode.ScreenSpaceOverlay;
            cGO.AddComponent<CanvasScaler>();
            cGO.AddComponent<GraphicRaycaster>();
        }
        cv.renderMode = RenderMode.ScreenSpaceOverlay;

        // LIMPIAR todo el canvas (elimina UI leftover de M1)
        for (int i = cv.transform.childCount - 1; i >= 0; i--)
            Destroy(cv.transform.GetChild(i).gameObject);

        Transform c = cv.transform;

        // UI de M2
        gm.progressText = MakeText(c, "ProgressText", "Colocados: 0 / 5",
            new Vector2(0, -40), new Vector2(260, 50), TextAnchor.MiddleRight, 24, Color.white,
            new Vector2(1, 1), new Vector2(1, 1));

        gm.instructionText = MakeText(c, "Instruccion", "Apunta al target. Arrastra las pelotas en orden 1 -> 5",
            new Vector2(0, -40), new Vector2(500, 50), TextAnchor.MiddleCenter, 22, Color.white,
            new Vector2(.5f, 1), new Vector2(.5f, 1));

        // Panel carita feliz
        GameObject hfPanel = MakePanel(c, "HappyFacePanel", Vector2.zero, new Vector2(320, 320), new Color(0, 0, 0, 0.5f));
        hfPanel.SetActive(false);
        gm.happyFacePanel = hfPanel;

        Sprite happySprite = LoadSprite("Assets_Modulo2/carita_feliz");
        if (happySprite != null)
        {
            GameObject imgGO = new GameObject("CaritaImg");
            imgGO.transform.SetParent(hfPanel.transform, false);
            Image img = imgGO.AddComponent<Image>();
            img.sprite = happySprite;
            img.preserveAspect = true;
            RectTransform rt = imgGO.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero; rt.anchoredPosition = Vector2.zero;
        }

        // Win Panel
        GameObject winPanel = MakePanel(c, "WinPanel", Vector2.zero, new Vector2(500, 420), new Color(0.05f, 0.05f, 0.2f, 0.95f));
        winPanel.SetActive(false);
        gm.winPanel = winPanel;
        Transform wp = winPanel.transform;

        Sprite trophySprite = LoadSprite("Assets_Modulo2/trophy");
        if (trophySprite != null) AddCornerImage(wp, trophySprite, new Vector2(190, 160), new Vector2(70, 70));

        gm.winMessage = MakeText(wp, "WinMessage", "¡Excelente!",
            new Vector2(0, 145), new Vector2(420, 60), TextAnchor.MiddleCenter, 38, Color.yellow,
            new Vector2(.5f, .5f), new Vector2(.5f, .5f));

        Sprite starSprite = LoadSprite("Assets_Modulo2/star");
        if (starSprite != null) AddStarsRow(wp, starSprite, new Vector2(0, 80));

        gm.scoreM1Text = MakeText(wp, "ScoreM1", "Modulo 1: 0 pts",
            new Vector2(0, 20), new Vector2(420, 50), TextAnchor.MiddleCenter, 26, Color.white,
            new Vector2(.5f, .5f), new Vector2(.5f, .5f));

        gm.scoreM2Text = MakeText(wp, "ScoreM2", "Modulo 2: 0 pts",
            new Vector2(0, -35), new Vector2(420, 50), TextAnchor.MiddleCenter, 26, Color.white,
            new Vector2(.5f, .5f), new Vector2(.5f, .5f));

        gm.scoreTotalText = MakeText(wp, "ScoreTotal", "TOTAL: 0 pts",
            new Vector2(0, -95), new Vector2(420, 60), TextAnchor.MiddleCenter, 32, Color.cyan,
            new Vector2(.5f, .5f), new Vector2(.5f, .5f));

        MakeButton(wp, "BtnRestart", "Jugar de Nuevo",
            new Vector2(-70, -165), new Vector2(200, 55), new Color(0.1f, 0.6f, 0.1f),
            gm.RestartGame);

        MakeButton(wp, "BtnM1", "Ir a Modulo 1",
            new Vector2(100, -165), new Vector2(180, 55), new Color(0.1f, 0.3f, 0.7f),
            () => SceneManager.LoadScene(0));
    }

    // --- HELPERS -----------------------------------------------------------------

    Sprite LoadSprite(string path)
    {
        Texture2D tex = Resources.Load<Texture2D>(path);
        if (tex == null) return null;
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    }

    void AddCornerImage(Transform parent, Sprite sprite, Vector2 pos, Vector2 size)
    {
        GameObject go = new GameObject("TrophyIcon");
        go.transform.SetParent(parent, false);
        Image img = go.AddComponent<Image>();
        img.sprite = sprite; img.preserveAspect = true;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos; rt.sizeDelta = size;
    }

    void AddStarsRow(Transform parent, Sprite starSprite, Vector2 centerPos)
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject go = new GameObject("Star_" + i);
            go.transform.SetParent(parent, false);
            Image img = go.AddComponent<Image>();
            img.sprite = starSprite; img.preserveAspect = true;
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(45, 45);
            rt.anchoredPosition = new Vector2(centerPos.x + (i - 1) * 55f, centerPos.y);
        }
    }

    Text MakeText(Transform parent, string name, string content,
        Vector2 pos, Vector2 size, TextAnchor anchor, int fs, Color color,
        Vector2 aMin, Vector2 aMax)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = aMin; rt.anchorMax = aMax; rt.sizeDelta = size;
        rt.anchoredPosition = pos;
        Text t = go.AddComponent<Text>();
        t.text = content;
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.fontSize = fs; t.fontStyle = FontStyle.Bold;
        t.color = color; t.alignment = anchor;
        Shadow sh = go.AddComponent<Shadow>();
        sh.effectColor = new Color(0, 0, 0, 0.7f);
        sh.effectDistance = new Vector2(1.5f, -1.5f);
        return t;
    }

    GameObject MakePanel(Transform parent, string name, Vector2 pos, Vector2 size, Color bg)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos; rt.sizeDelta = size;
        go.AddComponent<Image>().color = bg;
        return go;
    }

    void MakeButton(Transform parent, string name, string label,
        Vector2 pos, Vector2 size, Color bg, UnityEngine.Events.UnityAction onClick)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos; rt.sizeDelta = size;
        go.AddComponent<Image>().color = bg;
        go.AddComponent<Button>().onClick.AddListener(onClick);
        GameObject tGO = new GameObject("Text");
        tGO.transform.SetParent(go.transform, false);
        Text t = tGO.AddComponent<Text>();
        t.text = label;
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.fontSize = 22; t.fontStyle = FontStyle.Bold;
        t.color = Color.white; t.alignment = TextAnchor.MiddleCenter;
        RectTransform trt = tGO.GetComponent<RectTransform>();
        trt.anchorMin = Vector2.zero; trt.anchorMax = Vector2.one;
        trt.sizeDelta = Vector2.zero; trt.anchoredPosition = Vector2.zero;
    }

    int[] Shuffle(int[] arr)
    {
        int[] r = (int[])arr.Clone();
        for (int i = r.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int tmp = r[i]; r[i] = r[j]; r[j] = tmp;
        }
        return r;
    }
}
