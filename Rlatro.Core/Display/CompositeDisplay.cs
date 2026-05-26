using Balatro.Core.Contracts.Display;
using Balatro.Core.GameEngine.Contracts;
using Balatro.Core.GameEngine.GameStateController;

namespace Balatro.Core.Display
{
    /// <summary>
    /// Composite display that manages multiple IGameDisplay implementations.
    /// Forwards all display calls to each registered display in sequence.
    /// Useful for combining different display behaviors (console + file logging, etc.).
    /// </summary>
    public class CompositeDisplay : IGameDisplay
    {
        private readonly List<IGameDisplay> Displays;
        private readonly object DisplaysLock = new object();

        /// <summary>
        /// Initializes a new instance of the CompositeDisplay with the provided displays.
        /// </summary>
        /// <param name="displays">The initial displays to include in the composite.</param>
        public CompositeDisplay(params IGameDisplay[] displays)
        {
            Displays = new List<IGameDisplay>(displays ?? Array.Empty<IGameDisplay>());
        }

        /// <summary>
        /// Initializes a new instance of the CompositeDisplay with a collection of displays.
        /// </summary>
        /// <param name="displays">The collection of displays to include in the composite.</param>
        public CompositeDisplay(IEnumerable<IGameDisplay> displays)
        {
            Displays = new List<IGameDisplay>(displays ?? Array.Empty<IGameDisplay>());
        }

        /// <summary>
        /// Gets the number of displays currently managed by this composite.
        /// </summary>
        public int DisplayCount
        {
            get
            {
                lock (DisplaysLock)
                {
                    return Displays.Count;
                }
            }
        }

        /// <summary>
        /// Adds a display to the composite.
        /// The display will receive all future display calls.
        /// </summary>
        /// <param name="display">The display to add.</param>
        /// <exception cref="ArgumentNullException">Thrown when display is null.</exception>
        public void AddDisplay(IGameDisplay display)
        {
            if (display == null) throw new ArgumentNullException(nameof(display));

            lock (DisplaysLock)
            {
                if (!Displays.Contains(display))
                {
                    Displays.Add(display);
                }
            }
        }

        /// <summary>
        /// Removes a display from the composite.
        /// The display will no longer receive display calls.
        /// </summary>
        /// <param name="display">The display to remove.</param>
        /// <returns>True if the display was found and removed, false otherwise.</returns>
        public bool RemoveDisplay(IGameDisplay display)
        {
            if (display == null) return false;

            lock (DisplaysLock)
            {
                return Displays.Remove(display);
            }
        }

        /// <summary>
        /// Removes all displays from the composite.
        /// </summary>
        public void ClearDisplays()
        {
            lock (DisplaysLock)
            {
                Displays.Clear();
            }
        }

        /// <summary>
        /// Forwards the DisplayGameState call to all managed displays.
        /// If any display throws an exception, it is logged to console but does not stop other displays.
        /// </summary>
        /// <param name="gameContext">The current game context containing all game state.</param>
        /// <param name="currentState">The current phase state for specialized serialization.</param>
        public void DisplayGameState(GameContext gameContext, IGamePhaseState currentState)
        {
            IGameDisplay[] displaysCopy;
            
            // Create a copy to avoid holding the lock during display operations
            lock (DisplaysLock)
            {
                displaysCopy = Displays.ToArray();
            }

            // Forward to all displays
            foreach (var display in displaysCopy)
            {
                try
                {
                    display.DisplayGameState(gameContext, currentState);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"CompositeDisplay: Error in display {display.GetType().Name}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Forwards the DisplayMessage call to all managed displays.
        /// If any display throws an exception, it is logged to console but does not stop other displays.
        /// </summary>
        /// <param name="message">The message to display.</param>
        public void DisplayMessage(string message)
        {
            IGameDisplay[] displaysCopy;
            
            // Create a copy to avoid holding the lock during display operations
            lock (DisplaysLock)
            {
                displaysCopy = Displays.ToArray();
            }

            // Forward to all displays
            foreach (var display in displaysCopy)
            {
                try
                {
                    display.DisplayMessage(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"CompositeDisplay: Error in display {display.GetType().Name}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Forwards the DisplayError call to all managed displays.
        /// If any display throws an exception, it is logged to console but does not stop other displays.
        /// </summary>
        /// <param name="errorMessage">The error message to display.</param>
        public void DisplayError(string errorMessage)
        {
            IGameDisplay[] displaysCopy;
            
            // Create a copy to avoid holding the lock during display operations
            lock (DisplaysLock)
            {
                displaysCopy = Displays.ToArray();
            }

            // Forward to all displays
            foreach (var display in displaysCopy)
            {
                try
                {
                    display.DisplayError(errorMessage);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"CompositeDisplay: Error in display {display.GetType().Name}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Forwards the Clear call to all managed displays.
        /// If any display throws an exception, it is logged to console but does not stop other displays.
        /// </summary>
        public void Clear()
        {
            IGameDisplay[] displaysCopy;
            
            // Create a copy to avoid holding the lock during display operations
            lock (DisplaysLock)
            {
                displaysCopy = Displays.ToArray();
            }

            // Forward to all displays
            foreach (var display in displaysCopy)
            {
                try
                {
                    display.Clear();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"CompositeDisplay: Error in display {display.GetType().Name}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Gets the first StateCapturingDisplay or FileLoggerDisplay found in the managed displays.
        /// Useful for accessing captured state from composite displays.
        /// </summary>
        /// <returns>The captured state from the first suitable display, or null if none found.</returns>
        public string? GetCapturedState()
        {
            lock (DisplaysLock)
            {
                // Look for StateCapturingDisplay first
                var stateDisplay = Displays.OfType<StateCapturingDisplay>().FirstOrDefault();
                if (stateDisplay != null)
                {
                    return stateDisplay.GetCapturedState();
                }

                // Look for FileLoggerDisplay second
                var fileDisplay = Displays.OfType<FileLoggerDisplay>().FirstOrDefault();
                if (fileDisplay != null)
                {
                    return fileDisplay.GetCapturedState();
                }

                return null;
            }
        }

        /// <summary>
        /// Disposes any disposable displays managed by this composite.
        /// Should be called when the composite is no longer needed.
        /// </summary>
        public void Dispose()
        {
            lock (DisplaysLock)
            {
                foreach (var display in Displays.OfType<IDisposable>())
                {
                    try
                    {
                        display.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"CompositeDisplay: Error disposing display {display.GetType().Name}: {ex.Message}");
                    }
                }
                
                Displays.Clear();
            }
        }
    }
}