#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// You must obfuscate your secrets using Window > Unity IAP > Receipt Validation Obfuscator
// before receipt validation will compile in this sample.
#define RECEIPT_VALIDATION
#endif

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

using UnityEngine.Purchasing;

#if RECEIPT_VALIDATION
using UnityEngine.Purchasing.Security;
#endif

public class MarketService : SingleTon<MarketService>, IStoreListener {

    // Unity IAP objects 
    private IStoreController m_Controller;
    private IAppleExtensions m_AppleExtensions;

    private int m_SelectedItemIndex = -1; // -1 == no product
    private bool m_PurchaseInProgress;

    private Selectable m_InteractableSelectable; // Optimization used for UI state management

    //참조 - GameDAO에서 초기화
    public GameDAO.PlayerData playerData = null;
    public SystemDAO.FileManager file_manager = null;

    //값
    private const int small_value = 3;
    private const int medium_value = 18;
    private const int large_value = 40;

    //쿠폰 체크 여부
    private bool check_crystal_small = false;
    private bool check_crystal_medium = false;
    private bool check_crystal_large = false;

#if RECEIPT_VALIDATION
    private CrossPlatformValidator validator;
#endif

    private bool IsInitialized()
    {
        // Only say we are initialized if both the Purchasing references are set.
        return m_Controller != null;
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        m_Controller = controller;
        m_AppleExtensions = extensions.GetExtension<IAppleExtensions>();

        // On Apple platforms we need to handle deferred purchases caused by Apple's Ask to Buy feature.
        // On non-Apple platforms this will have no effect; OnDeferred will never be called.
        m_AppleExtensions.RegisterPurchaseDeferredListener(OnDeferred);

        // Prepare model for purchasing
        if (m_Controller.products.all.Length > 0)
        {
            m_SelectedItemIndex = 0;
        }

        // Populate the product menu now that we have Products
        for (int t = 0; t < m_Controller.products.all.Length; t++)
        {
            var item = m_Controller.products.all[t];
            var description = string.Format("{0} - {1}", item.metadata.localizedTitle, item.metadata.localizedPriceString);
        }
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        Debug.Log("Purchase OK: " + e.purchasedProduct.definition.id);
        Debug.Log("Receipt: " + e.purchasedProduct.receipt);

        m_PurchaseInProgress = false;

#if RECEIPT_VALIDATION
        if (Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            try
            {
                var result = validator.Validate(e.purchasedProduct.receipt);
                Debug.Log("Receipt is valid. Contents:");
                foreach (IPurchaseReceipt productReceipt in result)
                {
                    Debug.Log(productReceipt.productID);
                    Debug.Log(productReceipt.purchaseDate);
                    Debug.Log(productReceipt.transactionID);

                    GooglePlayReceipt google = productReceipt as GooglePlayReceipt;
                    if (null != google)
                    {
                        Debug.Log(google.purchaseState);
                        Debug.Log(google.purchaseToken);
                    }

                    AppleInAppPurchaseReceipt apple = productReceipt as AppleInAppPurchaseReceipt;
                    if (null != apple)
                    {
                        Debug.Log(apple.originalTransactionIdentifier);
                        Debug.Log(apple.cancellationDate);
                        Debug.Log(apple.quantity);
                    }
                }
            }
            catch (IAPSecurityException)
            {
                Debug.Log("Invalid receipt, not unlocking content");
                return PurchaseProcessingResult.Complete;
            }
        }
#endif
        // You should unlock the content here.
        if (String.Equals(e.purchasedProduct.definition.id, "test_crystal", StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", e.purchasedProduct.definition.id));
            playerData.my_subdata.crystal += small_value;
            file_manager.SaveAll();
        }
        else if (String.Equals(e.purchasedProduct.definition.id, "crystal_medium", StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", e.purchasedProduct.definition.id));
            playerData.my_subdata.crystal += medium_value;
            file_manager.SaveAll();
        }
        else if (String.Equals(e.purchasedProduct.definition.id, "crystal_large", StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", e.purchasedProduct.definition.id));
            playerData.my_subdata.crystal += large_value;
            file_manager.SaveAll();
        } 
        else if (String.Equals(e.purchasedProduct.definition.id, "commercial_off", StringComparison.Ordinal)) {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", e.purchasedProduct.definition.id));
            file_manager.commercial_off = true;
            file_manager.SaveAll();
        } 
        else if (String.Equals(e.purchasedProduct.definition.id, "armor", StringComparison.Ordinal)) {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", e.purchasedProduct.definition.id));
            file_manager.SaveClothBuy(Define.SPECIAL_CLOTH);
            file_manager.SaveAll();
        }
        // Indicate we have handled this purchase, we will not be informed of it again.x
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product item, PurchaseFailureReason r)
    {
        Debug.Log("Purchase failed: " + item.definition.id);
        Debug.Log(r);

        m_PurchaseInProgress = false;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("Billing failed to initialize!");
        switch (error)
        {
            case InitializationFailureReason.AppNotKnown:
                Debug.LogError("Is your App correctly uploaded on the relevant publisher console?");
                break;
            case InitializationFailureReason.PurchasingUnavailable:
                // Ask the user if billing is disabled in device settings.
                Debug.Log("Billing disabled!");
                break;
            case InitializationFailureReason.NoProductsAvailable:
                // Developer configuration error; check product metadata.
                Debug.Log("No products available for purchase!");
                break;
        }
    }

    private void OnDeferred(Product item)
    {
        Debug.Log("Purchase deferred: " + item.definition.id);
    }

    private void OnTransactionsRestored(bool success)
    {
        Debug.Log("Transactions restored.");
    }

    //초기화
    void Awake()
    {
        //파괴 안되게
        DontDestroyOnLoad(gameObject);

        //이미 초기화 되있으면 실행X
        if (IsInitialized())
            return;

        var module = StandardPurchasingModule.Instance();

        // The FakeStore supports: no-ui (always succeeding), basic ui (purchase pass/fail), and 
        // developer ui (initialization, purchase, failure code setting). These correspond to 
        // the FakeStoreUIMode Enum values passed into StandardPurchasingModule.useFakeStoreUIMode.
        //module.useFakeStoreUIMode = FakeStoreUIMode.StandardUser;

        var builder = ConfigurationBuilder.Instance(module);
        // This enables the Microsoft IAP simulator for local testing.
        // You would remove this before building your release package.
        //builder.Configure<IMicrosoftConfiguration>().useMockBillingSystem = true;
        //builder.Configure<IGooglePlayConfiguration>().SetPublicKey("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAjdFv0ty34YbYflul9H9dxW3uYfR/zPUlTUnGcWICJOoi7HmddNPhG6K+DwRLHYVQC9SPsdnu6IHezfjLMZdU8u8jJbvaZDC/osRDkISoNGdDgGQac5gfUvCxyQpV6FjFq9yyKDp8uQ506Hwyj/4rI4Lff5q8dM60NaRX87ysAy+wwJgjKNz1dddUxDRU0usIP/9jK2XTeEWqZHLWbOIBOTLd4x63ssYj/eEQerSo8iKw6kOjkkBvyXMny3+aqIHYZaoR2Lw7Ua41mpXNJGE23y5Xx9B4aDq7Om7L5PQ1+1wD/QOMm0+VUqvjRHq0WECzJ/AixSYElXxTxCzo0uWUVQIDAQAB");
        builder.Configure<IGooglePlayConfiguration>().SetPublicKey("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAoF6LTXuNooBS3BP1zK0DEngPFQVz0KkfG4ziCAZoE1mVp/wb7JfvwDtHUKw7tL1RsmPG3RXdVuKuQ+/ahqflHT/x1rS6OQL5bC/fZ/Il9BbVW73iQRUi0UeNeXbdXLPi0+31qGJwVr1QBDH2SgJM5/fkLdCxUHz7fjDppup9jqJCEp8W5wAeit6rcofNUpknIz8G1QTtNhDqrgkLkqiUsKdYPZFF41fBMPMtptZWO1l7gVDqQDI6Gf7Tye2seJ9zYfm6mCg7cWRNGlQCD5r+TKMggzbW2BIjMOUq7yFC2DgpPJDEjKbNszFOxxl7FiKolAiV/2qg0ZMlerI8UDatRwIDAQAB");

        // Define our products.
        // In this case our products have the same identifier across all the App stores,
        // except on the Mac App store where product IDs cannot be reused across both Mac and
        // iOS stores.
        // So on the Mac App store our products have different identifiers,
        // and we tell Unity IAP this by using the IDs class.
        builder.AddProduct("test_crystal", ProductType.Consumable, new IDs
        {
            {"test_crystal", GooglePlay.Name},
        });

        builder.AddProduct("crystal_medium", ProductType.Consumable, new IDs
        {
            {"crystal_medium", GooglePlay.Name},
        });

        builder.AddProduct("crystal_large", ProductType.Consumable, new IDs
        {
            {"crystal_large", GooglePlay.Name},
        });
        builder.AddProduct("commercial_off", ProductType.Consumable, new IDs
        {
            {"commercial_off", GooglePlay.Name },
        });
        builder.AddProduct("armor", ProductType.Consumable, new IDs
        {
            {"armor", GooglePlay.Name },
        });
        // Write Amazon's JSON description of our products to storage when using Amazon's local sandbox.
        // This should be removed from a production build.
        //builder.Configure<IAmazonConfiguration>().WriteSandboxJSON(builder.products);

#if RECEIPT_VALIDATION
        validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.bundleIdentifier);
#endif
        // Now we're ready to initialize Unity IAP.
        UnityPurchasing.Initialize(this, builder);
    }

    //구매
    public void BuyProductID(string productId)
    {
        // If the stores throw an unexpected exception, use try..catch to protect my logic here.
        try
        {
            // If Purchasing has been initialized ...
            if (IsInitialized())
            {
                // ... look up the Product reference with the general product identifier and the Purchasing system's products collection.
                Product product = m_Controller.products.WithID(productId);

                // If the look up found a product for this device's store and that product is ready to be sold ... 
                if (product != null && product.availableToPurchase)
                {
                    Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));// ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed asynchronously.
                    m_Controller.InitiatePurchase(product);
                    if (String.Equals(productId, "test_crystal"))
                        check_crystal_small = true;
                    if (String.Equals(productId, "crystal_medium"))
                        check_crystal_medium = true;
                    if (String.Equals(productId, "crystal_large"))
                        check_crystal_large = true;
                }
                // Otherwise ...
                else {
                    // ... report the product look-up failure situation  
                    Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                }
            }
            // Otherwise ...
            else {
                // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or retrying initiailization.
                Debug.Log("BuyProductID FAIL. Not initialized.");
            }
        }
        // Complete the unexpected exception handling ...
        catch (Exception e)
        {
            // ... by reporting any unexpected exception for later diagnosis.
            Debug.Log("BuyProductID: FAIL. Exception during purchase. " + e);
        }
    }    

    //결제여부 체크
    public void CheckPurchasedItem(string productId)
    {
        if (String.Equals(productId, "test_crystal") && check_crystal_small)
            return;
        if (String.Equals(productId, "crystal_medium") && check_crystal_medium)
            return;
        if (String.Equals(productId, "crystal_large") && check_crystal_large)
            return;

        try
        {
            if (IsInitialized())
            {
                Product product = m_Controller.products.WithID(productId);
                
                if (product != null && product.availableToPurchase && product.hasReceipt)
                {
                    m_Controller.InitiatePurchase(product);
                    if (String.Equals(productId, "test_crystal"))
                        check_crystal_small = true;
                    if (String.Equals(productId, "crystal_medium"))
                        check_crystal_medium = true;
                    if (String.Equals(productId, "crystal_large"))
                        check_crystal_large = true;
                }
                else
                {
                    // ... report the product look-up failure situation  
                    Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                }
            }
            else
            {
                // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or retrying initiailization.
                Debug.Log("BuyProductID FAIL. Not initialized.");
            }
        }
        catch (Exception e)
        {
            // ... by reporting any unexpected exception for later diagnosis.
            Debug.Log("BuyProductID: FAIL. Exception during purchase. " + e);
        }
    }
}
