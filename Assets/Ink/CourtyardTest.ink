-> belle_start //Belle start by default, but speaker should jump to their start

== belle_start == 
@Belle: Hi there, I'm Belle the Blacksmith!
@Belle: I'll be able to craft weapons and stuff for you eventually, but right now I don't do a whole lot.
+   [Weapons?]
    @Belle: Yeah, I'm a Blacksmith.
    -> belle_goodbye
+   [Stuff?]
    @Belle Yeah?
    ->belle_goodbye
    
== belle_goodbye ==
@Belle: Safe travels!
@Bianca: Come say hi!
-> END

== bianca_start==
@Bianca: Hi there, I'm bianca!
+   [What's up?]
    @Bianca: Not much!
    -> bianca_goodbye
+   [Bye.]
    -> bianca_goodbye

== bianca_goodbye ==
@Bianca: Bye bye!
-> END