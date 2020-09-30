There are no tests for this sample at the moment.

Ideally we'd like a full integration test that deploys the function,
brings up a Slack workspace, installs the slash command, then sends
a slash command and checks the response.

It would be possible to write tests with a fake KG API, or to unit
test just aspects such as the formatting, but that would potentially
involve:

- Introducing an IClock for testability
- Making currently-private methods internal in order to test them

If this sample included "how we test it" that would make sense, but
as the tutorial focuses on "getting a slash command to work" I think
it would be distracting.
