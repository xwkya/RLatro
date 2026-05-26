using System.Runtime.CompilerServices;
using System.Text;
using Balatro.Core.Contracts.Display;
using Balatro.Core.CoreObjects.Cards.CardObject;
using Balatro.Core.GameEngine.Contracts;
using Balatro.Core.GameEngine.GameStateController;
using Balatro.Core.GameEngine.GameStateController.PhaseStates;
using Balatro.Core.GameEngine.StateController;

namespace Balatro.Core.Display
{
    /// <summary>
    /// High-performance state capturing display optimized for RL environments.
    /// Captures and serializes game state based on the current phase with minimal allocations.
    /// Each instance is connected 1:1 with a game controller.
    /// </summary>
    public class StateCapturingDisplay : IGameDisplay
    {
        private readonly StringBuilder StringBuilder;
        private readonly int EnvironmentId;
        private volatile bool StateReady;
        
        /// <summary>
        /// The last captured state as JSON string.
        /// </summary>
        private string LastCapturedState = string.Empty;

        /// <summary>
        /// Initializes a new instance of the StateCapturingDisplay for the specified environment.
        /// </summary>
        /// <param name="environmentId">The unique identifier for this environment instance.</param>
        public StateCapturingDisplay(int environmentId)
        {
            EnvironmentId = environmentId;
            StringBuilder = new StringBuilder(2048); // Pre-allocate reasonable capacity
        }

        /// <summary>
        /// Gets the environment ID associated with this display.
        /// </summary>
        public int GetEnvironmentId => EnvironmentId;

        /// <summary>
        /// Gets a value indicating whether the state has been captured and is ready for retrieval.
        /// </summary>
        public bool IsStateReady => StateReady;

        /// <summary>
        /// Captures the current game state based on the active phase.
        /// This is the main entry point called by the game controller.
        /// </summary>
        /// <param name="gameContext">The current game context containing all game state.</param>
        /// <param name="currentState">The current phase state for specialized serialization.</param>
        public void DisplayGameState(GameContext gameContext, IGamePhaseState currentState)
        {
            CapturePhaseSpecificState(gameContext, currentState);
            StateReady = true;
        }

        /// <summary>
        /// No-op implementation for performance - messages are not needed for RL environments.
        /// </summary>
        /// <param name="message">The message to display (ignored).</param>
        public void DisplayMessage(string message) { /* No-op for performance */ }

        /// <summary>
        /// No-op implementation for performance - errors are not displayed in RL environments.
        /// </summary>
        /// <param name="errorMessage">The error message to display (ignored).</param>
        public void DisplayError(string errorMessage) { /* No-op for performance */ }

        /// <summary>
        /// No-op implementation for performance - clearing is not needed for RL environments.
        /// </summary>
        public void Clear() { /* No-op for performance */ }

        /// <summary>
        /// Gets the captured state as a JSON string.
        /// Should be called after DisplayGameState to retrieve the serialized state.
        /// </summary>
        /// <returns>The captured game state as a JSON string.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetCapturedState() => LastCapturedState;

        /// <summary>
        /// Captures game state optimized per game phase for maximum performance.
        /// Uses phase-specific serialization to include only relevant data for each phase.
        /// </summary>
        /// <param name="gameContext">The current game context.</param>
        /// <param name="currentState">The current phase state.</param>
        private void CapturePhaseSpecificState(GameContext gameContext, IGamePhaseState currentState)
        {
            StringBuilder.Clear();

            // Switch on phase for optimized serialization
            switch (currentState.Phase)
            {
                case GamePhase.Round:
                    CaptureRoundState(gameContext, currentState as RoundState);
                    break;
                case GamePhase.Shop:
                    CaptureShopState(gameContext, currentState as ShopState);
                    break;
                case GamePhase.BlindSelection:
                    CaptureBlindSelectionState(gameContext, currentState as BlindSelectionState);
                    break;
                case GamePhase.ArcanaPack:
                case GamePhase.JokerPack:
                case GamePhase.PlanetPack:
                case GamePhase.SpectralPack:
                case GamePhase.CardPack:
                    CapturePackState(gameContext, currentState);
                    break;
                default:
                    CaptureBasicState(gameContext, currentState);
                    break;
            }

            LastCapturedState = StringBuilder.ToString();
        }

