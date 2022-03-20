#nullable enable
using System;
using static System.Console;

namespace Bme121
{
    record Player( string Colour, string Symbol, string Name );
    
    // The `record` is a kind of automatic class in the sense that the compiler generates
    // the fields and constructor and some other things just from this one line.
    // There's a rationale for the capital letters on the variable names (later).
    // For now you can think of it as roughly equivalent to a nonstatic `class` with three
    // public fields and a three-parameter constructor which initializes the fields.
    // It would be like the following. The 'readonly' means you can't change the field value
    // after it is set by the constructor method.
    
    //class Player
    //{
        //public readonly string Colour;
        //public readonly string Symbol;
        //public readonly string Name;
        
        //public Player( string Colour, string Symbol, string Name )
        //{
            //this.Colour = Colour;
            //this.Symbol = Symbol;
            //this.Name = Name;
        //}
    //}
    
    static partial class Program
    {
        // Display common text for the top of the screen.
        
        static void Welcome( )
        {
			WriteLine( "********** Welcome to Othello! **********" );
        }
        
        // Collect a player name or default to form the player record.
        
        static Player NewPlayer( string colour, string symbol, string defaultName )
        {
			Write("Please enter name [or <Enter> for name: '{0}']: ", defaultName);
			string name = ReadLine()!;
			if (name.Length == 0) name = defaultName;
			WriteLine("Player {0}, the {1} disc ({2}) represents your move in the game.", name, colour, symbol);			
			return new Player( colour, symbol, name );
        }
        
        // Determine which player goes first or default.
        
        static int GetFirstTurn( Player[ ] players, int defaultFirst )
        {
			Write("Who wants to go first(Enter player number (1 or 2) or <Enter> for random): ");
			string firstP = ReadLine()!;
			if(firstP.Length == 0)
			{
				WriteLine($"********** {players[defaultFirst].Name} goes first **********!");
				return defaultFirst;
			}
			int firstPlayer = int.Parse(firstP);			
			if(firstPlayer>2)
			{
				WriteLine($"********** Invalid player number entered, {players[defaultFirst].Name} goes first! **********");
				return defaultFirst;
			}
			WriteLine($"********** Great, player {players[firstPlayer - 1].Name} goes first! **********");			
            return firstPlayer - 1;
        }
        
        // Get a board size (between 4 and 26 and even) or default, for one direction.
        
        static int GetBoardSize( string direction, int defaultSize )
        {
			Write($"Please enter the number of {direction} or (<Enter> for default): ");
			string size = ReadLine()!;
			if(size.Length == 0) 
			{
				WriteLine($"********** Invalid Entry: Default size of {defaultSize} is applied! **********");
				return defaultSize;
			}
			int sizeInput = int.Parse(size);
			if(sizeInput < 4 || sizeInput >26 || sizeInput%2 != 0)
			{
				 Write($"********** The row size you entered was invalid, so default size of {defaultSize} is applied! **********\n");
				 return defaultSize;
			 }
			 return sizeInput;
		 }
		 
        // Get a move from a player.
        
        static string GetMove( Player player )
        {
			Write($"{player.Name}, make a move: ");
			string move = ReadLine()!;
            return move;
        }
        
        // Try to make a move. Return true if it worked.
        
        static bool TryMove( string[ , ] board, Player player, string move )
        {
			if(move == "skip") return true;
			
			// checking length of input
			if(move.Length != 2) 
			{
				WriteLine("********** Invalid Entry: Input must be two letters! **********");
				return false;
			}
			int row = IndexAtLetter(move[0].ToString()); // extracting the entry for row
			int col = IndexAtLetter (move[1].ToString()); // extracting entry for column
			
			int rowLimit = board.GetLength(0);
			int collLimit = board.GetLength(1);
			
			// check if input is in the correct range
			if(row == -1 || col == -1)
			{
				WriteLine("********** Invalid Entry! **********");
				return false;
			}
			
			// check if input is in the range
			if(row >= rowLimit || col >= collLimit)
			{
				WriteLine("********** Invalid Entry: Input out of grid! **********");
				return false;
			}
			// check for empty cells
			if(board[row,col] != " ") 
			{
				WriteLine("********** Invalid Entry: That cell is already filled! **********");
				return false;
			}		
			
			// array of bool to check cells in 8 directions around a cell and fill the list if opponent symbol is enclosed by the player symbol
			bool[] passedDirection = new bool [8];
			passedDirection[0] = TryDirection(board, player, row, -1, col, -1);// top left
			passedDirection[1] = TryDirection(board, player, row, -1, col, 0);// top
			passedDirection[2] = TryDirection(board, player, row, -1, col, 1);// top right
			passedDirection[3] = TryDirection(board, player, row, 0, col, 1);// right
			passedDirection[4] = TryDirection(board, player, row, 1, col, 1);// bottom right
			passedDirection[5] = TryDirection(board, player, row, 1, col, 0);// bottom
			passedDirection[6] = TryDirection(board, player, row, 1, col, -1);// bottom left
			passedDirection[7] = TryDirection(board, player, row, 0, col, -1);// left
			
			// return true for indexes at bool array
			foreach(bool direction in passedDirection)
			{
				if(direction == true) return true;
			}	
			WriteLine("********** Invalid Entry! **********");
			return false;
        }
        
