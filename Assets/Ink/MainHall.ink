
//For editor testing
Where to start?
+ [Belle] -> belle_start

== belle_start ==
{belle_start < 2:->belle_first|->belle_second}

== belle_first ==
@Belle: !! 
@Belle: Oh My Gosh! Are you alright?? I saw you Tumble in. 
* [Where Am I?]
    @Belle: Well uhh...I'm not sure. I think I'm as lost as you are.
    ->belle_howlong
* [How did I get here]
    @Belle: "I heard screaming and when I came to help, I found you.
    ->belle_howlong

== belle_howlong ==
* [How long was I out...?]
    ->belle_nevermind
* [...Were you watching me that whole time?]
    ->belle_nevermind

== belle_nevermind ==
@Belle: Haha…Nevermind that…ha..haha...
->END

== belle_second ==
@Belle: Back again, huh? 
* [Ugh...Why can't I...remember..."]
    @Belle: We should focus on escaping
* [Who...who are you?] 
    @Belle: My name is Belle. We should try find a way out
-> END

