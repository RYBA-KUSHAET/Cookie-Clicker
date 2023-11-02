using UnityEngine;
using UnityEngine.UI;

public class ScoreKeeper : MonoBehaviour
{

    public GameObject SemitransparentPrefab;
    public GameObject Granny;
    public int ScoreSpeedUpStep = 20;
    public int ScoreToWin1 = 100;
    public int ScoreToWin2 = 200;
    public int ScoreToWin3 = 300;
    public float GrannyTime = 5f;
    public float InvisibleTime = 5f;

    private int _score = 0;
    private int _scoreIncrement = 0;
    private int _lastScoreStep = 0;
    private int _counter = 0;
    private int _level = 1;  
    private Text _scoreText;
    private Text _scorePerClickText;
    private Text _levelText;

    private SpriteRenderer _grannySpriteRenderer;
    private bool _grannyVisible = false;
    private float _timer = 0;
    private bool _timerEnabled = false;
    private bool _changeLevelTo2 = false;
    private bool _changeLevelTo3 = false;
    private AudioSource _audioSource;

    private Vector3  _grannyOriginalPosition;

    public AudioClip ScorePlusSound;
    public AudioClip LevelPassedSound;
    public AudioClip winningTheGameSound;


    // ����� Start() ���������� ��� ������ ���� (�� ���� private)
    private void Start()
    {
        FillComponents();
        InitValues();
    }

    // �����: ����� ��� ���������� ����������� (����� ����������� � ��� Start())
    private void FillComponents()
    {
        _scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        _scorePerClickText = GameObject.Find("ScorePerClickText").GetComponent<Text>();
        _levelText = GameObject.Find("LevelText").GetComponent<Text>();
        _grannySpriteRenderer = Granny.GetComponent<SpriteRenderer>();
        _audioSource = gameObject.GetComponent<AudioSource>();  

    }
    
    // �����: ����� ��� ���������� ��������� �������� ���������� (����� ����������� � ��� Start())
    private void InitValues()
    {
        _score = 0;
        _scoreIncrement = 0;
        _lastScoreStep = 0;
        _counter = 0;

        _scoreText.text = "Score: 0";
        _scorePerClickText.text = "";

        _grannySpriteRenderer.enabled = false;

        _grannyOriginalPosition = Granny.transform.localPosition;
    }

    // ����� Update() ���������� �� ������ ����� ���� (�� ���� private)
    private void Update()
    {
        TimerTick();
        CheckRightClick();

        if (_grannyVisible)
        {
            float xPosition = _grannyOriginalPosition.x + Mathf.Sin(Time.time) * 0.5f;
            Granny.transform.localPosition = new Vector3(xPosition, Granny.transform.localPosition.y, Granny.transform.localPosition.z);
        }
    }

    private void CheckRightClick()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (_level == 2)
            {
                _score -= 15;
            }
            else if (_level == 3)
            {
                _score -= 30;
            }
            _scoreText.text = "Score: " + _score;
        }
    }

    // �����: ����� ��� ������� ������� (����� ����������� � ��� Update())
    private void TimerTick()
    {
        if (!_timerEnabled)
        {
            return;
        }

        _timer += Time.deltaTime;
        TrySetActiveGranny();
    }

    // �����: ����� ��� �������� ������� ��� ������ �������
    private void TrySetActiveGranny()
    {
        if (_grannyVisible && _timer >= GrannyTime)
        {
            SetActiveGranny(false);
        }
        else if (!_grannyVisible && _timer >= InvisibleTime)
        {
            SetActiveGranny(true);
        }
    }

    // �����: ����� ��� ������ � ������� ������� �� �������� value
    private void SetActiveGranny(bool value)
    {
        _grannyVisible = value;
        _grannySpriteRenderer.enabled = value;
        ResetTimer();
    }

    // �����: ����� ��� ��������� ������� �������� 0
    private void ResetTimer()
    {
        _timer = 0;
    }

    // ����� ��� ��������� ������ ����� �� �������� (�� ���� private)
    private void OnMouseDown()
    {
        if (_grannyVisible && _level == 1)
        {
            return;
        }

        CheckGrannyClick();
        NewLine();
        TrySpawnCookie();
        StartGrannyLogic();

        transform.localScale *= 0.8f;
        Invoke(nameof(EnlargeCookie), 0.1f);
    }

    private void CheckGrannyClick()
    {
        if (_grannyVisible && _level > 1)
        {
            if (_grannyVisible && _level == 2)
            {
                _score -= 10;
                _scoreText.text = "Score: " + _score;
            }
            else if (_grannyVisible && _level == 3)
            {
                _score -= 20;
                _scoreText.text = "Score: " + _score;
            }
        }
    }

    private void DisableScript()
    {
        gameObject.GetComponent<ScoreKeeper>().enabled = false;   
    }
    private void EnlargeCookie()
    {
        transform.localScale /= 0.8f;
    }

    // �����: ����� ��� �������� �� ����� ������ � ���� ���������� ����� �� ���� ����
    private void NewLine()
    {
        if (_counter % 20 == 0 && _counter != 0)
        {
            _scorePerClickText.text += "\n";
        }
    }

    // �����: ����� ��� ������� �������� ���� � ������� ����� ������ ��������
    private void TrySpawnCookie()
    {
        if (_score < ScoreToWin3)
        {
            if (_score < ScoreToWin3)
            {
                IncreaseScoreIncrement();
                AddScore();
                SpawnCookie();
            }
            CheckLevel();
        }
    }

    private void CheckLevel()
    {
        if (_score >= ScoreToWin1 && !_changeLevelTo2)
        {
            _levelText.text = "Level: 2";
            _level = 2;
            _changeLevelTo2 = true;
            _audioSource.PlayOneShot(LevelPassedSound);
        }
        else if (_score >= ScoreToWin2 && !_changeLevelTo3)
        {
            _levelText.text = "Level: 3";
            _level = 3;
            _changeLevelTo2 = true;
            _audioSource.PlayOneShot(LevelPassedSound);

            if (_score >= ScoreToWin3)
            {
                _scoreText.text = "You win!!\nScore: " + _score + "\nClicks: " + _counter;
                _audioSource.PlayOneShot(winningTheGameSound);
                DisableScript();
            }
        }
    }

    // �����: ����� ��� ���������� ���������� ����� �� ����
    private void IncreaseScoreIncrement()
    {
        if (_score >= _lastScoreStep)
        {
            _scoreIncrement++;
            _lastScoreStep += ScoreSpeedUpStep;
        }
    }

    // �����: ����� ��� ���������� �����
    private void AddScore()
    {
        _score += _scoreIncrement;
        _scoreText.text = "Score: " + _score;
        _scorePerClickText.text += " + " + _scoreIncrement;
        _counter++;
        _audioSource.PlayOneShot(ScorePlusSound);
    }

    // �����: ����� ��� �������� ������ ������� ��������
    private void SpawnCookie()
    {
        float randomX = Random.Range(-10f, 10f);
        float randomY = Random.Range(-1f, 1f);

        Instantiate(SemitransparentPrefab, new Vector3(randomX, randomY, 0), Quaternion.identity);
    }

    // �����: ����� ��� ��������� ������� � ����
    private void StartGrannyLogic()
    {
        if (_score >= 20 && !_timerEnabled)
        {
            SetActiveGranny(true);
            _timerEnabled = true;
        }
    }
}