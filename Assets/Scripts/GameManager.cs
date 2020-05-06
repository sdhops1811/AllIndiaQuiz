using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudienceNetwork;

public class GameManager : MonoBehaviour
{
    public DatabaseHandler databaseHandler;
    public List<QuestionData> questionList;

    public GameObject btn_noAds;

    public AdMobManager adMobManager;
    public UnityAdsManager unityAdsManager;
    public FBAdManager fBAdManager;

    void Start()
    {
        if (PlayerPrefs.GetInt(SingletonClass.instance.IS_NO_ADS, 0) == 0)
        {
            btn_noAds.SetActive(true);
            IAPManager.Instance.InitializeIAPManager(InitComplete);
            //   IAPManager.Instance.RestorePurchases(ProductRestoreCallback);
        }
        else
        {
            btn_noAds.SetActive(false);
        }
        databaseHandler = new DatabaseHandler();
        questionList = databaseHandler.GetQuestionList(1);

    }

    private void InitComplete(IAPOperationStatus status, string errorMsg, List<StoreProduct> allStoreProducts)
    {
        // Check if initialisation is success
        if (status == IAPOperationStatus.Success)
        {
            // Here we check if user had already bought RemoveAds
            for (int i = 0; i < allStoreProducts.Count; i++)
            {
                if (allStoreProducts[i].productName == ShopProductNames.RemoveAd.ToString() || allStoreProducts[i].productName == ShopProductNames.NoAds.ToString())
                {
                    //if active variable is true, means that user had bought that product
                    if (allStoreProducts[i].active)
                    {
                        PlayerPrefs.SetInt(SingletonClass.instance.IS_NO_ADS, 1);
                        btn_noAds.SetActive(false);
                    }
                }
                else
                {
                    //btn_noAds.SetActive(true);
                    //adMobManager.InappInitialised();
                    unityAdsManager.ShowBannerAds();
                }
            }
        }
        else
        {

        }

    }

    public void BuyNoAds()
    {
            if (SingletonClass.instance.IS_FIREBASE)
            {
                Firebase.Analytics.FirebaseAnalytics
                 .LogEvent("remove_ads_initiated");
            }
        IAPManager.Instance.BuyProduct(ShopProductNames.RemoveAd, ProductBrought);
    }

    private void ProductBrought(IAPOperationStatus status, string errorMsg, StoreProduct broughtProduct)
    {
        if (status == IAPOperationStatus.Success)
        {
            if (broughtProduct.productName == ShopProductNames.RemoveAd.ToString())
            {
                // Purchase is complete
                PlayerPrefs.SetInt(SingletonClass.instance.IS_NO_ADS, 1);
                btn_noAds.SetActive(false);

                unityAdsManager.DestroyBanner();

                Debug.Log("fckkkkkkkkk purchase was successful");
                if (SingletonClass.instance.IS_FIREBASE)
                {
                    Firebase.Analytics.FirebaseAnalytics
                     .LogEvent("remove_ads_completed");
                }
            }
        }
        else
        {
            Debug.Log("fckkkkkkkkk purchase failed");
        }
    }

    public void RateUs()
    {
        if (SingletonClass.instance.IS_FIREBASE)
        {
            Firebase.Analytics.FirebaseAnalytics
             .LogEvent("rateus_okay");
        }
        Application.OpenURL("market://details?id=com.oeos.allindiaquiz");
    }

    public void RestorePurchase()
    {
        IAPManager.Instance.RestorePurchases(ProductRestoreCallback);
    }

    private void ProductRestoreCallback(IAPOperationStatus status, string message, StoreProduct product)
    {
        if (status == IAPOperationStatus.Success)
        {
            if (product.productName == ShopProductNames.RemoveAd.ToString() || product.productName == ShopProductNames.NoAds.ToString())
            {
                PlayerPrefs.SetInt(SingletonClass.instance.IS_NO_ADS, 1);
                btn_noAds.SetActive(false);
                adMobManager.DestroyBanner();
                unityAdsManager.DestroyBanner();
            }
        }
    }

    public void OpenPrivacyPolicy()
    {
        Application.OpenURL("http://oeostudios.com/index.php/privacy-policy/");
    }
}
