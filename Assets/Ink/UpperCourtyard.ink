EXTERNAL hasItem(x)

// For editor testing
Where to start?
+ [Clover] -> clover_start

== clover_start ==
@Clover: Get back! I'll have you know I can do karate
* [I'm here to help]
    @Clover: Oh are you here to rescue me? 
    @Clover: Oh goody! I’ve been trapped down here for so long I ate my arm just to survive
        -> CloverBothArms
* [Quiet down before you get hurt] 
    @Clover: Oh are you here to rescue me? 
    @Clover: Oh goody! I’ve been trapped down here for so long I ate my arm just to survive
        -> CloverBothArms
+ [Hello]
    @Clover: I saw something shiny in the other room
    {hasItem("key"):->CloverHasKey|->END}
    
== CloverBothArms ==
* [But...but you have both arms...]
    @Clover: Yes that's true, but I really did think about it!
    {hasItem("key"):->CloverHasKey|->END}
->END

== CloverHasKey ==
@Clover: Oh you found the key.
* [How do you know that?]
    @Clover: I can see your inventory, Duh?
    -> CloverSister
* [Yes, but where does it go?]
    @Clover: Look for a locked door Dummy! 
    @Clover: Then just keep on poking your key till it fits.
    -> CloverSister

== CloverSister ==
@Clover: Wait, have you seen my sister? She's got blue hair and dark fur.
* [Yes! She sent me to find you.]
    @Clover: Yay! I bet she was worried sick teehee
    -> END
* [I think so. She's downstairs.]
    @Clover: Yay! I bet she's worried sick teehee
    -> END
    
== function hasItem(x) ==
~return true