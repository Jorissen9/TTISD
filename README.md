# TTISD

Bjorn Jorissen, William Thenaers

------

## Assignment 1: Android geoTODO

Some screenshots from an emulator can be found in `TTISDassignment1\doc\`.

A video showing the execution on a real Android smart phone and a Samsung Gear Live is also inlcuded.

## Assignment 2: Interactive Room

Some screenshots and a video can be found in `TTISDassignment2\doc\`.

A release build is provided in `TTISDassignment2\bin\Release`.
Running it might require the Kinect SDK (v1.8).

### Gameplay

First, someone presses the `Start Calibration` button.

![1_Start_info_screen](TTISDassignment2\doc\1_Start_info_screen.png)

Four cyan square will appear in each corner of the game window. When starting calibration, only one will be shown at a time. The tracked skeleton position of one person will be shown on the left side of the above status window. The person should stand on a projected square, and pressing `Lock Point` will calibrate the game corner the person is standing on with their current Kinect skeleton position. After locking, the next square will be shown, until all points are locked.

![1_Start_info_screen](TTISDassignment2\doc\2_Game_calibration_points.png)

After all four corners are calibrated, a second person can join the Kinect viewing range and when the two people are being tracked (both XYZ positional values are being updated in the status screen), someone can press play and the game will start.

The purpose of the game is gathering as many points as possible by destroying the blocks in the middle, and when they are destroyed, players must compete against each other to bring the other's score to 0. The last player with a score greater than 0 wins.

![1_Start_info_screen](TTISDassignment2\doc\3_Game_start.png)

The colour of a block determines the score a player gets by destroying it (green gives 1 point, yellow 2, orange 3, up to magenta with 5 points). Hitting a block will transition it to a lower state, e.g. hitting a magenta block will give the player 5 points, then the block will turn red and will give 4 points on the next hit. A green block worth 1 point will disappear when hit. 

The last player that touched the ball will get the points. If a ball reaches the left or right boundary on a player's side, 5 points will be deducted from that player and the ball's position and speed will reset to the side the ball was lost in. The ball will start moving towards the other side. When a player hits the ball, its speed will slightly increase, while hitting a block will reset the speed modifier.

If all blocks are destroyed and one player loses all his points, then the other player wins. If both players lose all their points (by not preventing a ball from reaching their border), the game ends in a draw as seen below.

![1_Start_info_screen](TTISDassignment2\doc\4_Game_draw.png)