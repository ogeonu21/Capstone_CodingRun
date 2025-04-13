using UnityEngine;

public class ConfigManager : MonoBehaviour
{
    public static ConfigManager Instance;
    public ItemConfig itemConfig;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadConfig();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadConfig()
    {
        TextAsset configText = Resources.Load<TextAsset>("itemConfig");
        if (configText != null)
        {
            itemConfig = JsonUtility.FromJson<ItemConfig>(configText.text);
        }
        else
        {
            Debug.LogError("itemConfig.json 파일을 Resources 폴더에서 찾을 수 없습니다. ");
        }
    }
}