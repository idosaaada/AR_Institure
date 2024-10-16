using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class FactoryGameManager : MonoBehaviour
{
    public Button[] printerButtons;
    public Button[] modelButtons;
    public Button[] inventoryButtons;

    public GameObject orderPrefab;
    public Transform orderContainer;

    public TextMeshProUGUI timerText;

    private float gameTime = 300.0f; // 5 minutes in seconds
    private int[] inventoryStock = { 500, 500, 500 }; // Initial stock for each material
    private int supplyAmount = 100; // Amount added to inventory on supply order
    private float supplyTime = 30.0f; // Time to receive supplies
    private float printTime = 20.0f; // Time to print a model

    private Queue<Order> orderQueue = new Queue<Order>();
    private Dictionary<int, float> printingTimers = new Dictionary<int, float>();

    private void Start()
    {
        // Initialize printer buttons
        for (int i = 0; i < printerButtons.Length; i++)
        {
            int index = i;
            printerButtons[i].onClick.AddListener(() => SelectPrinter(index));
            SetPrinterButtonColor(index, true); // Initialize printers as available
        }

        // Initialize model buttons
        for (int i = 0; i < modelButtons.Length; i++)
        {
            int index = i;
            modelButtons[i].onClick.AddListener(() => StartPrint(index));
        }

        // Initialize inventory buttons
        for (int i = 0; i < inventoryButtons.Length; i++)
        {
            int index = i;
            inventoryButtons[i].onClick.AddListener(() => OrderSupplies(index));
            UpdateInventoryUI(index);
        }

        // Start game timer
        StartCoroutine(GameTimer());
    }

    private void Update()
    {
        // Update printing timers
        List<int> printersToUpdate = new List<int>(printingTimers.Keys);
        foreach (int printerIndex in printersToUpdate)
        {
            printingTimers[printerIndex] -= Time.deltaTime;
            if (printingTimers[printerIndex] <= 0)
            {
                printingTimers.Remove(printerIndex);
                SetPrinterButtonColor(printerIndex, true); // Mark printer as available
            }
        }
    }

    private IEnumerator GameTimer()
    {
        while (gameTime > 0)
        {
            gameTime -= Time.deltaTime;
            timerText.text = "Time: " + Mathf.Ceil(gameTime).ToString();
            yield return null;
        }

        EndGame();
    }

    private void EndGame()
    {
        // Handle end game logic here
        Debug.Log("Game Over!");
        // Calculate score based on remaining inventory and orders completed
    }

    private void CreateNewOrder(int modelIndex, int quantity)
    {
        GameObject newOrder = Instantiate(orderPrefab, orderContainer);
        Order order = newOrder.GetComponent<Order>();
        order.Initialize(modelIndex, quantity, this);
        orderQueue.Enqueue(order);
    }

    public void AcceptOrder(Order order)
    {
        orderQueue.Dequeue();
        Destroy(order.gameObject);
    }

    public void RejectOrder(Order order)
    {
        orderQueue.Dequeue();
        Destroy(order.gameObject);
    }

    private void SelectPrinter(int printerIndex)
    {
        // Logic to select printer for printing
    }

    private void StartPrint(int modelIndex)
    {
        // Logic to start printing a model
        for (int i = 0; i < printerButtons.Length; i++)
        {
            if (!printingTimers.ContainsKey(i))
            {
                printingTimers[i] = printTime;
                SetPrinterButtonColor(i, false); // Mark printer as busy
                inventoryStock[modelIndex] -= 1;
                UpdateInventoryUI(modelIndex);
                break;
            }
        }
    }

    private void OrderSupplies(int materialIndex)
    {
        StartCoroutine(SupplyRoutine(materialIndex));
    }

    private IEnumerator SupplyRoutine(int materialIndex)
    {
        yield return new WaitForSeconds(supplyTime);
        inventoryStock[materialIndex] += supplyAmount;
        UpdateInventoryUI(materialIndex);
    }

    private void UpdateInventoryUI(int materialIndex)
    {
        TextMeshProUGUI inventoryText = inventoryButtons[materialIndex].GetComponentInChildren<TextMeshProUGUI>();
        inventoryText.text = "Material " + (materialIndex + 1) + ": " + inventoryStock[materialIndex];
    }

    private void SetPrinterButtonColor(int index, bool isAvailable)
    {
        Color color = isAvailable ? Color.green : Color.red;
        ColorBlock cb = printerButtons[index].colors;
        cb.normalColor = color;
        cb.highlightedColor = color;
        printerButtons[index].colors = cb;
    }
}
