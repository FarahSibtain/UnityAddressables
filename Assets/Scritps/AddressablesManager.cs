using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public class AssetReferenceAudioClip : AssetReferenceT<AudioClip>
{
    public AssetReferenceAudioClip(string guid) : base(guid) { }
}

public class AddressablesManager : MonoBehaviour
{
    [SerializeField]
    private AssetReference playerArmatureAssetsRef;

    [SerializeField]
    private AssetReferenceAudioClip musicAssetReference;

    [SerializeField]
    private AssetReferenceTexture2D unityLogoAssetRef;

    [SerializeField]
    private RawImage rawImage;

    [SerializeField]
    private CinemachineVirtualCamera cinemachineVC;

    [SerializeField]
    private GameObject loading;

    GameObject playerController;

    private bool clearPreviousScene = false;
    private SceneInstance previousLoadedScene;
    // Start is called before the first frame update
    void Start()
    {

        Addressables.InitializeAsync().Completed += AddressablesManager_Completed;
    }

    [System.Obsolete]
    private void AddressablesManager_Completed(AsyncOperationHandle<IResourceLocator> obj)
    {
        playerArmatureAssetsRef.LoadAssetAsync<GameObject>().Completed += (playerArmatureAsset) =>
        {
            playerArmatureAssetsRef.InstantiateAsync().Completed += (playerArmaturego) =>
            {
                playerController = playerArmaturego.Result;

                cinemachineVC.Follow = playerController.transform.Find("PlayerCameraRoot");
            };
        };

        musicAssetReference.LoadAsset<AudioClip>().Completed += (clip) =>
        {
            var audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = clip.Result;
            audioSource.playOnAwake = false;
            audioSource.loop = true;
            audioSource.Play();
        };

        unityLogoAssetRef.LoadAssetAsync<Texture2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (unityLogoAssetRef.Asset != null && rawImage.texture == null)
        {
            rawImage.texture = unityLogoAssetRef.Asset as Texture2D;
            Color currentColor = rawImage.color;
            currentColor.a = 1.0f;
            rawImage.color = currentColor;
        }

        if (playerArmatureAssetsRef.Asset != null && musicAssetReference.Asset != null  && unityLogoAssetRef.Asset != null)
        {
            loading.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        playerArmatureAssetsRef.ReleaseInstance(playerController);
        unityLogoAssetRef.ReleaseAsset();
    }

    public void LoadAddressableLevel(string addressableKey)
    {
        if (clearPreviousScene)
        {
            Addressables.UnloadScene(previousLoadedScene).Completed += (asyncHandle) =>
            {
                clearPreviousScene = false;
                previousLoadedScene = new SceneInstance();

            };            
        }

        Addressables.LoadSceneAsync(addressableKey, LoadSceneMode.Additive).Completed += (asyncHandle) =>
        {
            clearPreviousScene = true;
            previousLoadedScene = asyncHandle.Result;
        };
    }

    public void LoadScene()
    {
        SceneManager.LoadScene("Playground");
    }
}
