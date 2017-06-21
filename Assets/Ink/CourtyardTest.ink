EXTERNAL move(x)

//For editor testing
Where to start?
+ [Belle] -> belle_start
+ [Bianca] -> bianca_start

== belle_start == 
{belle_start < 2: @Belle: Hey there, I see you're surviving!|@Belle: Gee, you like talking.}
*   [Where am I?]
    @Belle: An abandoned courtyard or something.
    @Belle: I'm not sure.
    @Belle: Eh, it's probably fine.
    ->rad_place
+   [Yep]
    @Belle: Carry on.
    ->END
        
== rad_place ==
@Bianca: This place is pretty rad though!
*   [Who is that?]
    @Belle: Dunno, why don't you go say hi?
    ->END
+   [Bye]
    @Belle: {~See you later!|Catch you around!|Don't die!}
    ->END

== bianca_start ==
@Bianca: {bianca_start < 10:{This place is pretty rad, huh?|Not sure why I'm here...|This grass feels nice.{move(-1)}|Okay, that's enough talking.}|{Oh my GOD, shut up!|Seriously man.|This is getting ridiculous.|->character_done}}
->END

== character_done ==
... #animation_idle
->END

//Fallback move function to prevent editor errors - does nothing
== function move(x) ==
~ return