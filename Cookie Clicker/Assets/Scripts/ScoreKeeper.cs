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


    // Метод Start() вызывается при старте игры (он стал private)
    private void Start()
    {
        FillComponents();
        InitValues();
    }

    // НОВОЕ: Метод для заполнения компонентов (может пригодиться и вне Start())
    private void FillComponents()
    {
        _scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        _scorePerClickText = GameObject.Find("ScorePerClickText").GetComponent<Text>();
        _levelText = GameObject.Find("LevelText").GetComponent<Text>();
        _grannySpriteRenderer = Granny.GetComponent<SpriteRenderer>();
        _audioSource = gameObject.GetComponent<AudioSource>();  

    }
    
    // НОВОЕ: Метод для заполнения начальных значений переменных (может пригодиться и вне Start())
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

    // Метод Update() вызывается на каждом кадре игры (он стал private)
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

    // НОВОЕ: Метод для отсчёта времени (может пригодиться и вне Update())
    private void TimerTick()
    {
        if (!_timerEnabled)
        {
            return;
        }

        _timer += Time.deltaTime;
        TrySetActiveGranny();
    }

    // НОВОЕ: Метод для проверки условий для показа бабушки
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

    // НОВОЕ: Метод для показа и скрытия бабушки по значению value
    private void SetActiveGranny(bool value)
    {
        _grannyVisible = value;
        _grannySpriteRenderer.enabled = value;
        ResetTimer();
    }

    // НОВОЕ: Метод для установки таймеру значения 0
    private void ResetTimer()
    {
        _timer = 0;
    }

    // Метод для обработки левого клика по печеньке (он стал private)
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

    // НОВОЕ: Метод для перехода на новую строку в поле увеличения очков за один клик
    private void NewLine()
    {
        if (_counter % 20 == 0 && _counter != 0)
        {
            _scorePerClickText.text += "\n";
        }
    }

    // НОВОЕ: Метод для попытки добавить очки и создать новый префаб печеньки
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

    // НОВОЕ: Метод для увеличения количества очков за клик
    private void IncreaseScoreIncrement()
    {
        if (_score >= _lastScoreStep)
        {
            _scoreIncrement++;
            _lastScoreStep += ScoreSpeedUpStep;
        }
    }

    // НОВОЕ: Метод для добавления очков
    private void AddScore()
    {
        _score += _scoreIncrement;
        _scoreText.text = "Score: " + _score;
        _scorePerClickText.text += " + " + _scoreIncrement;
        _counter++;
        _audioSource.PlayOneShot(ScorePlusSound);
    }

    // НОВОЕ: Метод для создания нового префаба печеньки
    private void SpawnCookie()
    {
        float randomX = Random.Range(-10f, 10f);
        float randomY = Random.Range(-1f, 1f);

        Instantiate(SemitransparentPrefab, new Vector3(randomX, randomY, 0), Quaternion.identity);
    }

    // НОВОЕ: Метод для появления бабушки в игре
    private void StartGrannyLogic()
    {
        if (_score >= 20 && !_timerEnabled)
        {
            SetActiveGranny(true);
            _timerEnabled = true;
        }
    }
}