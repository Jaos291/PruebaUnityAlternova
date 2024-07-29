using UnityEngine;
using UnityEngine.UI;

public class BlockBehaviour : MonoBehaviour
{
    public Text numberText;
    public Image image;
    public Button button;
    public int number;
    public bool isRevealed = false;

    public void Setup(int num)
    {
        number = num;
        numberText.text = "";
    }

    public void OnClick()
    {
        if (GameController.Instance.canPlay)
        {
            if (isRevealed) return;

            isRevealed = true;
            image.enabled = true;
            numberText.text = number.ToString();

            GameController.Instance.gridManager.BlockRevealed(this);
        }
        
    }

    public void Hide()
    {
        isRevealed = false;
        image.enabled = false;
        numberText.text = "";
    }
}
