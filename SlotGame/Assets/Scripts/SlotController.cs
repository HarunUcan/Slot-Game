using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SlotController : MonoBehaviour
{
    #region Collections

    [SerializeField] private List<GameObject> _payLineObjs = new List<GameObject>();
    private Dictionary<int, List<GameObject>> _slotRowsPool = new Dictionary<int, List<GameObject>>(); // key: row index, value: list of slot items in that row
    private bool[] _isRowHasBonus = new bool[5];
    private int[,] _slotMatrix = new int[3, 5];

    #endregion

    #region Private Variables

    private int _lineIndex = 0;
    private int _spinCount = 0;
    private int _maxSpinCount = 18;
    private int _spinCounter = 3;

    private bool _isAutoSpin = false;
    private bool _canSpin = true;
    private bool _canGenerateLine = false;
    private bool _isWin = false;

    private bool _isFreeSpin = false;
    private int  _freeSpinCount = 0;

    private int _amountOfMoney;
    private int _currentBetIndex = 14;
    private int _bet;
    private int[] _bets = { 50, 100, 150, 200, 250, 300, 350, 400, 450, 500, 750, 1000, 1500, 2000, 2500, 3000, 3500, 4000, 5000, 7500, 10000 };
    private float _earnedMoney;


    #endregion

    void Start()
    {
        JsonSaveLoad jsonSaveLoad = new JsonSaveLoad();
        jsonSaveLoad.LoadData();

        _amountOfMoney = PlayerStats.AmountOfMoney;
        _bet = _bets[_currentBetIndex];
        UIManager.Instance.UpdateTotalMoney(_amountOfMoney);
        UIManager.Instance.UpdateCurrentBet(_bet);

        _slotRowsPool = SlotItemSpawner.SlotRowsPool;

        foreach (var item in _payLineObjs)
        {
            item.SetActive(false);
        }
    }

    void Update()
    {
        if (_canGenerateLine)
        {
            _canGenerateLine = false;
            StartCoroutine(GenerateLine());
        }
        if (_isAutoSpin && _canSpin)
        {
            _canSpin = false;
            StartCoroutine(AutoSpin());
        }
    }

    public void Spin()
    {
        if(_amountOfMoney >= _bets[_currentBetIndex] || _isFreeSpin)
        {
            if(_freeSpinCount > 0)
            {
                _freeSpinCount--;
                UIManager.Instance.ShowFreeSpin(_freeSpinCount);
            }
            else if(_isFreeSpin && _freeSpinCount <= 0)
            {                
                UIManager.Instance.BetChangeBtnsInteractable(true);
                _isFreeSpin = false;                
            }
            else
            {
                _amountOfMoney -= _bets[_currentBetIndex];
            }

            PlayerStats.AmountOfMoney = _amountOfMoney;
            JsonSaveLoad jsonSaveLoad = new JsonSaveLoad();
            jsonSaveLoad.SaveData();
            UIManager.Instance.UpdateTotalMoney(_amountOfMoney); 
            StartCoroutine(AudioManager.Instance.PlaySpinSound());
            ResetMatrix();
            GenerateMatrix();
        }
    }
    public IEnumerator AutoSpin()
    {
        
        Spin();
        yield return new WaitForSeconds(3.25f);
        if (_isWin)
        {
            yield return new WaitForSeconds(1f);
            _isWin = false;
        }
        
        _canSpin = true;
        
    }
    public void AutoSpinController()
    {
        UIManager.Instance.ChangeAutoSpinBtnImg();
        if (_isAutoSpin)        
            _isAutoSpin = false;
        
        else
            _isAutoSpin = true;
        
    }
    public void IncreaseBet()
    {
        _currentBetIndex++;
        if(_currentBetIndex > _bets.Length - 1)        
            _currentBetIndex = 0;        
        _bet = _bets[_currentBetIndex];
        UIManager.Instance.UpdateCurrentBet(_bet);
    }
    public void DecreaseBet()
    {
        _currentBetIndex--;
        if (_currentBetIndex < 0)        
            _currentBetIndex = _bets.Length - 1;        
        _bet = _bets[_currentBetIndex];
        UIManager.Instance.UpdateCurrentBet(_bet);
    }
    public void MaxBet()
    {
        for(int i = _bets.Length - 1; i >= 0; i--)
        {
            if(PlayerStats.AmountOfMoney >= _bets[i])
            {
                _currentBetIndex = i;
                _bet = _bets[_currentBetIndex];
                break;
            }
        }
        UIManager.Instance.UpdateCurrentBet(_bet);
    }



    // SlotItemTypes: Lemon, Cherry, Watermelon, Seven, Ten, K, J, Horseshoe, Wild = %8, Bonus = %6, Clover
    // Chances: K = 12%, J = 12%, Ten = 12%, Seven = 12%, Watermelon = 10%, Lemon = 9%, Cherry = 9%, Horseshoe = 7%, Clover = 5%, Wild = 7%, Bonus = 5%
    public void GenerateMatrix()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                var randomItem = !_isRowHasBonus[j] ? Random.Range(0, 100) : Random.Range(0, 95);

                _slotMatrix[i, j] = randomItem < 12 ? (int)SlotItemType.K : 
                                    randomItem < 24 ? (int)SlotItemType.J : 
                                    randomItem < 36 ? (int)SlotItemType.Ten : 
                                    randomItem < 48 ? (int)SlotItemType.Seven : 
                                    randomItem < 58 ? (int)SlotItemType.Watermelon : 
                                    randomItem < 67 ? (int)SlotItemType.Lemon : 
                                    randomItem < 76 ? (int)SlotItemType.Cherry : 
                                    randomItem < 83 ? (int)SlotItemType.Horseshoe : 
                                    randomItem < 88 ? (int)SlotItemType.Clover : 
                                    randomItem < 95 ? (int)SlotItemType.Wild : 
                                                      (int)SlotItemType.Bonus;
                if (_slotMatrix[i, j] == (int)SlotItemType.Bonus)
                    _isRowHasBonus[j] = true;
            }
        }
        _canGenerateLine = true;
    }

    public IEnumerator GenerateLine()
    {
        _spinCount++;
        if (_spinCount < _maxSpinCount)
        {
            for (int i = 0; i < 5; i++)
            {
                var randomItem = (SlotItemType)Random.Range(0, 11);
                var item = _slotRowsPool[i].FirstOrDefault(x => x.GetComponent<SlotItemInfos>().ItemType == randomItem);

                item.GetComponent<SlotItemInfos>().TargetLineIndex = 0;


                _slotRowsPool[i].Remove(item);
                item.SetActive(true);
                item.GetComponent<SlotItemMovement>().CanMove = true;
            }
        }
        else
        {
            for (int i = 0; i < 5; i++)
            {
                var item = _slotRowsPool[i].FirstOrDefault(x => x.GetComponent<SlotItemInfos>().ItemType == (SlotItemType)_slotMatrix[_spinCounter-1, i]);
                item.GetComponent<SlotItemInfos>().TargetLineIndex = _spinCounter;

                _slotRowsPool[i].Remove(item);
                item.SetActive(true);
                item.GetComponent<SlotItemMovement>().CanMove = true;
            }
            _spinCounter--;
            if(_spinCounter <= 0)
                _spinCounter = 3;
        }

        _lineIndex++;
        if (_lineIndex == 3)        
            _lineIndex = 0;

        yield return new WaitForSeconds(0.08f);

        if (_spinCount < 20)        
            _canGenerateLine = true;
        else
        {
            _spinCount = 0;
            CheckWins();
        }

    }

    public void ResetMatrix()
    {
        _spinCount = 0;

        for(int i = 0; i < 5; i++)
            _isRowHasBonus[i] = false;

        foreach (var payLineObj in _payLineObjs)
            payLineObj.SetActive(false);
        
        
        for(int i = 0; i < 5; i++)
        {
            //_slotRowsPool[i].ForEach(x => x.GetComponent<SlotItemMovement>().ResetPosition());
            foreach (var item in SlotItemSpawner.SlotRowsPool[i])
            {
                item.GetComponent<SlotItemMovement>().ResetPosition();                
            }
        }
    }

    private void CheckWins() // wins : {{x,x,x,x,x},{x,x,x,x,0},{0,x,x,x,x},{x,x,x,0,0},{0,x,x,x,0},{0,0,x,x,x}}
    {
        _earnedMoney = 0;
        var payLines = LineController.PayLines; // index = row , value = line
        List<int> lineValues = new List<int>();
        List<int> winLines = new List<int>();

        // Bonus win check
        int bonusCount = 0;

        for (int i = 0; i<3; i++)
        {
            for (int j = 0; j<5; j++)
            {
                if (_slotMatrix[i, j] == (int)SlotItemType.Bonus)
                    bonusCount++;
            }
        }

        switch (bonusCount)
        {
            case 3:
                UIManager.Instance.BetChangeBtnsInteractable(false);
                _isFreeSpin = true;
                _freeSpinCount = 10;
                UIManager.Instance.ShowFreeSpin(_freeSpinCount);
                break;
            case 4:
                UIManager.Instance.BetChangeBtnsInteractable(false);
                _isFreeSpin = true;
                _freeSpinCount = 15;
                UIManager.Instance.ShowFreeSpin(_freeSpinCount);
                break;
            case 5:
                UIManager.Instance.BetChangeBtnsInteractable(false);
                _isFreeSpin = true;
                _freeSpinCount = 20;
                UIManager.Instance.ShowFreeSpin(_freeSpinCount);
                break;
        }

        

        for (int i = 0;i<20;i++)
        {
            lineValues.Clear();
            for(int j = 0; j < 5; j++)
            {
                lineValues.Add(_slotMatrix[payLines[i, j], j]);
            }

            //Wild check
            if (lineValues.Contains((int)SlotItemType.Wild))
            {
                //{w,w,w,w,w}
                if (lineValues[0] == (int)SlotItemType.Wild && lineValues[1] == (int)SlotItemType.Wild && lineValues[2] == (int)SlotItemType.Wild && lineValues[3] == (int)SlotItemType.Wild && lineValues[4] == (int)SlotItemType.Wild)
                {
                    winLines.Add(i);
                    break;
                }
                //{w,w,w,w,0}
                else if (lineValues[0] == (int)SlotItemType.Wild && lineValues[1] == (int)SlotItemType.Wild && lineValues[2] == (int)SlotItemType.Wild && lineValues[3] == (int)SlotItemType.Wild)
                {
                    winLines.Add(i);
                }
                //{0,w,w,w,w}
                else if (lineValues[1] == (int)SlotItemType.Wild && lineValues[2] == (int)SlotItemType.Wild && lineValues[3] == (int)SlotItemType.Wild && lineValues[4] == (int)SlotItemType.Wild)
                {
                    winLines.Add(i);
                    break;
                }
                //{w,w,w,0,0}
                else if (lineValues[0] == (int)SlotItemType.Wild && lineValues[1] == (int)SlotItemType.Wild && lineValues[2] == (int)SlotItemType.Wild)
                {
                    winLines.Add(i);
                    break;
                }
                //{0,w,w,w,0}
                else if (lineValues[1] == (int)SlotItemType.Wild && lineValues[2] == (int)SlotItemType.Wild && lineValues[3] == (int)SlotItemType.Wild)
                {
                    winLines.Add(i);
                    break;
                }
                //{0,0,w,w,w}
                else if (lineValues[2] == (int)SlotItemType.Wild && lineValues[3] == (int)SlotItemType.Wild && lineValues[4] == (int)SlotItemType.Wild)
                {
                    winLines.Add(i);
                    break;
                }
                for (int j = 0; j < 5; j++)
                {
                    if (lineValues[j] == (int)SlotItemType.Wild)
                    {
                        if (lineValues[(j + 1) % 5] != (int)SlotItemType.Bonus && lineValues[(j + 1) % 5] != (int)SlotItemType.Wild)
                        {
                            lineValues[j] = lineValues[(j + 1) % 5];
                        }
                    }
                }
            }
            // {x,x,x,x,x}
            if (lineValues[0] == lineValues[1] && lineValues[1] == lineValues[2] && lineValues[2] == lineValues[3] && lineValues[3] == lineValues[4])
            {
                var item = SlotItemSpawner.SlotRowsPool[0].FirstOrDefault(x => x.GetComponent<SlotItemInfos>().ItemType == (SlotItemType)lineValues[0]);
                _earnedMoney += item.GetComponent<SlotItemInfos>().X5 * _bet;
                winLines.Add(i);
            }
            // {x,x,x,x,0}
            else if (lineValues[0] == lineValues[1] && lineValues[1] == lineValues[2] && lineValues[2] == lineValues[3])
            {
                var item = SlotItemSpawner.SlotRowsPool[0].FirstOrDefault(x => x.GetComponent<SlotItemInfos>().ItemType == (SlotItemType)lineValues[0]);
                _earnedMoney += item.GetComponent<SlotItemInfos>().X4 * _bet;
                winLines.Add(i);
            }
            // {0,x,x,x,x}
            else if (lineValues[1] == lineValues[2] && lineValues[2] == lineValues[3] && lineValues[3] == lineValues[4])
            {
                var item = SlotItemSpawner.SlotRowsPool[0].FirstOrDefault(x => x.GetComponent<SlotItemInfos>().ItemType == (SlotItemType)lineValues[1]);
                _earnedMoney += item.GetComponent<SlotItemInfos>().X4 * _bet;
                winLines.Add(i);
            }
            // {x,x,x,0,0}
            else if (lineValues[0] == lineValues[1] && lineValues[1] == lineValues[2])
            {
                var item = SlotItemSpawner.SlotRowsPool[0].FirstOrDefault(x => x.GetComponent<SlotItemInfos>().ItemType == (SlotItemType)lineValues[0]);
                _earnedMoney += (item.GetComponent<SlotItemInfos>().X3 * _bet);
                winLines.Add(i);
            }
            // {0,x,x,x,0}
            else if (lineValues[1] == lineValues[2] && lineValues[2] == lineValues[3])
            {
                var item = SlotItemSpawner.SlotRowsPool[0].FirstOrDefault(x => x.GetComponent<SlotItemInfos>().ItemType == (SlotItemType)lineValues[1]);
                _earnedMoney += (item.GetComponent<SlotItemInfos>().X3 * _bet);
                winLines.Add(i);
            }
            // {0,0,x,x,x}
            else if (lineValues[2] == lineValues[3] && lineValues[3] == lineValues[4])
            {
                var item = SlotItemSpawner.SlotRowsPool[0].FirstOrDefault(x => x.GetComponent<SlotItemInfos>().ItemType == (SlotItemType)lineValues[2]);
                _earnedMoney += (item.GetComponent<SlotItemInfos>().X3 * _bet);
                winLines.Add(i);
            }

        }
        Debug.Log("Earned Money: " + _earnedMoney); 
        _amountOfMoney += (int)_earnedMoney;
        PlayerStats.AmountOfMoney = _amountOfMoney;
        JsonSaveLoad jsonSaveLoad = new JsonSaveLoad();
        jsonSaveLoad.SaveData();
        UIManager.Instance.UpdateTotalMoney(_amountOfMoney);
        UIManager.Instance.ShowEarnedMoney((int)_earnedMoney);

        if (winLines.Count > 0)
        {
            foreach(var line in winLines)
            {
                _payLineObjs[line].SetActive(true);
            }
            _isWin = true;
        }

    }

}
