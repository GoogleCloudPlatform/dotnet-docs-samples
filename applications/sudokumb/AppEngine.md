### <a name="appengine"><img src="http://cloud.google.com/_static/images/cloud/products/logos/svg/appengine.svg" width=64> Sudokumb automatically scales on [App Engine](https://cloud.google.com/appengine/docs/flexible/dotnet/).</a>

Solving a Sudoku puzzle the dumb way is an intentionally CPU and network intensive task.
What happens when 20 users want to solve a puzzle at the same time?
[App Engine](https://cloud.google.com/appengine/docs/flexible/dotnet/)
automatically [spins up additional instances](https://cloud.google.com/appengine/docs/flexible/dotnet/an-overview-of-app-engine)
to handle the load spike, and then
spins down the instances when the load has returned to normal.

