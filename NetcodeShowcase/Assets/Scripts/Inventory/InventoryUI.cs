using PlayFab.ClientModels;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryUI : MonoBehaviour
{
	private UIDocument _document;
	private Label _goldValueTxt;
	private Button _earnGoldBtn;
	private Button _buySwordBtn;
	private Button _buyPotionBtn;
	private Button _usePotionBtn;
	private ListView _itemsList;

	private void Awake()
	{
		_document = GetComponent<UIDocument>();
		InitializeUIElements();
		SubscribeToEvents();
		//if (!PlayFabPlayer.Instance.IsLoggedIn) return;
		RefreshInventory();
	}

	private void InitializeUIElements()
	{
		var root = _document.rootVisualElement;

		_goldValueTxt = root.Q<Label>("GoldValueTxt");
		_earnGoldBtn = root.Q<Button>("EarnGoldBtn");
		_buySwordBtn = root.Q<Button>("BuySwordBtn");
		_buyPotionBtn = root.Q<Button>("BuyPotionBtn");
		_usePotionBtn = root.Q<Button>("UsePotionBtn");
		_itemsList = root.Q<ListView>("itemsList");

		// Setup inventory list
		SetupInventoryList();
	}

	private void SubscribeToEvents()
	{
		_earnGoldBtn.clicked += OnEarnGoldClicked;
		_buySwordBtn.clicked += OnBuySwordClicked;
		_buyPotionBtn.clicked += OnBuyPotionClicked;
		_usePotionBtn.clicked += OnUsePotionClicked;
	}



	private void OnEarnGoldClicked()
	{
		Debug.Log("Earn gold button clicked");
		//TODO: add 5 gold
	}

	private void OnBuySwordClicked()
	{
		Debug.Log("Buy sword button clicked");
		// TODO: Implement buy sword logic
	}

	private void OnBuyPotionClicked()
	{
		Debug.Log("Buy potion button clicked");
		// TODO: Implement buy potion logic
	}

	private void OnUsePotionClicked()
	{
		Debug.Log("Use potion button clicked");
		//TODO: Consume first potion (find the first item in the inventory with correct ItemId)
	}


	private void RefreshInventory()
	{
		// TODO: Fetch inventory from PlayFabPlayer and Update the Gold and Inventory displays

	}

	private void UpdateGoldDisplay()
	{
		//TODO: Set the Gold Balance
		_goldValueTxt.text = "00";
	}

	private void UpdateInventoryDisplay()
	{
		//TODO: set the itemsSource 

		//_itemsList.itemsSource = PlayFabPlayer.Instance.Inventory;
		//_itemsList.Rebuild();
	}

	// Binding for the ListView elements to the Inventory List
	private void SetupInventoryList()
	{
		// Configure binding for the template defined in InventoryItem.uxml
		_itemsList.bindItem = (element, index) =>
		{
			if (_itemsList.itemsSource is List<ItemInstance> items)
			{
				var itemLabel = element.Q<Label>("ItemLabel");
				if (itemLabel != null)
				{
					itemLabel.text = items[index].DisplayName;
				}
			}
		};

		// Initialize with empty inventory
		_itemsList.itemsSource = new List<string>();
	}
}
