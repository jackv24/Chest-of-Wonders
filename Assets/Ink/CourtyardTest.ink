->belle_start //Belle start by default, but speaker should jump to their start

== belle_start == 
@Belle: Yo whassup I'm a Blacksmith.
+   [That's cool]
    @Belle: Yeah it's pretty rad.
    @Belle: You know what else is rad?
    @Belle: This totally rad place we're in.
    ->rad_place
+   [Where are we?]
    @Belle: Some weird courtyard castle thingo.
    ->rad_place
        
== rad_place ==
@Bianca: It's totally rad and stuff!
+   [Who is that?]
    @Belle: Some random, idk.
    ->END
+   [Bye]
    @Belle: {~See you later!|Catch you around!|Don't die!}
    ->END

== bianca_start ==
@Bianca: This place is pretty rad, huh?
->END