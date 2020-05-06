using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class NativeShareButton : MonoBehaviour
{
    private bool isProcessing = false;
    private bool isFocus = false;

    public void Share()
    {
        StartCoroutine(TakeSSAndShare());
    }

    IEnumerator TakeSSAndShare()
    {

        yield return new WaitForEndOfFrame();

        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();

        string filePath = Path.Combine(Application.temporaryCachePath, "shared img.png");
        File.WriteAllBytes(filePath, ss.EncodeToPNG());

        // To avoid memory leaks
        Destroy(ss);

        new NativeShare().AddFile(filePath).SetSubject("All India Quiz").SetText("Test your knowledge of India in All India Quiz!\n PlayStore: https://play.google.com/store/apps/details?id=com.oeos.allindiaquiz \n AppStore: https://apps.apple.com/us/app/all-india-quiz/id1504230585?ls=1").Share();

        if (SingletonClass.instance.IS_FIREBASE)
        {
            Firebase.Analytics.FirebaseAnalytics
             .LogEvent("main_menu_share");
        }
        // Share on WhatsApp only, if installed (Android only)
        //if (NativeShare.TargetExists("com.facebook"))
        //    new NativeShare().AddFile(filePath).SetText("Test your knowledge of India in All India Quiz!").SetTarget("com.whatsapp").Share();

    }

    //public void ShareBtnPress()
    //{
    //    if (!isProcessing)
    //    {
    //        //   CanvasShareObj.SetActive(true);
    //        StartCoroutine(ShareRefCode());
            
    //    }
    //}

    //IEnumerator ShareRefCode()
    //{
    //    isProcessing = true;

    //    yield return new WaitForSecondsRealtime(0.3f);

    //    if (!Application.isEditor)
    //    {
    //        AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
    //        AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
    //        intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
    //        AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
    //        //            AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + destination);
    //        //             intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"),
    //        //                uriObject);
    //        //intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"),
    //        //    "https://play.google.com/store/apps/details?id=com.firescore.gamingarcade Download Pocket League Lite and use my invite code " + GameManager.instance.REF_CODE + " to get free cash worth $" + GameManager.instance.REF_AMOUNT + "! Let the games begin!");

    //        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"),
    //            "https://play.google.com/store/apps/details?id=com.firescore.gamingarcade Download Pocket League Lite and use my invite code");

    //        //            intentObject.Call<AndroidJavaObject>("setType", "image/jpeg");
    //        intentObject.Call<AndroidJavaObject>("setType", "text/plain");
    //        AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    //        AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
    //        AndroidJavaObject chooser = intentClass.CallStatic<AndroidJavaObject>("createChooser",
    //            intentObject, "Refer and earn Rs.0.1");
    //        currentActivity.Call("startActivity", chooser);

    //        PlayerPrefs.SetInt("is_shared", 1);
    //        yield return new WaitForSecondsRealtime(1);
    //    }

    //    yield return new WaitUntil(() => isFocus);
    //    //    CanvasShareObj.SetActive(false);
    //    isProcessing = false;
    //}
    //private void OnApplicationFocus(bool focus)
    //{
    //    isFocus = focus;
    //}
}
