->bianca_start //Belle start by default, but speaker should jump to their start

== belle_start == 
@Belle: Hi there, I'm Belle the Blacksmith!
@Belle: I'll be able to craft weapons and stuff for you eventually, but right now I don't do a whole lot.
+   [Weapons?]
    @Belle: Yeah, I'm a Blacksmith.
+   [Stuff?]
    @Belle Yeah?
-   @Belle: Safe travels!
    @Bianca: Come say hi!
    -> END

== bianca_start ==
@Bianca: Hi there, I'm bianca!
+   [What's up?]
    @Bianca: Not much!
+   +   [How're things?]
            @Bianca: Oh yeah they're pretty good.
            @Belle: Don't lie, they're crap!
            @Bianca: Yeah nah, they're not so good.
    +   +   +   [Damn]
                ->bianca_goodbye
    +   +   +   [Huh]
                ->bianca_goodbye
    +   +   +   [Aww]
                ->bianca_goodbye
    +   +   [Interesting]
            ->bianca_goodbye
+   [Bye.]
    ->bianca_goodbye

== bianca_goodbye ==
@Bianca: Bye bye!
@Belle: Come say hi!
-> END