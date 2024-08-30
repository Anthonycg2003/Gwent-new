using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameControler : MonoBehaviour
{
    Interpreter interpreter;
    [SerializeField] GameObject CardPrefab;
    [SerializeField] GameObject DeckOfPlayer;
    [SerializeField] GameObject DeckOfOpponent;
    public GameObject FieldOfPlayer;
    public GameObject FieldOfOpponent;
    [SerializeField] GameObject HandOfPlayer;
    [SerializeField] GameObject HandOfOpponent;
    [SerializeField] GameObject GraveyardOfPlayer;
    [SerializeField] GameObject GraveyardOfOpponent;
    public GameObject PlayerMeleeZone;
    public GameObject PlayerSiegeZone;
    public GameObject PlayerRangeZone;
    public GameObject PlayerBoostMeleeZone;
    public GameObject PlayerBoostRangeZone;
    public GameObject PlayerBoostSiegeZone;
    public GameObject OpponentMeleeZone;
    public GameObject OpponentSiegeZone;
    public GameObject OpponentRangeZone;
    public GameObject OpponentBoostMeleeZone;
    public GameObject OpponentBoostRangeZone;
    public GameObject OpponentBoostSiegeZone;
    public GameObject WeatherZone;
    [SerializeField] TMP_Text TurnText;
    [SerializeField] GameObject coin;
    ZoneInterface[] Zones;
    public bool IsPlayerTurn;
    bool PlayerPlayActualRound;
    bool OpponentPlayActualRound;
    Round Round;
    Stack<Round> Rounds;
    int PlayerPower;
    int OpponentPower;
    int PlayerWinRounds;
    int OpponentWinRounds;
    public GameControler()
    {
        Rounds = new Stack<Round>();
        Rounds.Push(Round.Round_3);
        Rounds.Push(Round.Round_2);
        Rounds.Push(Round.Round_1);
        PlayerWinRounds = 0;
        OpponentWinRounds = 0;
    }

    void Start()
    {
        DataCards dataCards = GameObject.FindWithTag("Data").GetComponent<DataCards>();
        interpreter = new Interpreter(dataCards.elementalProgram);
        CreateCards(dataCards.PlayerDeck, true);
        CreateCards(dataCards.OpponentDeck, false);
        Draw(10, true);
        Draw(10, false);
        Invoke("StartTurn", 1f);
        PlayerPlayActualRound = true;
        OpponentPlayActualRound = true;
        Round = Rounds.Pop();
        PlayerPower = 0;
        OpponentPower = 0;
        Zones = new ZoneInterface[8]
        {
            DeckOfPlayer.GetComponent<Deck>(),DeckOfOpponent.GetComponent<Deck>(),HandOfPlayer.GetComponent<Hand>(),HandOfOpponent.GetComponent<Hand>(),GraveyardOfPlayer.GetComponent<Graveyard>(),GraveyardOfOpponent.GetComponent<Graveyard>(),FieldOfPlayer.GetComponent<Field>(),FieldOfOpponent.GetComponent<Field>()
        };
    }
    public void ManagerCard(GameCard gameCard)
    {
        Player owner = (Player)gameCard.Properties["Owner"];
        Debug.Log("can invoke?");
        if (MaySummon())
        {
            Debug.Log("yes");
            Summon();
        }
        #region internal metods
        void Summon()
        {
            switch (gameCard.Card.Type)
            {
                case CardType.Oro:
                    {
                        ManageUnitSummon(gameCard.Card.Ranges);
                        break;
                    }
                case CardType.Plata:
                    {
                        ManageUnitSummon(gameCard.Card.Ranges);
                        break;
                    }
                case CardType.Clima:
                    {
                        SummonIn(WeatherZone.transform);
                        PostSummon();
                        break;
                    }
                case CardType.Aumento:
                    {
                        PostSummon();
                        break;
                    }
                case CardType.Lider:
                    {
                        PostSummon();
                        break;
                    }
            }
        }
        void ManageUnitSummon(List<Range> Ranges)
        {
            if (Ranges.Count == 1)
            {
                switch (Ranges[0].ToString())
                {
                    case "Melee":
                        {
                            if (owner == Player.player)
                            {
                                SummonIn(PlayerMeleeZone.transform);
                            }
                            else
                            {
                                SummonIn(OpponentMeleeZone.transform);
                            }
                            break;
                        }
                    case "Range":
                        {
                            if (owner == Player.player)
                            {
                                SummonIn(OpponentRangeZone.transform);
                            }
                            else
                            {
                                SummonIn(OpponentRangeZone.transform);
                            }
                            break;
                        }
                    case "Siege":
                        {
                            if (owner == Player.player)
                            {
                                SummonIn(OpponentSiegeZone.transform);
                            }
                            else
                            {
                                SummonIn(OpponentSiegeZone.transform);
                            }
                            break;
                        }
                }
                PostSummon();
            }
            else
            {
                ActivateSummonZones(Ranges);
            }
        }
        void ActivateSummonZones(List<Range> Ranges)
        {
            Debug.Log("activationg zones");
            foreach (Range Range in Ranges)
            {
                switch (Range)
                {
                    case Range.Melee:
                        {
                            if (owner == Player.player)
                            {
                                PlayerMeleeZone.GetComponent<SummonZone>().ActiveZone(gameCard, Ranges);
                            }
                            else
                            {
                                OpponentMeleeZone.GetComponent<SummonZone>().ActiveZone(gameCard, Ranges);
                            }
                            continue;
                        }
                    case Range.Range:
                        {
                            if (owner == Player.player)
                            {
                                PlayerRangeZone.GetComponent<SummonZone>().ActiveZone(gameCard, Ranges);
                            }
                            else
                            {
                                OpponentRangeZone.GetComponent<SummonZone>().ActiveZone(gameCard, Ranges);
                            }
                            continue;
                        }
                    case Range.Siege:
                        {
                            if (owner == Player.player)
                            {
                                PlayerSiegeZone.GetComponent<SummonZone>().ActiveZone(gameCard, Ranges);
                            }
                            else
                            {
                                OpponentSiegeZone.GetComponent<SummonZone>().ActiveZone(gameCard, Ranges);
                            }
                            continue;
                        }
                }
            }
        }
        void PostSummon()
        {
            if (owner == Player.player)
            {
                if (OpponentPlayActualRound)
                {
                    IsPlayerTurn = false;
                    UpdateTurn();
                }
            }
            else
            {
                if (PlayerPlayActualRound)
                {
                    IsPlayerTurn = true;
                    UpdateTurn();
                }
            }
            interpreter.Interpret(gameCard.Card);
            SyncZonesWithList();
            gameCard.InBattle=true;
        }
        void SummonIn(Transform parent)
        {
            gameCard.gameObject.transform.SetParent(parent);
        }
        bool MaySummon()
        {
            if(gameCard.InBattle)
            {
                return false;
            }
            if (owner == Player.player)
            {
                if (IsPlayerTurn)
                {
                    return true;
                }
            }
            else
            {
                if (!IsPlayerTurn)
                {
                    return true;
                }
            }
            return false;
        }
    #endregion
    }
    public void SummonIn(Transform parent, List<Range> Ranges, GameCard gameCard)
    {
        Player owner = (Player)gameCard.Properties["Owner"];
        SummonIn(parent);
        DisableSummonZones(Ranges);
        PostSummon();
        void DisableSummonZones(List<Range> Ranges)
        {
            foreach (Range Range in Ranges)
            {
                switch (Range)
                {
                    case Range.Melee:
                        {
                            if (owner == Player.player)
                            {
                                PlayerMeleeZone.GetComponent<SummonZone>().DisableZone();
                            }
                            else
                            {
                                OpponentMeleeZone.GetComponent<SummonZone>().DisableZone();
                            }
                            break;
                        }
                    case Range.Range:
                        {
                            if (owner == Player.player)
                            {
                                PlayerRangeZone.GetComponent<SummonZone>().DisableZone();
                            }
                            else
                            {
                                OpponentRangeZone.GetComponent<SummonZone>().DisableZone();
                            }
                            break;
                        }
                    case Range.Siege:
                        {
                            if (owner == Player.player)
                            {
                                PlayerSiegeZone.GetComponent<SummonZone>().DisableZone();
                            }
                            else
                            {
                                OpponentSiegeZone.GetComponent<SummonZone>().DisableZone();
                            }
                            break;
                        }
                }
            }
        }
        void PostSummon()
        {
            if (owner == Player.player)
            {
                if (OpponentPlayActualRound)
                {
                    IsPlayerTurn = false;
                    UpdateTurn();
                }
            }
            else
            {
                if (PlayerPlayActualRound)
                {
                    IsPlayerTurn = true;
                    UpdateTurn();
                }
            }
            interpreter.Interpret(gameCard.Card);
            SyncZonesWithList();
            gameCard.InBattle=true;
        }
        void SummonIn(Transform parent)
        {
            gameCard.gameObject.transform.SetParent(parent);
        }
    }
    void DecideRoundWinner()
    {
        if (PlayerPower > OpponentPower)
        {
            PlayerWinRounds++;
        }
        else if (OpponentPower > PlayerPower)
        {
            OpponentWinRounds++;
        }
        else
        {
            PlayerWinRounds++;
            OpponentWinRounds++;
        }
    }
    bool CheckGameWinner(bool IsPlayer)
    {
        if (IsPlayer)
        {
            if (PlayerWinRounds >= 2)
            {
                return true;
            }
        }
        else
        {
            if (OpponentWinRounds >= 2)
            {
                return true;
            }
        }
        return false;
    }
    void FinishedGame(bool? IsPlayerWinner)
    {

    }
    void SyncZonesWithList()
    {
        foreach(ZoneInterface zone in Zones)
        {
            zone.SyncWithList();
            zone.UpdateCardsProperties();
        }
    }
    bool BothPlayerPased()
    {
        return !(PlayerPlayActualRound || OpponentPlayActualRound);
    }
    public void UpdateTurn()
    {
        if (IsPlayerTurn)
        {
            TurnText.text = "Player Turn";
        }
        else
        {
            TurnText.text = "Opponent Turn";
        }
    }
    public void PassTurn()
    {
        if (IsPlayerTurn)
        {
            IsPlayerTurn = false;
            PlayerPlayActualRound = false;
            if (BothPlayerPased())
            {
                PassRound();
            }
        }
        else
        {
            IsPlayerTurn = true;
            OpponentPlayActualRound = false;
            if (BothPlayerPased())
            {
                PassRound();
            }
        }
        UpdateTurn();
    }
    void PassRound()
    {
        DecideRoundWinner();
        bool PlayerWinner = CheckGameWinner(true);
        bool OpponentWinner = CheckGameWinner(false);
        if (PlayerWinner || OpponentWinner)
        {
            if (PlayerWinner && OpponentWinner)
            {
                FinishedGame(null);
            }
            else if (PlayerWinner)
            {
                FinishedGame(true);
            }
            else
            {
                FinishedGame(false);
            }
        }
    }
    void Draw(int numberOfCards, bool IsPlayer)
    {
        if (IsPlayer)
        {
            for (int i = 0; i < numberOfCards; i++)
            {
                try
                {
                    DeckOfPlayer.transform.GetChild(0).SetParent(HandOfPlayer.transform);
                }
                catch
                {
                    Debug.Log("there isnt any cards in player deck");
                }

            }
        }
        else
        {
            for (int i = 0; i < numberOfCards; i++)
            {
                try
                {
                    DeckOfOpponent.transform.GetChild(0).SetParent(HandOfOpponent.transform);
                }
                catch
                {
                    Debug.Log("there isnt any cards in opponent deck");
                }
            }
        }
    }
    void StartTurn()
    {
        coin.GetComponent<Animator>().enabled = false;
        int x = Random.Range(0, 2);
        if (x == 0)
        {
            IsPlayerTurn = true;
            coin.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            IsPlayerTurn = false;
            coin.transform.rotation = Quaternion.Euler(359, 0, 0);
        }
        Destroy(coin, 1f);
        UpdateTurn();
    }
    void CreateCards(Dictionary<Card, int> Deck, bool IsPlayer)
    {
        foreach (KeyValuePair<Card, int> keyValuePair in Deck)
        {
            if (keyValuePair.Value == 0)
            {
                continue;
            }
            else
            {
                CreateCard(keyValuePair.Key, IsPlayer, keyValuePair.Value);
            }
        }
    }
    void CreateCard(Card card, bool IsPlayer, int factor)
    {
        if (IsPlayer)
        {
            card.Properties["Owner"] = Player.player;
            GameObject CardGameObject = Instantiate(CardPrefab, DeckOfPlayer.transform);
            CardGameObject.GetComponent<GameCard>().Card = card;
            CardGameObject.GetComponent<GameCard>().UpdateProperties();
            factor--;
            if (factor > 0)
            {
                CreateCard(card, IsPlayer, factor);
            }
        }
        else
        {
            card.Properties["Owner"] = Player.opponent;
            GameObject CardGameObject = Instantiate(CardPrefab, DeckOfOpponent.transform);
            CardGameObject.GetComponent<GameCard>().Card = card;
            CardGameObject.GetComponent<GameCard>().UpdateProperties();
            factor--;
            if (factor > 0)
            {
                CreateCard(card, IsPlayer, factor);
            }
        }
    }

}
public enum Round
{
    Round_1, Round_2, Round_3
}