        // Do the flips along a direction specified by the row and column delta for one step.
        
        static bool TryDirection( string[ , ] board, Player player,
            int moveRow, int deltaRow, int moveCol, int deltaCol )
        {
			int nextRow = moveRow + deltaRow;
			int  nextCol = moveCol + deltaCol;
			int symbolFlip = 1; // counter to check how many opponent symbols to flip
			bool validMove = false;
			// the while loop checks in the direction of the entered row and column + the adjacent cell for fliiping until the players symbol is found
			while(!validMove)
			{
				// check if cell on board
				if(nextRow < 0 || nextRow >= board.GetLength(0)-1) return false;						
				if(nextCol < 0 || nextCol>= board.GetLength(1)-1) return false;
				
				// check if cell is empty in the chain or has player symbol
				if(board[nextRow,nextRow] == " ") return false;
				if(board[nextRow,nextCol] != player.Symbol) symbolFlip++;
				
				else validMove = true;	// found my symbol ext while loop
				
				// look for next cell
				nextRow = nextRow + deltaRow;
				nextCol = nextCol + deltaCol;				
			}
			
			if(symbolFlip == 0)return false;		
			// look for next cell
			int nextR = moveRow + deltaRow;
			int nextC = moveCol + deltaCol;
			
			board[moveRow,moveCol] = player.Symbol;	
			
			// Loop to do flips
			for(int i = 0; i<symbolFlip; i++)
			{
				// do flip
				board[nextR,nextC] = player.Symbol;
				
				//change the cell in which we flip the symbol
				nextR = nextR + deltaRow;
				nextC = nextC + deltaCol;		
			}
			
            return true;
        }
        
        // Count the discs to find the score for a player.
        
        static int GetScore( string[ , ] board, Player player )
        {
			int score = 0; // counter
			
			// for loop to go through the board and look for player symbol
			for(int i = 0; i<board.GetLength(0);i++)
			{
				for(int j =0; j<board.GetLength(1);j++)
				{
					if(board[i,j] == player.Symbol) score++; // if player symbol found counter + 1
				} 
			}
            return score;
        }
        
        // Display a line of scores for all players.
        
        static void DisplayScores( string[ , ] board, Player[ ] players )
        {
			int score1 = GetScore(board, players[0]); 
			int score2 = GetScore(board, players[1]);
			
			WriteLine($"{players[0].Name} score: {score1} \t {players[1].Name} score: {score2} ");
        }
        
        // Display winner(s) and categorize their win over the defeated player(s).
        
        static void DisplayWinners( string[ , ] board, Player[ ] players)
        {
			// gets score for each player
			int score1 = GetScore(board, players[0]); 
			int score2 = GetScore(board, players[1]);
			
			// compare scores of each player and results
			WriteLine();
			WriteLine("********** GAME OVER **********	");
			WriteLine();
			WriteLine("********** FINAL SCORES & RESULTS **********");
			WriteLine();
			WriteLine($"{players[0].Name} score: {score1} \t {players[1].Name} score: {score2} ");
			if(score2>score1)WriteLine($"********** {players[0].Name} losses and {players[1].Name} wins! **********");
			else if(score1>score2)WriteLine($"********** {players[1].Name} looses and {players[0].Name} wins! **********");
			else if(score1 == score2) WriteLine($"********** Both players tie! **********");
		}        
        
        static void Main( )
        {
            // Set up the players and game.
            // Note: I used an array of 'Player' objects to hold information about the players.
            // This allowed me to just pass one 'Player' object to methods needing to use
            // the player name, colour, or symbol in 'WriteLine' messages or board operation.
            // The array aspect allowed me to use the index to keep track or whose turn it is.
            // It is also possible to use separate variables or separate arrays instead
            // of an array of objects. It is possible to put the player information in
            // global variables (static field variables of the 'Program' class) so they
            // can be accessed by any of the methods without passing them directly as arguments.
            
            Welcome( );
            
            Player[ ] players = new Player[ ] 
            {
                NewPlayer( colour: "black", symbol: "X", defaultName: "Black" ),
                NewPlayer( colour: "white", symbol: "O", defaultName: "White" ),
            };
            
            int turn = GetFirstTurn( players, defaultFirst: 0 );
           
            int rows = GetBoardSize( direction: "rows",    defaultSize: 8 );
            int cols = GetBoardSize( direction: "columns", defaultSize: 8 );
            
            string[ , ] game = NewBoard( rows, cols );
            
            // Play the game.
            
            bool gameOver = false;
            while( ! gameOver )
            {
                DisplayBoard( game ); 
                DisplayScores( game, players );
                
                string move = GetMove( players[ turn ] );
                if( move == "quit" ) 
                {
					DisplayWinners( game, players ); // shows results/winner
					WriteLine( );
					WriteLine("**********Have a great day!**********");
					gameOver = true;
				}
                else
                {
                    bool madeMove = TryMove( game, players[ turn ], move );
                    if( madeMove ) turn = ( turn + 1 ) % players.Length;
                    else 
                    {
                        Write( "********** Press <Enter> to try again. **********" );
                        ReadLine( ); 
                    }
                }
            }
        }
    }
}
