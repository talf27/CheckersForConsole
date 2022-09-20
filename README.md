# Checkers as console application

Checkers game implemented in C# at .NET framework to console application.

-----

- pick a board size (6 x 6 , 8 x 8 , 10 x 10)
- enter your player's name
- choose playing against another player or against the computer

-----

The player makes his move by the format: COLrow>COLrow (for example in the picture below: Dg>Ef).

<img src="Damka console screenshot.jpg" width=400>

The O's kings are marked with 'Q', and The X's kings are marked with Z.\
At any point, the user can quit the game by entering 'Q' instead of a valid move.

-----

Each round is over when there are no more checkers on board for some player,\
or when there are no more legal moves for both players.

score is calculated in each round's end as the difference between the players' remaining checkers on board\
(regular checker = 1 points ; king = 4 points) and is granted to the winner,\
or to both players if round ends with a tie.

-----

Enjoy the game! :)
