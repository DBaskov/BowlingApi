# BowlingApi
This is a RESTful API that is meant to display player's score throughout the game on the display screen. The Data Model is organized
in a way of being able to track player score during game session, and the resource is under /players. The API has 5 calls:

POST: api/bowling/v1/players  [string playerName]
Coincides when player enter's name at the console or starts a new game. Creates a player resouce with unique ID, that's passed
back to the client.

POST: api/bowling/v1/players/{playerId}/calculateNewScore [int numberOfPins]
Used to track new score after pins been knocked down by the players. Calculates new score (Total, Result, and Running Total), and
returns that to the client as player resourrce

GET: api/bowling/v1/players/{playerId}
Used to get player's current score in the game

DELETE: api/bowling/v1/players/{playerId}
Deletes player resouce, coinsides with when player leaves bowling alley or finishes the game. 

PUT: api/bowling/v1/players/{playerId}
Replaces player resource, which can be used when player starts a new game (reset score to 0s)

Player resource model looks like this:
public class PlayerGameDataOut
    {
        public string PlayerId { get; set; }
        public string PlayerName { get; set; }
        public int TotalScore { get; set; }

        public List<int> RunningTotalList { get; set; }

        public List<List<int>> ResultList { get; set; }
    }
   
An Example of how API would be used in a bolwing Alley: Group of 4 players/individuals enter their names on console. 
The UI calls POST api/bowling/v1/players 4 times with each player's name, and gets their ID's back. During each player's turn, the bowling
system will call the API with POST: api/bowling/v1/players/{playerId}/calculateNewScore [int numberOfPins] call to calculate player's
score based on number of pins knocked down, and as result will get new score data in response. After last player completed scoring, the
bowling system will call DELETE: api/bowling/v1/players/{playerId} endpoint for each player's ID if they are done playing or 
call PUT: api/bowling/v1/players/{playerId} to reset everyones score to zeros, and start a new game using their same ids. 
