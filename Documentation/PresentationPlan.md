# What is BT

Quick overview of behavior tree for testing can be found here:
https://github.com/Stanislau/behavior-tree-for-testing

In general, behavior tree is a way to systemize testing requirements and scenarios, but it is a tool that should be used carefully.

First we need to understand testing pyramid:
https://www.headspin.io/blog/the-testing-pyramid-simplified-for-one-and-all

Let's review it a bit starting from the bottom - Unit Tests. While it is possible to use BT approach for Unit Tests, it is not recommended, because usually unit tests are too simple and do not contain any interdependent flows. All of them are followed arrange-act-assert pattern (AAA).

In the meantime integration and e2e tests usually consist of chains of AAAs, often intersecting chains.

When we are trying to write integration tests it is common for us to duplicate a lot of flows, like: process cart when user is anonymous, when user is logged in, in foreground, in background, in several different tabs and so on, so a lot of common parts are going to be duplicated. As a result maintanance and visibility of coverage decreasing a lot. We can solve the problem by using Behavior Trees.

Behavior Tree (BT) is a way to organize flows in a form of tree, very similar to flow diagram, but with a bit different notation. More information is here:
https://en.wikipedia.org/wiki/Behavior_tree
https://staff.itee.uq.edu.au/kirsten/publications/BTMergeSemantics_withCopyright.pdf

Below I show how to solve typical exponential tests growth with Behavior Tree.

# Demo

Problem: Need to cover new functionality with integration tests.

Environment: Mobile application, some UI test framework.

We will assume that not all requirements came into account right away. It means that (similar like in real world scenario) we do not have full picture and cannot prepare all test cases.

## Understanding of test cases

Requirements for MVP:
- Tapping on push notification should navigate to feature proposal.
- Once user applies to it, new tab become available for the user.

### MVP testsing

```cs
[Test]
public void TapOnNotification_ApplyToFeature()
{
    var c = new Context();
    c.TapOnNotification();
    c.EnsureAppIsRunning();
    c.EnsureNavigatedToFeatureProposal();
    c.Item.FeatureProposal.Apply();
    c.Item.Feature.IsEnabled.ClaimTrue();
}
```

Notification appears, system taps on notification, ensures app started and navigated to feature proposal, then user applies to feature and system ensures that feature is enabled.

This test is straitforward, it is just happy path, which not take into account any technical pitfalls.

### More cases with some technical details

First of all app can be already running when notification appears.
It splits existing test into two new tests.

```cs
[Test]
public void TapOnNotification_AppIsClosed_ApplyToFeature()
{
    var c = new Context();

    c.EnsureAppIsNotRunning();
    c.TapOnNotification();
    c.EnsureAppIsRunning();

    c.EnsureNavigatedToFeatureProposal();
    c.Item.FeatureProposal.Apply();
    c.Item.Feature.IsEnabled.ClaimTrue();
}

[Test]
public void TapOnNotification_AppIsRunning_ApplyToFeature()
{
    var c = new Context();

    c.EnsureAppIsRunning();

    c.TapOnNotification();
    c.EnsureNavigatedToFeatureProposal();
    c.Item.FeatureProposal.Apply();
    c.Item.Feature.IsEnabled.ClaimTrue();
}
```

As you may notice, almost 80% of entire test bodies are duplicated. It is unavoidable when creating integration tests. The only method how to avoid duplication is to wrap those common lines into methods or classes, and it immediately ruins all the visibility around what is going on in tests.

Next pitfall: user can be logged out for some reason, when notification is tapped.
I would not provide full code, just names of tests, but test bodies contains unavoidable duplications.

```cs
TapOnNotification_AppIsClosed_LoggedIn_ApplyToFeature
TapOnNotification_AppIsRunning_LoggedIn_ApplyToFeature
TapOnNotification_AppIsClosed_NotLoggedIn_ApplyToFeature
TapOnNotification_AppIsRunning_NotLoggedIn_ApplyToFeature
```

As you may see, new requirement causes tests to be doubled. Now the amount of tests are 4 and they are just permutations of preparation steps.

### Maintanance killer

And after all our struggle we realize, that we also need to check failed scenario:
Once user rejects it, new notification icon appeared in the settings.

All our tests are going to double once again, 8 in total.

```cs
TapOnNotification_AppIsClosed_LoggedIn_ApplyToFeature
TapOnNotification_AppIsRunning_LoggedIn_ApplyToFeature
TapOnNotification_AppIsClosed_NotLoggedIn_ApplyToFeature
TapOnNotification_AppIsRunning_NotLoggedIn_ApplyToFeature

TapOnNotification_AppIsClosed_LoggedIn_RejectFeature
TapOnNotification_AppIsRunning_LoggedIn_RejectFeature
TapOnNotification_AppIsClosed_NotLoggedIn_RejectFeature
TapOnNotification_AppIsRunning_NotLoggedIn_RejectFeature
```

