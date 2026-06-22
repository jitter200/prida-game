using UnityEngine;

public class CodeProgress : MonoBehaviour
{
    public static CodeProgress Instance;

    [Header("Code Progress")]
    public string knownCode = "****";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetKnownDigit(int index, char digit)
    {
        if (index < 0 || index >= knownCode.Length)
        {
            Debug.LogWarning("Wrong code digit index");
            return;
        }

        char[] chars = knownCode.ToCharArray();
        chars[index] = digit;
        knownCode = new string(chars);

        Debug.Log("Known code: " + knownCode);
    }

    public string GetKnownCode()
    {
        return knownCode;
    }
}