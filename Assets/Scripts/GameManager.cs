using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    #region Const strings
    private const string TEXT_STARTGAME = "The game will start";
    private const string TEXT_CONGRATULATION = "Congratulation";
    #endregion

    #region Statics
    private static GameManager s_gameManagerInstance = null;
    #endregion

    #region Variables
    [SerializeField] private TMP_Text infoText = null;
    [SerializeField] private TMP_Text playerText = null;
    [SerializeField][Range(1f, 10f)] private float messageTime = 2f;
    [SerializeField] List<string> randomNames = new List<string>();

    private List<CannonController> players = new List<CannonController>();
    private int _currentPlayer = 0;

    private Coroutine _gameCoroutine = null;

    private BallMovement _firedBall = null;
    #endregion

    #region Life Cycle
    private void Awake()
    {
        //Singleton
        if (s_gameManagerInstance != null)
            DestroyImmediate(this);
        else
            s_gameManagerInstance = this;


        //Initialization
        players = gameObject.GetComponentsInChildren<CannonController>().ToList();
        if(randomNames != null && randomNames.Count >= players.Count)
        {
            for(int i = 0; i < players.Count; i++)
            {
                if(players[i] != null)
                {
                    int randomNameIndex = Random.Range(0, randomNames.Count);
                    players[i].name = randomNames[randomNameIndex];
                    randomNames.RemoveAt(randomNameIndex);
                }
            }
        }

        
        if (players == null || players.Count < 2)
            return;

        this.GiveControlTo(-1);

        _currentPlayer = Random.Range(0, players.Count);

        if(_gameCoroutine != null)
        {
            this.StopCoroutine(_gameCoroutine);
            _gameCoroutine = null;
        }

        //Start Game
        _gameCoroutine = this.StartCoroutine(GameLoop());
    }
    #endregion

    #region Publics
    public void SetFiredBall(BallMovement ball)
    {
        if (_firedBall == null)
            _firedBall = ball;
        else if (ball == _firedBall)
            _firedBall = null;
    }
    #endregion

    #region Privates
    private void GiveControlTo(int player)
    {
        for(int i = 0; i < players.Count; i++)
        {
            if (players[i] != null)
                players[i].IsControllable = (i == player);
        }
    }
    #endregion

    #region Coroutines
    private IEnumerator GameLoop()
    {
        //Presentation Text
        if (infoText)
        {
            infoText.text = TEXT_STARTGAME;
            yield return new WaitForSeconds(messageTime);
            infoText.text = string.Empty;
        }

        //GameLoop
        while (players.Count > 1)
        {
            //Check for died people
            for(int i = players.Count-1; i >= 0; i--)
            {
                if (players[i] == null)
                    players.RemoveAt(i);
            }

            //Player turn
            if (_currentPlayer >= players.Count)
                _currentPlayer = 0;

            //If current player exist
            if (players[_currentPlayer] != null)
            {
                //Player turn Text
                if (playerText)
                {
                    playerText.text = players[_currentPlayer].name;
                    playerText.color = players[_currentPlayer].PlayerColor;
                    yield return new WaitForSeconds(messageTime);

                    playerText.text = string.Empty;
                }

                //Give player control
                this.GiveControlTo(_currentPlayer);

                //Wait for player  firing
                UnityAction<BallMovement> fireAction = (BallMovement bm) => {
                    this.SetFiredBall(bm);
                    this.GiveControlTo(-1);
                };
                players[_currentPlayer].OnFire.AddListener(fireAction);

                while (_firedBall == null && players[_currentPlayer] != null && players.Count > 1)
                    yield return new WaitForEndOfFrame();

                if (players[_currentPlayer] != null && players.Count > 1)
                {
                    //Wait for bullet stop
                    BallMovement eventBall = _firedBall;
                    players[_currentPlayer].OnFire.RemoveListener(fireAction);

                    UnityAction ballStoppedAction = () => this.SetFiredBall(eventBall);
                    eventBall.OnStop.AddListener(ballStoppedAction);

                    while (_firedBall != null)
                        yield return new WaitForEndOfFrame();

                    eventBall.OnStop.RemoveListener(ballStoppedAction);
                }

                //Next player
                _currentPlayer++;
            }
        }

        //Player turn Text
        if (infoText && playerText && players.Count == 1)
        {
            infoText.text = TEXT_CONGRATULATION;
            playerText.text = players[0].name;
            playerText.color = players[0].PlayerColor;
        }

    }
    #endregion
}
