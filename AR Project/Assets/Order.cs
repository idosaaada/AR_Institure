using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Order : MonoBehaviour
{
    public TextMeshProUGUI orderText;
    public Button acceptButton;
    public Button rejectButton;

    private int modelIndex;
    private int quantity;
    private FactoryGameManager gameManager;

    public void Initialize(int modelIndex, int quantity, FactoryGameManager gameManager)
    {
        this.modelIndex = modelIndex;
        this.quantity = quantity;
        this.gameManager = gameManager;

        orderText.text = "Order: Model " + (modelIndex + 1) + " x " + quantity;

        acceptButton.onClick.AddListener(() => gameManager.AcceptOrder(this));
        rejectButton.onClick.AddListener(() => gameManager.RejectOrder(this));
    }
}
