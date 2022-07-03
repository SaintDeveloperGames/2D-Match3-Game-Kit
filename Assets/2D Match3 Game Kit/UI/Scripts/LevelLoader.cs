using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private Text _textLevel;

    private SaveData _saveData;

    private ISaveSystem _saveSystem;

    private void Start()
    {
        _saveSystem = new JsonSaveSystem();
        LoadData();
        UpdateTextMenu();
    }

    private void UpdateTextMenu()
    {
        _textLevel.text = "LEVEl " + _saveData.Level;
    }

    private void LoadData()
    {        
        _saveData = _saveSystem.Load();
        if (_saveData.Level == 0)
        {
            _saveData.Level++;
            _saveSystem.Save(_saveData);
        }
    }

    public void OnClick()
    {
        SceneManager.LoadScene(_saveData.Level);
    }
}
