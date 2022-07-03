using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text _scoreItemsText;
    [SerializeField] private Text _scoreMovesText;
    [SerializeField] private Image _imageItem;
    [SerializeField] private GameObject _panelGameOver;

    private Gameplay _gameplay;

    private SaveData _saveData;

    private ISaveSystem _saveSystem;

    private void Start()
    {
        _saveSystem = new JsonSaveSystem();
        _gameplay = GetComponent<Gameplay>();
        LoadData();
    }

    private void Update()
    {
        UpdateInfo();
    }

    private void UpdateInfo()
    {
        _scoreItemsText.text = _gameplay.ScoreItems.ToString();
        _scoreMovesText.text = _gameplay.ScoreMoves.ToString();
        _imageItem.sprite = _gameplay.SpritesItems[_gameplay.ValueNecessaryItem - 1];
        ShowPanel();
    }

    private void ShowPanel()
    {       
        if (_gameplay.ScoreMoves <= 0 || _gameplay.ScoreItems <= 0)
        {
            var index = 0;
            _panelGameOver.SetActive(true);
            if (_gameplay.ScoreItems <= 0)
            {
                index = 1;
                _gameplay.ScoreItems = 0;
            }                            
            _panelGameOver.GetComponent<Transform>().GetChild(index).gameObject.SetActive(true);
            
        }
    }

    public void OnClickMenu()
    {
        SaveLevel();
        SceneManager.LoadScene(0);
    }

    public void OnClickNextLvl()
    {
        SaveLevel();
        SceneManager.LoadScene(_saveData.Level);
    }

    private void SaveLevel()
    {
        if (_panelGameOver.GetComponent<Transform>().GetChild(1).gameObject.activeSelf)
            if (_saveData.Level < SceneManager.sceneCountInBuildSettings - 1)
            {
                _saveData.Level++;
                SaveData();
            }
    }

    private void LoadData()
    {
        _saveData = _saveSystem.Load();
    }

    private void SaveData()
    {
        _saveSystem.Save(_saveData);
    }
}
