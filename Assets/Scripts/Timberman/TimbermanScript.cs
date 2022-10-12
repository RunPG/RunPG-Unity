using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TimbermanScript : MonoBehaviour
{
    [SerializeField]
    private GameObject logTemplate;

    [SerializeField]
    private GameObject tree;
    [SerializeField]
    private GameObject character;
    [SerializeField]
    private GameObject gameInfo;
    [SerializeField]
    private TextMeshProUGUI scoreIndicator;
    [SerializeField]
    private TextMeshProUGUI timer;
    [SerializeField]
    private TextMeshProUGUI tuto;
    [SerializeField]
    private Image veil;
    [SerializeField]
    private TextMeshProUGUI countdown;

    [SerializeField]
    private GameObject results;
    [SerializeField]
    private TextMeshProUGUI resultsScore;
    [SerializeField]
    private TextMeshProUGUI firstQuantity;
    [SerializeField]
    private TextMeshProUGUI secondQuantity;
    [SerializeField]
    private TextMeshProUGUI thirdQuantity;
    [SerializeField]
    private Button resultButton;

    private List<LogScript> logs = new List<LogScript>();
    private int index = 0;
    private int score = 0;

    private Coroutine timerCoroutine;
    private Coroutine resultsCoroutine;

    private bool hasLaunched = false;
    private bool hasStarted = false;
    private bool hasEnded = false;

    // Start is called before the first frame update
    void Start()
    {
        resultButton.onClick.AddListener(() => SkipResults());
        InitLogs();
    }

    private void InitLogs()
    {
        while (index < 9)
        {
            GameObject gameObject = Instantiate(logTemplate, tree.transform);
            LogScript log = gameObject.GetComponent<LogScript>();
            log.InitBranch(index % 2 == 1);
            log.SetPos(index);
            logs.Add(log);
            index++;
        }
    }

    private void UpdateLogsPos()
    {
        for (int i = 0; i < logs.Count; i++)
        {
            logs[i].UpdatePos(i);
        }
    }

    private void RecycleLog()
    {
        LogScript log = logs[0];
        logs.RemoveAt(0);
        UpdateLogsPos();
        log.InitBranch(index % 2 == 1);
        log.SetPos(logs.Count);
        logs.Add(log);
        index++;
    }

    public void TapLeft()
    {
        if (!hasLaunched)
        {
            StartCoroutine(CountdownCoroutine());
        }

        if (hasEnded || !hasStarted)
            return;

        character.transform.position = new Vector3(-1.5f, character.transform.position.y, character.transform.position.z);
        RecycleLog();
        if (logs[0].HasLeftBranch())
        {
            End();
        }
        else
        {            
            score++;
            scoreIndicator.text = score.ToString();
        }
    }

    public void TapRight()
    {
        if (!hasLaunched)
        {
            StartCoroutine(CountdownCoroutine());
        }

        if (hasEnded || !hasStarted)
            return;

        character.transform.position = new Vector3(1.5f, character.transform.position.y, character.transform.position.z);
        RecycleLog();
        UpdateLogsPos();
        if (logs[0].HasRightBranch())
        {
            End();
        }
        else
        {
            score++;
            scoreIndicator.text = score.ToString();
        }
    }

    public void SkipResults()
    {
        StopCoroutine(resultsCoroutine);
        resultsScore.text = score.ToString();
        List<int> quantities = GetQuantities();
        firstQuantity.text = "x " + quantities[0].ToString();
        secondQuantity.text = "x " + quantities[1].ToString();
        thirdQuantity.text = "x " + quantities[2].ToString();

        resultButton.onClick.RemoveAllListeners();
        resultButton.onClick.AddListener(() => Leave());
    }

    public void Leave()
    {
        SceneManager.LoadScene("MapScene");
    }

    private IEnumerator CountdownCoroutine()
    {
        hasLaunched = true;
        veil.gameObject.SetActive(false);
        tuto.gameObject.SetActive(false);
        gameInfo.SetActive(true);
        countdown.gameObject.SetActive(true);
        countdown.text = "3";

        yield return new WaitForSeconds(1f);

        countdown.text = "2";

        yield return new WaitForSeconds(1f);

        countdown.text = "1";

        yield return new WaitForSeconds(1f);

        countdown.gameObject.SetActive(false);
        timerCoroutine = StartCoroutine(TimerCoroutine());
    }

    private IEnumerator TimerCoroutine()
    {
        hasStarted = true;
        float elapsedTime = 0f;
        while (elapsedTime < 30f)
        {
            elapsedTime += Time.deltaTime;
            timer.text = "0:" + (Mathf.RoundToInt(30f - elapsedTime)).ToString();
            yield return null;
        }
        End();
    }

    private void End()
    {
        hasEnded = true;
        StopCoroutine(timerCoroutine);
        resultsCoroutine = StartCoroutine(ResultCoroutine());
    }

    private List<int> GetQuantities()
    {
        List<int> quantities = new List<int>();
        quantities.Add(0);
        quantities.Add(0);
        quantities.Add(0);
        quantities[0] = Mathf.RoundToInt(score);
        quantities[1] = Mathf.RoundToInt(quantities[0] * 0.5f) / 5;
        quantities[0] -= quantities[1] * 5;
        quantities[2] = Mathf.RoundToInt(quantities[1] * 0.5f) / 5;
        quantities[1] -= quantities[2] * 5;
        return quantities;
    }

    private IEnumerator ResultCoroutine()
    {
        veil.gameObject.SetActive(true);
        gameInfo.SetActive(false);
        veil.color = new Color(0, 0, 0, 0);

        float elapsedTime = 0f;
        while (elapsedTime < 0.5f)
        {
            elapsedTime += Time.deltaTime;
            veil.color = new Color(0, 0, 0, Mathf.Lerp(0f, 0.95f, elapsedTime / 0.5f));
            yield return null;
        }

        RectTransform rect = results.GetComponent<RectTransform>();
        elapsedTime = 0f;
        while (elapsedTime < 0.5f)
        {
            elapsedTime += Time.deltaTime;
            rect.pivot = new Vector2(0.5f, Mathf.Lerp(0, 1, EasingFunc.EaseOutBounce(elapsedTime / 0.5f)));
            yield return null;
        }

        resultButton.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.1f);

        elapsedTime = 0f;
        while (elapsedTime < 0.5f)
        {
            elapsedTime += Time.deltaTime;
            resultsScore.text = Mathf.RoundToInt(Mathf.Lerp(0, score, EasingFunc.EaseOutQuad(elapsedTime / 0.7f))).ToString();
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        List<int> quantities = GetQuantities();

        elapsedTime = 0f;
        while (elapsedTime < 0.5f)
        {
            elapsedTime += Time.deltaTime;
            firstQuantity.text = "x " + Mathf.RoundToInt(Mathf.Lerp(0, quantities[0], EasingFunc.EaseOutQuad(elapsedTime / 1f))).ToString();
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        elapsedTime = 0f;
        while (elapsedTime < 0.5f)
        {
            elapsedTime += Time.deltaTime;
            secondQuantity.text = "x " + Mathf.RoundToInt(Mathf.Lerp(0, quantities[1], EasingFunc.EaseOutQuad(elapsedTime / 1f))).ToString();
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        elapsedTime = 0f;
        while (elapsedTime < 0.5f)
        {
            elapsedTime += Time.deltaTime;
            thirdQuantity.text = "x " + Mathf.RoundToInt(Mathf.Lerp(0, quantities[2], EasingFunc.EaseOutQuad(elapsedTime / 1f))).ToString();
            yield return null;
        }

        resultButton.onClick.RemoveAllListeners();
        resultButton.onClick.AddListener(() => Leave());
    }
}
