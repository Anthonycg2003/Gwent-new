using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] Text TextPlayerPower;
    [SerializeField] Text TextOpponentPower;
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
    void RefreshPower()
    {
        PlayerPower=FieldOfPlayer.GetComponent<Field>().GetPower();
        OpponentPower=FieldOfOpponent.GetComponent<Field>().GetPower();
        TextPlayerPower.text=PlayerPower.ToString();
        TextOpponentPower.text=OpponentPower.ToString();
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
            ActionText.Add_Action("Select a zone to summon the card");
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
                            ActionText.Add_Action("Melee Zone active");
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
                            ActionText.Add_Action("Range Zone active");
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
                            ActionText.Add_Action("Siege Zone active");
                            continue;
                        }
                }
            }
        }
        void PostSummon()
        {
            interpreter.Interpret(gameCard.Card);
            SyncZonesWithList();
            gameCard.InBattle = true;
            RefreshPower();
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

        }
        void SummonIn(Transform parent)
        {
            gameCard.gameObject.transform.SetParent(parent);
        }
        bool MaySummon()
        {
            if (gameCard.InBattle)
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
            interpreter.Interpret(gameCard.Card);
            SyncZonesWithList();
            RefreshPower();
            gameCard.InBattle = true;
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
        }
        void SummonIn(Transform parent)
        {
            gameCard.gameObject.transform.SetParent(parent);
        }
    }
    void SendToGraveyardAfterRound()
    {
        FieldOfPlayer.GetComponent<Field>().SendToGraveyard(GraveyardOfPlayer.transform);
        FieldOfOpponent.GetComponent<Field>().SendToGraveyard(GraveyardOfOpponent.transform);
    }
    void SyncZonesWithList()
    {
        foreach (ZoneInterface zone in Zones)
        {
            zone.SyncWithList();
            zone.UpdateCardsProperties();
        }
    }
    public void UpdateTurn()
    {
        if (IsPlayerTurn)
        {
            ActionText.Add_Action("Player Turn");
        }
        else
        {
            ActionText.Add_Action("Opponent Turn");
        }
    }
    public void PassTurn()
    {
        if (IsPlayerTurn)
        {
            IsPlayerTurn = false;
            PlayerPlayActualRound = false;
            ActionText.Add_Action("Player passed actual Round");
            if (BothPlayerPased())
            {
                PassRound();
            }
        }
        else
        {
            IsPlayerTurn = true;
            OpponentPlayActualRound = false;
            ActionText.Add_Action("Opponent passed actual Round");
            if (BothPlayerPased())
            {
                PassRound();
            }
        }
        bool BothPlayerPased()
        {
            return !(PlayerPlayActualRound || OpponentPlayActualRound);
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
        SendToGraveyardAfterRound();
        RefreshRound();
        RefreshPower();
        void RefreshRound()
        {
            Round=Rounds.Pop();
        }
        void FinishedGame(bool? IsPlayerWinner)
        {
            ActionText.Add_Action("Finished Game");
            if (IsPlayerWinner == null)
            {
                ActionText.Add_Action("Draw");
            }
            else if (IsPlayerWinner == false)
            {
                ActionText.Add_Action("Winner: Opponent");
            }
            else
            {
                ActionText.Add_Action("Winner: Player");
            }
        }
        void DecideRoundWinner()
        {
            if (PlayerPower > OpponentPower)
            {
                ActionText.Add_Action("Round Winner: Player");
                PlayerWinRounds++;
            }
            else if (OpponentPower > PlayerPower)
            {
                ActionText.Add_Action("Round Winner: Opponent");
                OpponentWinRounds++;
            }
            else
            {
                ActionText.Add_Action("Round Draw");
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

