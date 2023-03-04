// Name:        Angel Mario Colorado Chairez
// Modified:    January 29, 2022
// Description: Tic Tac Toe game using the game theory (Minimax Algorithm) to be unbeatable

// This game uses the same logic as the solution in Python presented by Kylie Ying on her youtube channel:
//      https://youtu.be/fT3YWCKvuQE


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace TicTacToe
{
    public sealed partial class MainPage : Page
    {
        // Declaration of variables to ensure proper scope
        string whoStarts = "X";                 // 'X' will always be the human and 'O' can be the 2nd human or the AI
        string playerTurn = "X";                // 'X' when the App loads X always will start
        int winsX = 0;                          // Holds the count of times 'X' have won
        int winsO = 0;                          // Holds the count of times 'O' have won
        int scratch = 0;                        // Counts the number of Scratch games
        int turnNum = 0;                        // Counts the number of turns
        string[] arrayMoves = new string[9];    // Declaration of an array to store the moves
        Button[] myArrayBtn = new Button[9];    // Declaration of an array to hold the 9 buttons
        bool isAiEnabled;                       // A flag to indicate if the AI is enabled
        // Array that holds the possible combinations to win
        int[] winCombinations = new int[24] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 0, 3, 6, 1, 4, 7, 2, 5, 8, 0, 4, 8, 2, 4, 6 };

        const int position = 0;                 // Constant used in the MiniMax function
        const int score = 1;                    // Constant used in the MiniMax function

        public MainPage()
        {
            this.InitializeComponent();
            txtStatus.Text = $"Player '{playerTurn}' it's your turn...";
            myArrayBtn[0] = btn00;              // Declaration of the buttons into the array
            myArrayBtn[1] = btn01;
            myArrayBtn[2] = btn02;
            myArrayBtn[3] = btn03;
            myArrayBtn[4] = btn04;
            myArrayBtn[5] = btn05;
            myArrayBtn[6] = btn06;
            myArrayBtn[7] = btn07;
            myArrayBtn[8] = btn08;
        }

        private void Cell_Click(object sender, RoutedEventArgs e)
        {
            if (turnNum == 0)                               // If it's the first turn
            {
                chkAI.IsEnabled = false;                    // Disables the check box
                if (chkAI.IsChecked == true)
                    isAiEnabled = true;                     // Enables the 'AI' for the second move
            }
            Button tempBtn = (Button)sender;                // Casts the 'sender' into a button object
            tempBtn.IsEnabled = false;                      // Disables the button that generated this Event
            tempBtn.Content = playerTurn;                   // Places the mark of the player on the chosen button

            int butPressed = int.Parse(Regex.Match(tempBtn.Name, @"\d+").Value);    // Gets the number of the button and parses it into a integer
            RecordMove(butPressed);                         // Records the requested move, sends the number of the button
            turnNum++;                                      // After the move, increases the counter

            if (CheckStatus())                              // 'CheckStatus' checks if somebody won or if it's a scratch game, otherwise toggles the 'turn'
                return;                                     // If the previous if returned 'true' it means the game is over, therefore ends the current Event-Method
                                                            // Otherwise continues the game
            if (isAiEnabled == true)                        // If 'AI' is enabled
                AiRoutine();                                // Calls the 'AiRoutine' to make a move

        }
        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            WhoStartsToggle();
            Array.Clear(arrayMoves, 0, arrayMoves.Length);  // Reset the array with the previous moves
            turnNum = 0;                                    // Restarts that counter
            DisEnableButtons(true);                         // Enable all the buttons

            if (whoStarts == "O")                           // If "O" (2nd human or AI) will start the next game
            {
                chkAI.IsEnabled = false;                    // Disables the check box
                if (chkAI.IsChecked == true)
                {
                    isAiEnabled = true;                     // Enables the 'AI' only before the first move
                    AiRoutine();                            // Calls the 'AiRoutine' to make a move
                }
            }
        }
        private void RecordMove(int but)
        {
            arrayMoves[but] = playerTurn;                   // Records the value on the array
        }

        private bool CheckStatus()                          // This method check the status of the game
        {
            bool gameOver = false;                          // A flag to indicate if the game is over
            if (SmbdyWin(playerTurn))                       // Calls the 'SmbdyWin' method to check if the current player won
            {
                if (isAiEnabled && playerTurn == "O")
                    txtStatus.Text = $"The AI wins !!!";    // Updates the text box
                else
                    txtStatus.Text = $"{playerTurn} wins !!!";
                if (playerTurn == "X")
                    winsX++;                                // Increases the count of games won for 'X'
                else
                    winsO++;                                // Increases the count of games won for 'X'
                DisEnableButtons(false);                    // Calls a method to disable the '9 buttons'
                gameOver = true;                            // Sets the gameOver flag
            }
            else if (turnNum == 9)                          // If it's the ninth turn
            {
                Scratch();                                  // Calls the 'Scratch' method
                gameOver = true;                            // Sets the gameOver flag
            }
            else
            {
                TurnToggle();                               // Toggles the turn to the next player
            }

            if (gameOver)                                   // If the game is over
            {
                RefreshDisplay();                           // Calls a method to refresh the counters
                isAiEnabled = false;                        // Disables the 'AI'
                chkAI.IsEnabled = true;                     // Enables the check box
            }
            return gameOver;
        }

        private bool SmbdyWin(string playerToTest)          // This methods verifies if a player won in the current turn
        {
            for (int i = 0; i < 23; i = i + 3)              // Loop through the table (array) checking the winning combinations
                if (arrayMoves[winCombinations[i]] == playerToTest && arrayMoves[winCombinations[i + 1]] == playerToTest && arrayMoves[winCombinations[i + 2]] == playerToTest)
                    return true;                            // There is a winner
            return false;                                   // No matches were found
        }

        private void Scratch()                              // Method called when there is a Scratch
        {
            scratch++;                                      // Increases the counter
            txtStatus.Text = "Nobody won !";                // Updates the text box
        }
        private void TurnToggle()
        {
            if (playerTurn == "X")                          // Toggles the turn
                playerTurn = "O";
            else
                playerTurn = "X";

            txtStatus.Text = $"Player '{playerTurn}' it's your turn...";
        }

        private void RefreshDisplay()                       // Method that refreshes the counters
        {
            txtXWins.Text = $"Player 'X' Wins: {winsX}";
            txtOWins.Text = $"Player 'O' Wins: {winsO}";
            txtScratch.Text = $"Scratch Games: {scratch}";
        }

        private void WhoStartsToggle()                      // Method that toggles who starts on each game
        {
            if (whoStarts == "X")                           // Toggles the turn
                whoStarts = "O";
            else
                whoStarts = "X";
            playerTurn = whoStarts;
            txtStatus.Text = $"Player '{playerTurn}' it's your turn...";
        }

        private void DisEnableButtons(bool status)          // Method that enables or disables the 9 buttons
        {
            foreach (Button btnXX in myArrayBtn)            // Loop that traverses the array of buttons
            {
                btnXX.IsEnabled = status;                   // Enables or disables the current button 
                if (status)                                 // If the flag passed is true
                    btnXX.Content = "?";                    // Loads the "?" character in the current button
            }
        }

        private void AiRoutine()                            // This routine calls methods used for the AI
        {
            int moveTemp = AiMiniMax("O")[position];        // Gets a move (position) from the MiniMax algorithm. The [score] is irrelevant in this point
            AiMakeMove(moveTemp);
            turnNum++;                                      // After the move, increases the counter
            CheckStatus();                                  // 'CheckStatus' checks if somebody won or if it's a scratch game,
        }                                                   //  otherwise toggles the 'turn'

        private int[] AiMiniMax(string player)              // This method uses the minimax algorithm
        {                                                   // Returns 2 parameters (position and score)
            string maxPlayer = "O";                         // 'O' is the AI
            string otherPlayer;                             // Creates a variable to switch the players on each iteration of this method

            if (player == "X")                              // Toggles the players
                otherPlayer = "O";
            else
                otherPlayer = "X";

            //--------------------------------------------------------------
            // This section ends the recursion of the 'AiMiniMax' method
            // When somebody wins or there are no more Cells left, it returns a score for that calculated branch
            // The returned position at this point is irrelevant
            if (SmbdyWin(otherPlayer))                      // If the previous calculated move, generates a win for the previous player
            {
                if (otherPlayer == maxPlayer)               // If the winner is the maxPlayer ("O")
                    return new int[] { 410, 1 * (EmptyCells() + 1) };   // Returns a positive value as the score
                else                                        // If the winner is the human ("X")
                    return new int[] { 420, -1 * (EmptyCells() + 1) };  // Returns a negative value as the score
            }

            if (EmptyCells() == 0)                          // If there are no empty cells
                return new int[] { 430, 0 };                // Returns the score of '0' = Scratch game
                                                            //--------------------------------------------------------------

            int[] best = new int[2];                        // This array stores the best [position] and [score]

            if (player == maxPlayer)                        // If the current 'player' of this iteration is the AI ('O')
            {
                best[position] = 100;                       // This value is useless at this point
                best[score] = int.MinValue;                 // Assigns a min value as the best, in order to maximize the score
            }
            else                                            // If the current 'player' of this iteration is the Human ('X')
            {
                best[position] = 200;                       // This value is useless at this point
                best[score] = int.MaxValue;                 // Assigns a max value as the best, in order to minimize the score
            }

            for (int i = 0; i < arrayMoves.Length; i++)     // Verifies all possible moves
            {
                if (arrayMoves[i] == null)                  // If it found a possible (available) move
                {
                    arrayMoves[i] = player;                 // It does the move and loads 'X' or 'O' (depending who's the current player) in the array of moves
                    int[] simulated = AiMiniMax(otherPlayer);   // Calls the recursive 'AiMiniMax' method and waits for an array of 2 variables [position, score]

                    // The previous line is the recursive call. It'll simulate an entire game (branch). It will finish when
                    // it finds a winner or there are no more available moves.

                    // The for loop will go deep to the last move of the branch with the recursive AiMiniMax(), and
                    // it will go up until it returns, while it finds the best path of that branch.
                    // It will try the others branch the same way. Practically it will try all possible combinations.

                    arrayMoves[i] = null;                   // Undoes the move
                    simulated[position] = i;                // Possible move. Overwrites the position returned

                    if (player == maxPlayer)                // If the current player is the AI ("O")
                    {
                        if (simulated[score] > best[score]) // If the simulated score is greater than the current score
                        {
                            best[score] = simulated[score]; // Assigns the 'simulated' values to the current values 'best'
                            best[position] = simulated[position];
                        }
                    }
                    else                                    // If the current player is the Human ("X")
                    {
                        if (simulated[score] < best[score]) // If the simulated score is lower than the current score
                        {
                            best[score] = simulated[score]; // Assigns the 'simulated' values to the current values 'best'
                            best[position] = simulated[position];
                        }
                    }
                }                                           // End of 'if (arrayMoves[i] == null)'
            }                                               // End of the 'recursive for loop'
            return best;                                    // End of AiMiniMax(), It returns the best[score] and the best[position]
        }

        private int EmptyCells()                            // This method verifies the number of empty cells on the board
        {
            int empty = 0;
            for (int i = 0; i < arrayMoves.Length; i++)
            {
                if (arrayMoves[i] == null)
                    empty++;
            }
            return empty;
        }

        private void AiMakeMove(int moveVar)
        {
            arrayMoves[moveVar] = "O";                      // Loads 'O' in the array of moves
            myArrayBtn[moveVar].IsEnabled = false;          // Disables the button
            myArrayBtn[moveVar].Content = "O";              // Loads 'O' in the button
        }

    }
}
