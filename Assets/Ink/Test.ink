EXTERNAL move(x)

// For editor testing
Where to start?
+ [Clover] -> clover_start

== clover_start ==
@Clover: Hi there!
@Clover: {&{move(1)}|{move(-1)}} #pause 0.5 #auto continue
@Clover: I'll just stand over here.
->END

== function move(x) ==
!No function bound for move({x})!