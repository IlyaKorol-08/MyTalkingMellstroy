using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Данные, которые сохраняются между сценами
    public float hunger = 100f;
    public float energy = 100f;
    public float happiness = 100f;
    public int currentLocationIndex = 2; // Индекс текущей локации (0=Hammam, 1=Spalnya, 2=Kipr, 3=Murino)

    void Awake()
    {
        // Синглтон — только один GameManager на все сцены
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Не уничтожать при смене сцен
        }
        else
        {
            Destroy(gameObject);
        }
    }
}