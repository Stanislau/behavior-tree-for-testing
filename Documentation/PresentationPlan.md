# What is BT

# Demo

/*
* @todo #3:20m/Arch Need to test mobile application with different starting points and different results.
*/

## Understanding of test cases

/*
* @todo #3:20m/Arch Describe possible requirements.
* Tapping on push notification should navigate to feature proposal.
* Once user applies to it, new tab become available for the user.
* Once user rejects it, new notification icon appeared in the settings.
*/

### MVP

/*
* @todo #3:20m/Arch Start the app, apply to new app feature.
*/

### More cases with some technical details

/*
* @todo #3:20m/Arch App is started.
*/

/*
* @todo #3:20m/Arch User is not logged in.
*/

### Maintanance killer

/*
* @todo #3:20m/Arch Reject feature.
*/

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
User is logged in
App is started
Tap on push notification
Tapping on push notification should navigate to feature proposal. Once user applies to it, new tab become available for the user.

N2
User is logged in
App is closed
Tap on push notification
Ensure app is running now
Tapping on push notification should navigate to feature proposal. Once user applies to it, new tab become available for the user.

N3
User is not logged in
App is started
Tap on push notification
Log in and continue the flow
Tapping on push notification should navigate to feature proposal. Once user applies to it, new tab become available for the user.

N4
User is not logged in
App is closed
Tap on push notification
Ensure app is running now
Log in and continue the flow
Tapping on push notification should navigate to feature proposal. Once user applies to it, new tab become available for the user.

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