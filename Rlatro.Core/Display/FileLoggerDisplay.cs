using Balatro.Core.Contracts.Display;
using Balatro.Core.GameEngine.Contracts;
using Balatro.Core.GameEngine.GameStateController;

namespace Balatro.Core.Display
{
    /// <summary>
    /// File logger display that wraps a StateCapturingDisplay to log game states to a file.
    /// Writes JSON game states to a specified file path for analysis and debugging.
    /// </summary>
    public class FileLoggerDisplay : IGameDisplay
    {
        private readonly StateCapturingDisplay StateCapturingDisplay;
        private readonly string FilePath;
        private readonly StreamWriter FileWriter;
        private readonly object FileLock = new object();
        private bool IsDisposed = false;

        /// <summary>
        /// Initializes a new instance of the FileLoggerDisplay with a specified file path.
        /// </summary>
        /// <param name="filePath">The path to the log file where game states will be written.</param>
        /// <param name="environmentId">The environment ID to pass to the wrapped StateCapturingDisplay.</param>
        public FileLoggerDisplay(string filePath, int environmentId = 0)
        {
            FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            StateCapturingDisplay = new StateCapturingDisplay(environmentId);
            
            // Create directory if it doesn't exist
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            // Open file for writing (append mode)
            FileWriter = new StreamWriter(filePath, append: true);
        }

        /// <summary>
        /// Gets the environment ID from the wrapped StateCapturingDisplay.
        /// </summary>
        public int GetEnvironmentId => StateCapturingDisplay.GetEnvironmentId;

        /// <summary>
        /// Gets whether the state is ready from the wrapped StateCapturingDisplay.
        /// </summary>
        public bool IsStateReady => StateCapturingDisplay.IsStateReady;

        /// <summary>
        /// Captures the game state using the wrapped StateCapturingDisplay and logs it to the file.
        /// Each state is written as a JSON line with a timestamp.
        /// </summary>
        /// <param name="gameContext">The current game context containing all game state.</param>
        /// <param name="currentState">The current phase state for specialized serialization.</param>
        public void DisplayGameState(GameContext gameContext, IGamePhaseState currentState)
        {
            // Capture state using the wrapped display
            StateCapturingDisplay.DisplayGameState(gameContext, currentState);
            
            // Log the captured state to file
            LogStateToFile();
        }

        /// <summary>
        /// Logs a message to the file with timestamp.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void DisplayMessage(string message)
        {
            LogToFile($"MESSAGE: {message}");
        }

        /// <summary>
        /// Logs an error message to the file with timestamp.
        /// </summary>
        /// <param name="errorMessage">The error message to log.</param>
        public void DisplayError(string errorMessage)
        {
            LogToFile($"ERROR: {errorMessage}");
        }

        /// <summary>
        /// Logs a clear action to the file.
        /// </summary>
        public void Clear()
        {
            LogToFile("CLEAR");
        }

        /// <summary>
        /// Gets the captured state from the wrapped StateCapturingDisplay.
        /// </summary>
        /// <returns>The captured game state as a JSON string.</returns>
        public string GetCapturedState() => StateCapturingDisplay.GetCapturedState();

        /// <summary>
        /// Logs the captured state to the file with timestamp.
        /// Thread-safe implementation using file lock.
        /// </summary>
        private void LogStateToFile()
        {
            if (IsDisposed) return;

            var capturedState = StateCapturingDisplay.GetCapturedState();
            if (string.IsNullOrEmpty(capturedState)) return;

            lock (FileLock)
            {
                try
                {
                    FileWriter.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] STATE: {capturedState}");
                    FileWriter.Flush(); // Ensure immediate write
                }
                catch (Exception ex)
                {
                    // Log error to console if file writing fails
                    Console.WriteLine($"FileLoggerDisplay: Failed to write to file {FilePath}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Logs a general message to the file with timestamp.
        /// Thread-safe implementation using file lock.
        /// </summary>
        /// <param name="message">The message to log.</param>
        private void LogToFile(string message)
        {
            if (IsDisposed) return;

            lock (FileLock)
            {
                try
                {
                    FileWriter.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}");
                    FileWriter.Flush(); // Ensure immediate write
                }
                catch (Exception ex)
                {
                    // Log error to console if file writing fails
                    Console.WriteLine($"FileLoggerDisplay: Failed to write to file {FilePath}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Disposes the file writer resources.
        /// Should be called when the display is no longer needed.
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed) return;

            lock (FileLock)
            {
                try
                {
                    FileWriter?.Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"FileLoggerDisplay: Error disposing file writer: {ex.Message}");
                }
                finally
                {
                    IsDisposed = true;
                }
            }
        }

        /// <summary>
        /// Finalizer to ensure file resources are cleaned up.
        /// </summary>
        ~FileLoggerDisplay()
        {
            Dispose();
        }
    }
}