        /// <summary>
        /// Captures round-specific state including hands, discards, scores, and cards.
        /// This is the most critical phase for RL training as it contains action decisions.
        /// </summary>
        /// <param name="gameContext">The current game context.</param>
        /// <param name="roundState">The round state containing hands, discards, and scoring info.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CaptureRoundState(GameContext gameContext, RoundState? roundState)
        {
            StringBuilder.Append("{\"phase\":\"Round\"");
            AppendBasicGameData(gameContext);
            
            if (roundState != null)
            {
                StringBuilder.Append(",\"hands\":").Append(roundState.Hands);
                StringBuilder.Append(",\"discards\":").Append(roundState.Discards);
                StringBuilder.Append(",\"currentScore\":").Append(roundState.CurrentChipsScore);
                StringBuilder.Append(",\"requiredScore\":").Append(roundState.CurrentChipsRequirement);
                StringBuilder.Append(",\"isPhaseOver\":").Append(roundState.IsPhaseOver ? "true" : "false");
            }

            AppendHandCards(gameContext);
            AppendPlayedCards(gameContext);
            AppendJokersAndConsumables(gameContext);
            StringBuilder.Append('}');
        }

        /// <summary>
        /// Captures shop-specific state including roll costs, item counts for action space calculation.
        /// </summary>
        /// <param name="gameContext">The current game context.</param>
        /// <param name="shopState">The shop state containing purchasable items.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CaptureShopState(GameContext gameContext, ShopState? shopState)
        {
            StringBuilder.Append("{\"phase\":\"Shop\"");
            AppendBasicGameData(gameContext);
            
            if (shopState != null)
            {
                StringBuilder.Append(",\"rollPrice\":").Append(shopState.RollPrice());
                StringBuilder.Append(",\"freeRolls\":").Append(shopState.NumberOfFreeRolls);
                
                // Shop items count for action space
                StringBuilder.Append(",\"shopItemsCount\":").Append(shopState.ShopContainer?.Items?.Count ?? 0);
                StringBuilder.Append(",\"boosterPacksCount\":").Append(shopState.BoosterContainer?.BoosterPacks?.Count ?? 0);
                StringBuilder.Append(",\"vouchersCount\":").Append(shopState.VoucherContainer?.Vouchers?.Count ?? 0);
            }

            AppendJokersAndConsumables(gameContext);
            StringBuilder.Append('}');
        }

        /// <summary>
        /// Captures blind selection state with available blind options.
        /// </summary>
        /// <param name="gameContext">The current game context.</param>
        /// <param name="blindState">The blind selection state.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CaptureBlindSelectionState(GameContext gameContext, BlindSelectionState? blindState)
        {
            StringBuilder.Append("{\"phase\":\"BlindSelection\"");
            AppendBasicGameData(gameContext);
            
            if (blindState != null)
            {
                // Add blind selection specific data if needed
                StringBuilder.Append(",\"availableBlinds\":3"); // Typically 3 blinds available
            }

            AppendJokersAndConsumables(gameContext);
            StringBuilder.Append('}');
        }

        /// <summary>
        /// Captures pack opening state for booster pack phases.
        /// </summary>
        /// <param name="gameContext">The current game context.</param>
        /// <param name="packState">The pack state containing selectable cards/items.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CapturePackState(GameContext gameContext, IGamePhaseState packState)
        {
            StringBuilder.Append("{\"phase\":\"").Append(packState.Phase).Append('"');
            AppendBasicGameData(gameContext);
            
            // Pack states typically have cards to choose from
            StringBuilder.Append(",\"packType\":\"").Append(packState.Phase).Append('"');
            
            AppendJokersAndConsumables(gameContext);
            StringBuilder.Append('}');
        }

