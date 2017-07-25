EXTERNAL setFlag(x)

//For editor testing
Where to start
+ [Belle] -> belle_start
+ [Clover] -> clover_start

== belle_start ==
{conversation<1:->conversation|@Belle: Good luck.}

== clover_start == 
{conversation<1:->conversation|@Clover: Don't die!}
        
== conversation ==
@Clover: Look sis! Its the guy who found me!
@Belle: I must thank yo- Oh, it's you!
@Clover: Oh you know this guy, Belle? 
@Belle: He's the one who saved me from those creatures.
@Clover: Oh him! You haven't stopped goin- Oww! 
@Belle: Quiet Clover!
* [Well I am the only guy here...]
    @Belle: Yes...but umm...well. Thank you anyway...mister...
*    * [Theodore. I remember my name now!] 
        @Belle: Ah. Yes. Thank you Theo.
        @Clover: Let's call him Todo!
        -> END
* [You're quite welcome miss.]
    @Belle: We owe you a debt of gratitude. 
    @Belle: If you ever need anything please let us know
    @Clover: Yeah my sister seems very eager to get to know you
    @Clover: Mister...uhh...hey what's your name?
 *   * [My name is Theodore.] 
        @Clover: Right! Mister Todo it is then.
        -> END
        
== function setFlag(x) ==
~return