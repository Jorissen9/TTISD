# TTISD Project

Bjorn Jorissen, William Thenaers

------

# Documentation
Our idea is to embed a boardgame that has complexer rules with technology to decrease the knowledge needed to play this game. 
Our goal was to make a framework that can be used in all boardgames with a similar style. Right now it could be used with all boardgames which have a linear pathway and the design we have right now can be used for all boardgames with 40 squares.
This framework would combine RFID tags to determine which piece had moved and LIDAR to determine where it moved to. This idea changed during our research and experiments.
We used monopoly as an example, we played it feature complete and there are a lot of rules that regular players never use while playing with the physical boardgame but these rules make the game more interesting.

## Our idea's:

### Multiple scanners
Our first idea was to work with multiple scanners and RFID tags on every pawn on the board game. With this we could make an triangular calculation with the signal strength of each RFID tag to determine the location. 
In the feedback for this idea we understood that you would need an MCU with an RFID reader for every single scanner which would turn out to be way too expensive.
We also got the information that a simultaneous RFID reader is available here at EDM. This reader should be able to read multiple tags at once which we were going to test out next.

### Multiple tags with one scanner
Our idea was to create a grid of RFID tags and attach a signal blocker to each pawn. With this information we could then know which positions of the board are blocked and thus where all paws are standing. 
For this to work we need a reliable read, which we tested next.
So the next thing we did was test the available RFID reader that could read multiple tags simultaneously, according to the spec sheets this reader is able to read 100 tags at once.

So we tried it and even with a very simple setup and the results are pretty bad. Bad CRC shows that the received signal is not reliable enough to ensure a correct received value.
The results show that most of the time only 2 different tags are detected, with the addition of tags in front of the reader this doesn't increase, it makes it even worst.
We tried ignoring the bad CRC and just taking the unique epc values but with this we got up to 10+ different epc values in a matter of seconds and stopped this test immediately.

This test resulted that the RFID reader is way to unreliable for even a simple setup so let alone a board filled with even more tags.
We did some research why this was the case because reviews of the reader are good and a lot of people have shown a working example. 
The tags are Ultra-High Frequency tags, so these should avoid interference with other tags according to our research. 
These tags are specially made to not interfere with other tags.
During this research we saw that the examples people had shown mostly had an external power supply so we tried adding a power supply as well because we only used power over USB before this. 
But the results were not better than before we attached the power supply.
During the changes and research we also tried changing the code a few times and reading the tags differently. But this didn't result in major changes, we made some improvements in readability so we better understood what was being read but that was all that gave better results. 
So most of our efforts were in vain.
We also noticed that the examples had completely different use cases which only required to read each tag once. In all use cases for RFID the tags are moving, like using RFID tags to track inventory items from a warehouse. 
So we tried putting the tags out of the scanners range and back in the scanners range. This resulted in always detecting the tags. 
Even just moving the tags in front of the scanner resulted in better output. An RFID tag draws energy from Radio Frequency waves, we think that if these stay stationary this energy may be insufficient for the RFID tag to sent a reliable and complete signal.

### LIDAR
While working on the RFID tags we also experimented with the LIDAR.

The Velodyne LIDAR has a minimum range of 1 meter and a maximum range of 100 meters. 
This is good for outdoors, which the LIDAR is designed for, but not really for our idea. 
We could change our idea, but at this stage we still though to combine the RFID tags and the LIDAR on a single boardgame. 
So we looked into different LIDAR's and found a cheaper developer edition with a minimum range of 20 centimeters. This one was ordered and delivered very quickly.

With the use of this LIDAR we made a program were you can set the amount of cells the boardgame has, the minimum range to the cells and the maximum range which is the distance from the LIDAR to the end of the cells. This can be calibrated in the game itself.
With this program we could determine which cells were occupied and pass this information to the game itself. From there we can then determine which cell position has moved (based on the previous received array) and how far is had moved.
We also noticed very quickly that the LIDAR had a minimum range of 12 centimeters instead of 20 centimeters.

We then found and adjusted a open source monopoly game. It turns out that finding a feature complete monopoly game that is also open source is non existent. 
Our original goal for using technology to make the complex features easier is not achieved because of this implementation only has the simple features implemented. 
To completely implement the game is kinda out of the scope of this project, the important part is using the LIDAR and plugging it into the game. If the game was feature complete we only needed to change the 'rollDice' function in order to make it work. 
Right now we even added a history function because we didn't even knew what was happened during a turn.


### Future work
The next idea is to make the game multiplayer, this gives the option to play on a physical boardgame to people all over the world. The difficulty here would be checking if the player is not cheating, right now you could easily set your piece wherever you want and then the game will just assume your new position as your actual position.
At the moment it's not that easy to plug the software we created for the lidar in any boardgame because you need to include the full sdk to communicatie with the LIDAR. After this is included you can set some parameters according to the board you printed or drew. 
We could make this process even easier by creating the communication through COM ports or something similar.
Because of the way the design if right now we could easily make more games using the exact same setup therefore adding more games would be an easy next step. If the game has a different amount of squares all you need to do is change the board but everything else can be re-used.

----
LIDAR?
------

### RPLIDAR A1
A devkit LIDAR from [Slamtec](https://www.slamtec.com/en/Lidar/A1).
[Digikey dev kit](https://www.digikey.be/product-detail/nl/DFR0315/1738-1402-ND/7597150/?itemSeq=290922675)

[Possible Python lib](https://github.com/SkoltechRobotics/rplidar)

RFID
-----
[RFID ULTRA SMALL TAGS](https://www.digikey.be/products/nl?keywords=490-16515-1-ND%20)

[RFID Antenna](https://www.digikey.be/products/nl?keywords=535-12930-ND%20)

[Arduino lib](https://github.com/sparkfun/SparkFun_Simultaneous_RFID_Tag_Reader_Library)
[ArduinoSTL lib](https://github.com/mike-matera/ArduinoSTL)
