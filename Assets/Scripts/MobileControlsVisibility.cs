using UnityEngine;

public class MobileControlsVisibility : MonoBehaviour
{
    public GameObject mobileControlsRoot;

    private void Start()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (mobileControlsRoot != null)
            mobileControlsRoot.SetActive(true);
#else
        if (mobileControlsRoot != null)
            mobileControlsRoot.SetActive(false);
#endif
    }
}