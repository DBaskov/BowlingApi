# BowlingApi
This is a RESTful API that is meant to display player's score throughout the game on the display screen. The Data Model is organized
in a way of being able to track player score during game session, and the resource is under /player-game-sessions. The API has 5 calls:

POST: api/bowling/v1/player-game-sessions  [string playerName]
Coincides when player enter's name at the console or starts a new game. Creates a player game session resouce with unique ID, that's passed back to the client.

POST: api/bowling/v1/player-game-sessions/{Id}/calculate-new-score [int numberOfPins]
Used to track new score after pins been knocked down by the players. Calculates new score (Total, Result, and Running Total), and
returns that to the client as player game session resource.

GET: api/bowling/v1/player-game-sessions/{Id}
Used to get player's current game session

DELETE: api/bowling/v1/player-game-sessions/{Id}
Deletes player's game session resouce, coinsides with when player leaves bowling alley or finishes the game. 

PUT: api/bowling/v1/player-game-sessions/{Id}
Replaces player game session resource, which can be used when player starts a new game (reset score to 0s)

Player Game Session resource model looks like this:
public class PlayerGameSessionOut
    {
        public string PlayerGameSessionId { get; set; }
        public string PlayerName { get; set; }
        public int TotalScore { get; set; }

        public List<int> RunningTotalList { get; set; }

        public List<List<int>> ResultList { get; set; }
    }
   
An Example of how API would be used in a bolwing Alley: Group of 4 players/individuals enter their names on console. 
The UI calls POST api/bowling/v1/player-game-sessions 4 times with each player's name, and gets their ID's back. During each player's turn, the bowling system will call the API with POST: api/bowling/v1/player-game-sessions/{Id}/calculate-new-score [int numberOfPins] call to calculate player's
score based on number of pins knocked down, and as result will get new player game session (new score data) in response. After last player completed scoring, the bowling system will call DELETE: api/bowling/v1/player-game-sessions/{Id} endpoint for each player's ID if they are done playing or 
call PUT: api/bowling/v1/player-game-sessions/{Id} to reset everyones score to zeros, and start a new game using their same ids. 
