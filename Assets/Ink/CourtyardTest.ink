->belle_start //Belle start by default, but speaker should jump to their start

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
@Bianca: This place is pretty rad, huh?
->END