        /// <summary>
        /// Captures basic state for unknown or unsupported phases.
        /// </summary>
        /// <param name="gameContext">The current game context.</param>
        /// <param name="currentState">The current phase state.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CaptureBasicState(GameContext gameContext, IGamePhaseState currentState)
        {
            StringBuilder.Append("{\"phase\":\"").Append(currentState.Phase).Append('"');
            AppendBasicGameData(gameContext);
            AppendJokersAndConsumables(gameContext);
            StringBuilder.Append('}');
        }

        /// <summary>
        /// Appends core game data that's relevant across all phases.
        /// Includes round, ante, gold, game over status, and container sizes.
        /// </summary>
        /// <param name="gameContext">The current game context.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AppendBasicGameData(GameContext gameContext)
        {
            StringBuilder.Append(",\"round\":").Append(gameContext.PersistentState.Round);
            StringBuilder.Append(",\"ante\":").Append(gameContext.PersistentState.Ante);
            StringBuilder.Append(",\"gold\":").Append(gameContext.PersistentState.EconomyHandler.GetCurrentGold());
            StringBuilder.Append(",\"isGameOver\":").Append(gameContext.IsGameOver ? "true" : "false");
            StringBuilder.Append(",\"deckSize\":").Append(gameContext.Deck.Count);
            StringBuilder.Append(",\"discardPileSize\":").Append(gameContext.DiscardPile.Count);
        }

        /// <summary>
        /// Appends the current hand cards in compact format for fast serialization.
        /// Uses direct Span access for zero-copy enumeration.
        /// </summary>
        /// <param name="gameContext">The current game context.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AppendHandCards(GameContext gameContext)
        {
            StringBuilder.Append(",\"handCards\":[");
            var handSpan = gameContext.Hand.Span;
            
            for (int i = 0; i < handSpan.Length; i++)
            {
                if (i > 0) StringBuilder.Append(',');
                AppendCardData(handSpan[i]);
            }
            
            StringBuilder.Append(']');
        }

        /// <summary>
        /// Appends the currently played cards in compact format.
        /// Used to track what cards are being played in the current hand.
        /// </summary>
        /// <param name="gameContext">The current game context.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AppendPlayedCards(GameContext gameContext)
        {
            StringBuilder.Append(",\"playedCards\":[");
            var playedSpan = gameContext.PlayContainer.Span;
            
            for (int i = 0; i < playedSpan.Length; i++)
            {
                if (i > 0) StringBuilder.Append(',');
                AppendCardData(playedSpan[i]);
            }
            
            StringBuilder.Append(']');
        }

        /// <summary>
        /// Appends compact card data using numeric enums for maximum performance.
        /// Includes card ID, rank, suit, enhancement, edition, and seal as bytes.
        /// </summary>
        /// <param name="card">The card to serialize.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AppendCardData(Card64 card)
        {
            // Compact card representation for performance
            StringBuilder.Append('{');
            StringBuilder.Append("\"id\":").Append(card.Id);
            StringBuilder.Append(",\"rank\":").Append((byte)card.GetRank());
            StringBuilder.Append(",\"suit\":").Append((byte)card.GetSuit());
            StringBuilder.Append(",\"enh\":").Append((byte)card.GetEnh());
            StringBuilder.Append(",\"edition\":").Append((byte)card.GetEdition());
            StringBuilder.Append(",\"seal\":").Append((byte)card.GetSeal());
            StringBuilder.Append('}');
        }

        /// <summary>
        /// Appends joker and consumable counts for action space calculation.
        /// These counts help determine available actions (sell joker, use consumable).
        /// </summary>
        /// <param name="gameContext">The current game context.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AppendJokersAndConsumables(GameContext gameContext)
        {
            StringBuilder.Append(",\"jokerCount\":").Append(gameContext.JokerContainer.Jokers.Count);
            StringBuilder.Append(",\"consumableCount\":").Append(gameContext.ConsumableContainer.Consumables.Count);
        }
    }
}