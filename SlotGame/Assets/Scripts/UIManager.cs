using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    //singleton
    public static UIManager Instance;

    [SerializeField] private Button _autoSpin;
    [SerializeField] private Button _betUp;
    [SerializeField] private Button _betDown;

    [SerializeField] private Sprite _btnGreenImg;
    [SerializeField] private Sprite _btnPurpleImg;

    [SerializeField] private TMP_Text _totalMoneyText;
    [SerializeField] private TMP_Text _currenBetText;
    [SerializeField] private TMP_Text _earnedMoneyText;
    [SerializeField] private TMP_Text _freeSpinText;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void ChangeAutoSpinBtnImg()
    {
        if(_autoSpin.image.sprite == _btnGreenImg)
        {
            _autoSpin.image.sprite = _btnPurpleImg;
        }
        else
        {
            _autoSpin.image.sprite = _btnGreenImg;
        }
    }

    public void UpdateTotalMoney(int totalMoney)
    {
        // 1000 -> 1,000
        _totalMoneyText.text = totalMoney.ToString("N0");

    }
    public void UpdateCurrentBet(int bet)
    {
        _currenBetText.text = bet.ToString("N0");
    }
    public void ShowEarnedMoney(int earnedMoney)
    {
        _earnedMoneyText.alpha = 1;
        _earnedMoneyText.text = $"+{earnedMoney.ToString("N0")}";
        _earnedMoneyText.DOColor(new Color32(0,222,0,0), 1.5f);
    }
    public void ShowFreeSpin(int freeSpin)
    {
        _freeSpinText.gameObject.SetActive(true);
        _freeSpinText.alpha = 1;
        _freeSpinText.text = $"X{freeSpin}\nFREE SPIN";
        _freeSpinText.DOColor(new Color32(255,0,0,0), 5f).OnComplete(() => { _freeSpinText.gameObject.SetActive(false); });
    }
    public void BetChangeBtnsInteractable(bool interactable)
    {
        _betUp.interactable = interactable;
        _betDown.interactable = interactable;
    }
}