## Behavior Tree approach

Here is descibed approach how to define BT from code.
Key concepts:
- Sequence - node-controller, executes all children in sequence.
- Step - block of code to be executed.
- Branching - each new run this node execute new child from the list of children.
- When - conditional executing node, executed it's child only when required node is included into execution flow.

### MVP

```cs
new BehaviorTree(
	new Sequence(
		new Step(
			"Tapping on push notification should navigate to feature proposal. " +
			"Once user applies to it, new tab become available for the user.",
			async c =>
			{
				// preparation part
				c.TapOnNotification();
				c.EnsureAppIsRunning();
				// functionality check part
				c.EnsureNavigatedToFeatureProposal();
				c.Item.FeatureProposal.Apply();
				c.Item.Feature.IsEnabled.ClaimTrue();
			}
		)
	)
)
```

Same as step 1 standalone unit test, so we can start with simple test without any tree in mind.
It provides smooth learning curve and intuitive tree decomposition. You do not need to think about decomposition in advance, you can start writing your tests right away.

### Application and User state

```cs
new BehaviorTree(
	new Sequence(
		new Branching(
			new Step(
				"User is logged in",
				async c =>
				{
					c.PrepareLoggedInContext();
				}
			).As(out var userIsLoggedIn),

			new Step(
				"User is not logged in",
				async c =>
				{
					c.EnsureAppIsNotRunning();
				}
			).As(out var userIsNotLoggedIn)
		),

		new Branching(
			new Step(
				"App is started",
				async c =>
				{
					c.EnsureAppIsRunning();
				}
			).As(out var appIsStarted),

			new Step(
				"App is closed",
				async c =>
				{
					c.EnsureAppIsNotRunning();
				}
			).As(out var appIsClosed)
		),

		new Step(
			"Tap on push notification",
			async c =>
			{
				c.TapOnNotification();
			}
		),

		new When(appIsClosed,
			new Step("Ensure app is running now",
				async c =>
				{
					c.EnsureAppIsRunning();
				})
		),

		new When(userIsNotLoggedIn,
			new Step("Log in and continue the flow",
				async c =>
				{
					c.EnsureNavigatedToLogin();
					c.PerformLogIn();
					c.EnsureUserIsLoggedIn();
				})
		),

		new Step(
			"Tapping on push notification should navigate to feature proposal. " +
			"Once user applies to it, new tab become available for the user.",
			async c =>
			{
				c.EnsureNavigatedToFeatureProposal();
				c.Item.FeatureProposal.Apply();
				c.Item.Feature.IsEnabled.ClaimTrue();
			}
		)
	)
)
```

Here we just moved preparation part into tree-like structure. Created two branches, added some branch specific checks, kept common part.
No code duplication, full visibility, ability to debug each step.
If you traverse this tree using behavior tree rules you'd already come up with 4 scenarios.

N1
- User is logged in
- App is started
- Tap on push notification
- Tapping on push notification should navigate to feature proposal. Once user applies to it, new tab become available for the user.

N2
- User is logged in
- App is closed
- Tap on push notification
- Ensure app is running now
- Tapping on push notification should navigate to feature proposal. Once user applies to it, new tab become available for the user.

N3
- User is not logged in
- App is started
- Tap on push notification
- Log in and continue the flow
- Tapping on push notification should navigate to feature proposal. Once user applies to it, new tab become available for the user.

N4
- User is not logged in
- App is closed
- Tap on push notification
- Ensure app is running now
- Log in and continue the flow
- Tapping on push notification should navigate to feature proposal. Once user applies to it, new tab become available for the user.

Finally, to add the last requirements, we just need to decompose the last step:

```cs
new Step(
	"Tapping on push notification should navigate to feature proposal. " +
	"Once user applies to it, new tab become available for the user.",
	async c =>
	{
		c.EnsureNavigatedToFeatureProposal();
		c.Item.FeatureProposal.Apply();
		c.Item.Feature.IsEnabled.ClaimTrue();
	}
)
```

We move navigation check into common step and branching the rest.

```cs
new Step(
    "Tapping on push notification should navigate to feature proposal.",
    async c =>
    {
        c.EnsureNavigatedToFeatureProposal();
    }
),

new Branching(
    new Step(
        "Once user applies to it, new tab become available for the user.",
        async c =>
        {
            c.Item.FeatureProposal.Apply();
            c.Item.Feature.IsEnabled.ClaimTrue();
        }
    ),

    new Step(
        "Once user rejects it, new notification icon appeared in the settings.",
        async c =>
        {
            c.Item.FeatureProposal.Reject();
            c.Item.Feature.IsEnabled.ClaimFalse();

            c.Item.Feature.IconVisible.ClaimTrue();
        }
    )
)
```

That's it. Adding two more steps added 4 more scenarios.
Imagine that adding each new requirement will generate even more scenarios without expanding codebase.